using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Repositories;
using Serilog;
using Service.Helper;
using Service.Implement.UtilityServices;
using Service.Interface.UtilityServices;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Transactions;
using System.Web;

namespace Service
{
    public class PaymentService : IPaymentService
    {
        #region Config
        private static readonly AdminActionNotificationHelper _adminActionNotificationHelper = new AdminActionNotificationHelper();
        private static ISystemSettingRepository _systemSettingRepository = new SystemSettingRepository();
        private static readonly IPaymentRepository _paymentRepository = new PaymentRepository();
        private static readonly IBrandRepository _brandRepository = new BrandRepository();
        private static readonly IUserRepository _userRepository = new UserRepository();
        private static readonly EmailTemplate _emailTemplate = new EmailTemplate();
        private static readonly IEmailService _emailService = new EmailService();
        private static readonly ConfigManager _configManager = new ConfigManager();
        private static ILogger _loggerService = new LoggerService().GetDbLogger();
        private static readonly HttpClient client = new HttpClient();
        private static readonly IEnvService _envService = new EnvService();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();
        #endregion

        public async Task<string> CreateWithDrawQR(Guid requestId)
        {
            string vietQR = @"https://img.vietqr.io/image/<BANK_ID>-<ACCOUNT_NO>-<TEMPLATE>.png?amount=<AMOUNT>&addInfo=<DESCRIPTION>&accountName=<ACCOUNT_NAME>";
            var request = await _paymentRepository.GetPaymentHistoryById(requestId);
            var bankInfor = request.BankInformation.Split(' ') ?? throw new Exception("Bank Information are null or not correct!");
            var accountName = HttpUtility.UrlEncode(request?.User?.DisplayName ?? "");

            vietQR = vietQR.Replace("<ACCOUNT_NO>", bankInfor[1])
                .Replace("<BANK_ID>", bankInfor[0])
                .Replace("<TEMPLATE>", "compact2")
                .Replace("<AMOUNT>", request?.Amount.ToString())
                .Replace("<DESCRIPTION>", requestId.ToString())
                .Replace("<ACCOUNT_NAME>", accountName);

            return vietQR;
        }

        public async Task CreatePaymentWithDraw(UserDTO userDto, WithdrawRequestDTO withdrawRequestDTO)
        {
            var user = await _userRepository.GetUserById(userDto.Id) ?? throw new KeyNotFoundException();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (withdrawRequestDTO.Amount > user.Wallet)
                    {
                        throw new InvalidOperationException("Số tiền vượt mức cho phép!");
                    }

                    if (withdrawRequestDTO.Amount < 10000 || withdrawRequestDTO.Amount > 50000000)
                    {
                        throw new InvalidOperationException("Số tiền phải từ 10 nghìn đến dưới 50 triệu!");
                    }

                    user.Wallet = user.Wallet - withdrawRequestDTO.Amount;
                    await _userRepository.UpdateUser(user);

                    var paymentHistory = new PaymentHistory()
                    {
                        UserId = userDto.Id,
                        Amount = withdrawRequestDTO.Amount,
                        BankInformation = withdrawRequestDTO.BankId + " " + withdrawRequestDTO.AccountNo.ToString(),
                        Type = (int)EPaymentType.WithDraw,
                        Status = (int)EPaymentStatus.Pending
                    };

                    await _paymentRepository.CreatePaymentHistory(paymentHistory);
                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }

            await SendMailRequestWithDraw(user, withdrawRequestDTO);
        }

        public async Task<FilterListResponse<PaymentHistoryDTO>> GetAllPayment(PaymentWithDrawFilterDTO filter)
        {
            IEnumerable<PaymentHistory> allPaymentHistories = Enumerable.Empty<PaymentHistory>();
            allPaymentHistories = await _paymentRepository.GetAllPaymentsHistory();

            #region Filter
            if (filter.PaymentType != null && filter.PaymentType.Any())
            {
                allPaymentHistories = allPaymentHistories.Where(i => filter.PaymentType.Contains((EPaymentType)i.Type!)).ToList();
            }

            if (filter.PaymentStatus != null && filter.PaymentStatus.Any())
            {
                allPaymentHistories = allPaymentHistories.Where(i => filter.PaymentStatus.Contains((EPaymentStatus)i.Status!)).ToList();
            }

            #endregion

            int totalCount = allPaymentHistories.Count();

            #region Sort
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var propertyInfo = typeof(PaymentHistory).GetProperty(filter.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    allPaymentHistories = filter.IsAscending.HasValue && filter.IsAscending.Value
                        ? allPaymentHistories.OrderBy(i => propertyInfo.GetValue(i, null))
                        : allPaymentHistories.OrderByDescending(i => propertyInfo.GetValue(i, null));
                }
            }
            #endregion


            #region Paging
            int pageSize = filter.PageSize;
            allPaymentHistories = allPaymentHistories
                .Skip((filter.PageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            #endregion

            return new FilterListResponse<PaymentHistoryDTO>
            {
                TotalCount = totalCount,
                Items = _mapper.Map<IEnumerable<PaymentHistoryDTO>>(allPaymentHistories)
            };
        }

        public async Task<bool> ProcessWithdrawalApproval(Guid paymentId, AdminPaymentResponse adminPaymentResponse, UserDTO userDto)
        {
            var paymentHistory = await _paymentRepository.GetPaymentHistoryPedingById(paymentId) ?? throw new InvalidOperationException("Giao dịch đã được xử lý!");
            paymentHistory.AdminMessage = adminPaymentResponse.AdminMessage;
            var user = paymentHistory.User ?? throw new KeyNotFoundException("Không tìm thấy người dùng.!");
            var isPaymentSuccess = false;

            if (adminPaymentResponse.IsApprove)
            {
                isPaymentSuccess = await CheckPaymentStatus(paymentId);
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (adminPaymentResponse.IsApprove)
                    {
                        if (isPaymentSuccess)
                        {
                            //đã chuyển tiền thành công
                            paymentHistory.Status = (int)EPaymentStatus.Done;
                            var fee = await GetWithDrawFee();
                            paymentHistory.AdminMessage = $"Do chính sách của trang web, mỗi giao dịch rút tiền sẽ chịu một khoản phí dịch vụ {fee * 100}% trên tổng số tiền rút." +
                                $" Điều này nhằm đảm bảo cho các hoạt động vận hành và duy trì dịch vụ chất lượng." +
                                $" Vì vậy, với yêu cầu rút {paymentHistory.Amount!.ToString("N2")} VND của bạn. " +
                                $" Sau khi trừ phí, số tiền bạn sẽ nhận được thực tế là {(paymentHistory.Amount - paymentHistory.Amount * fee).ToString("N2")} VND";
                            paymentHistory.NetAmount = paymentHistory.Amount * fee;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (adminPaymentResponse.AdminMessage.IsNullOrEmpty())
                        {
                            throw new InvalidOperationException("Lý do từ chối không được để trống.");
                        }

                        user.Wallet = user.Wallet + paymentHistory.Amount;
                        await _userRepository.UpdateUser(user);

                        paymentHistory.Status = (int)EPaymentStatus.Rejected;
                        paymentHistory.NetAmount = 0;
                    }

                    paymentHistory.ResponseAt = DateTime.Now;
                    await _paymentRepository.UpdatePaymentHistory(paymentHistory);

                    paymentHistory.User = null;
                    await _adminActionNotificationHelper.CreateNotification(userDto
                                                                        , adminPaymentResponse.IsApprove ? EAdminActionType.ApproveWithDraw : EAdminActionType.RejectWithDraw
                                                                        , paymentHistory, null);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _loggerService.Error("Has error while process ProcessWithdrawalApproval. Exception: " + ex);
                    return false;
                }
            }
            await SendMailResponseWithDraw(user, paymentHistory);
            return true;
        }

        private async Task<bool> CheckPaymentStatus(Guid paymentId)
        {
            try
            {
                var ApiKey = _envService.GetEnv("cassoApiKey");
                var ApiUri = "https://oauth.casso.vn/v2/transactions?sort=DESC";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Apikey", ApiKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var paymentCode = paymentId.ToString().Replace("-", "");
                    HttpResponseMessage response = await client.GetAsync(ApiUri);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        JObject parsedJson = JObject.Parse(responseData);
                        var records = parsedJson["data"]?["records"]?.FirstOrDefault(r => r?["description"]?.ToString()?.Contains(paymentCode) == true);

                        return records != null && records.HasValues;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Error("Has error while checking payment status. Exception: " + ex);
                throw;
            }
        }


        protected async Task<decimal> GetWithDrawFee()
        {
            var fee = await _systemSettingRepository.GetSystemSetting(_configManager.WithDrawFeeKey) ?? throw new Exception("Has error when get WithDraw Fee");
            return decimal.Parse(fee.KeyValue!.ToString()!);
        }

        public async Task UpdateVipPaymentRequest(string requestDto)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    var request = JsonSerializer.Deserialize<ExtraDataDTO>(requestDto);
                    var brand = await _brandRepository.GetByUserId(request.User.Id) ?? throw new KeyNotFoundException();

                    var paymentHistory = new PaymentHistory()
                    {
                        UserId = brand.UserId,
                        Amount = request.TotalAmount,
                        BankInformation = null,
                        NetAmount = request.TotalAmount,
                        Type = (int)EPaymentType.BuyPremium,
                        Status = (int)EPaymentStatus.Done,
                        ResponseAt = DateTime.UtcNow,

                        AdminMessage = $"Yêu cầu trở thành nhãn hàng Premium đã được hoàn tất."
                    };
                    await _paymentRepository.CreatePaymentHistory(paymentHistory);
                    if (brand.IsPremium = true && brand.PremiumValidTo.HasValue)
                    {
                        if (request.NumberMonthsRegis == 1)
                        {
                            brand.PremiumValidTo = (brand.PremiumValidTo ?? DateTime.UtcNow).AddMonths(1);
                        }
                        else if (request.NumberMonthsRegis == 3)
                        {
                            brand.PremiumValidTo = (brand.PremiumValidTo ?? DateTime.UtcNow).AddMonths(3);
                        }
                    }
                    else
                    {
                        brand.IsPremium = true;
                        if (request.NumberMonthsRegis == 1)
                        {
                            brand.PremiumValidTo = DateTime.UtcNow.AddMonths(1);

                        }
                        else if (request.NumberMonthsRegis == 3)
                        {
                            brand.PremiumValidTo = DateTime.UtcNow.AddMonths(3);

                        }
                    }
                    await _brandRepository.UpdateBrand(brand);
                    scope.Complete();
                    await SendMailResponseUpdatePremium(brand, paymentHistory, true, paymentHistory.AdminMessage);
                }
            }
            catch
            {
                throw;
            }
        }

        #region SendMail
        public async Task SendMailRequestWithDraw(User user, WithdrawRequestDTO withdrawRequestDTO)
        {
            try
            {
                string subject = "Thông Báo Yêu Cầu Rút Tiền";
                var body = _emailTemplate.requestWithDrawTemplate
                    .Replace("{DisplayName}", user.DisplayName)
                    .Replace("{Amount}", withdrawRequestDTO.Amount.ToString("N2"))
                    .Replace("{Money}", user.Wallet.ToString("N2"))
                    .Replace("{BankAccount}", withdrawRequestDTO.BankId + " " + withdrawRequestDTO.AccountNo.ToString())
                    .Replace("{CreatedAt}", DateTime.Now.ToString())
                    .Replace("{Link}", "")
                    .Replace("{projectName}", _configManager.ProjectName);

                _ = Task.Run(async () => await _emailService.SendEmail(_configManager.AdminPaymentHandler, subject, body));
            }
            catch (Exception ex)
            {
                _loggerService.Error("Lỗi khi gửi mail trạng thái thanh toán" + ex);
            }
        }
        public async Task SendMailResponseWithDraw(User user, PaymentHistory payment)
        {
            try
            {
                string subject = "Thông Báo Phản Hồi về yêu cầu rút tiền";
                var body = _emailTemplate.responseWithDrawTemplate
                    .Replace("{DisplayName}", user.DisplayName)
                    .Replace("{Status}", payment.Status == (int)EPaymentStatus.Rejected ? "bị từ chối" : "được chấp thuận")
                    .Replace("{Withdraw}", payment.Amount.ToString("N2"))
                    .Replace("{Money}", user.Wallet.ToString("N2"))
                    .Replace("{BankAccount}", payment.BankInformation)
                    .Replace("{CreatedAt}", payment.CreatedAt.ToString())
                    .Replace("{ResponseAt}", DateTime.Now.ToString())
                    .Replace("{Description}", payment.AdminMessage)
                    .Replace("{Link}", "")
                    .Replace("{projectName}", _configManager.ProjectName);

                _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { user.Email }, subject, body));
            }
            catch (Exception ex)
            {
                _loggerService.Error("Lỗi khi gửi mail trạng thái thanh toán" + ex);
            }
        }
        public async Task SendMailResponseUpdatePremium(Brand user, PaymentHistory payment, bool isApprove, string adminMessage)
        {
            try
            {
                string subject = "Thông Báo Phản Hồi về yêu cầu nâng cấp tài khoản Premium";
                var body = string.Empty;
                if (isApprove)
                {
                    body = _emailTemplate.ApproveUpdatePremium
                    .Replace("{BrandName}", user.User.DisplayName)
                    .Replace("{validDate}", user.PremiumValidTo.ToString())
                    .Replace("{Link}", "")
                    .Replace("{projectName}", _configManager.ProjectName);
                }
                else
                {
                    body = _emailTemplate.RejectUpdatePremium
                    .Replace("{BrandName}", user.User.DisplayName)
                    .Replace("{Link}", "")
                    .Replace("{adminMessage}", adminMessage)
                    .Replace("{projectName}", _configManager.ProjectName);
                }

                _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { user.User.Email }, subject, body));
            }
            catch (Exception ex)
            {
                _loggerService.Error("Lỗi khi gửi mail trạng thái thanh toán" + ex);
            }
        }
        #endregion


        public async Task<PaymentCollectionLinkResponse> UpdatePremium(UpdatePremiumRequestDTO updatePremiumRequestDTO, UserDTO userDto)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();

            var amount = await _systemSettingRepository.GetSystemSetting(_configManager.PremiumPrice) ?? throw new Exception("Has error when get Discount");
            var amountValue = decimal.Parse(amount.KeyValue!.ToString()!);
            var totalAmount = amountValue * updatePremiumRequestDTO.NumberMonthsRegis;
            if (updatePremiumRequestDTO.NumberMonthsRegis >= 3)
            {
                var discount = await _systemSettingRepository.GetSystemSetting(_configManager.UpdatePremiumDiscount) ?? throw new Exception("Has error when get Discount");
                var discountValue = 1 - decimal.Parse(discount.KeyValue!.ToString()!) / 100;
                totalAmount = totalAmount * discountValue;
            }

            var extraData = new ExtraDataDTO()
            {
                User = userDto,
                TotalAmount = totalAmount,
                NumberMonthsRegis = updatePremiumRequestDTO.NumberMonthsRegis
            };

            CollectionLinkRequest request = new CollectionLinkRequest();
            request.orderInfo = "UPDATE PREMIUM";
            request.partnerCode = "MOMO";
            request.ipnUrl = _envService.GetEnv("Payment_URL") + "/api/Payment/updatePremium/callback";
            request.redirectUrl = updatePremiumRequestDTO.redirectUrl;
            request.amount = (long)totalAmount;
            request.orderId = myuuidAsString;
            request.requestId = myuuidAsString;
            request.requestType = "payWithMethod";
            request.extraData = JsonSerializer.Serialize(extraData);
            request.partnerName = "MoMo Payment";
            request.storeId = "AdFusion";
            request.orderGroupId = "";
            request.autoCapture = true;
            request.lang = "vi";

            var response = await CreateCollectionLinkAsync(request);
            return response;
        }

        public async Task<PaymentCollectionLinkResponse> Deposit(DepositRequestDTO depositRequestDTO, UserDTO userDto)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            string brandId = userDto.Id.ToString();

            CollectionLinkRequest request = new CollectionLinkRequest();
            request.orderInfo = "DEPOSIT";
            request.partnerCode = "MOMO";
            request.ipnUrl = _envService.GetEnv("Payment_URL") + "/api/Payment/deposit/callback";
            request.redirectUrl = depositRequestDTO.redirectUrl;
            request.amount = depositRequestDTO.amount;
            request.orderId = myuuidAsString;
            request.requestId = myuuidAsString;
            request.requestType = "payWithMethod";
            request.extraData = brandId;
            request.partnerName = "MoMo Payment";
            request.storeId = "AdFusion";
            request.orderGroupId = "";
            request.autoCapture = true;
            request.lang = "vi";

            var response = await CreateCollectionLinkAsync(request);
            return response;
        }

        public async Task DepositCallBack(CallbackDTO callbackDTO)
        {
            if (callbackDTO.resultCode != 0)
            {
                _loggerService.Error(JsonSerializer.Serialize(callbackDTO));
                return;
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var user = await _userRepository.GetUserById(Guid.Parse(callbackDTO.extraData)) ?? throw new KeyNotFoundException();
                    user.Wallet += callbackDTO.amount;


                    var paymentHistory = new PaymentHistory()
                    {
                        UserId = user.Id,
                        Amount = callbackDTO.amount,
                        BankInformation = callbackDTO.partnerCode + " " + callbackDTO.payType,
                        Type = (int)EPaymentType.Deposit,
                        Status = (int)EPaymentStatus.Done,
                    };

                    await _userRepository.UpdateUser(user);
                    await _paymentRepository.CreatePaymentHistory(paymentHistory);
                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }

        }

        public async Task UpdatePremiumCallBack(CallbackDTO callbackDTO)
        {
            if (callbackDTO.resultCode != 0)
            {
                _loggerService.Error(JsonSerializer.Serialize(callbackDTO));
                return;
            }

            await UpdateVipPaymentRequest(callbackDTO.extraData);
        }

        private static async Task<PaymentCollectionLinkResponse> CreateCollectionLinkAsync(CollectionLinkRequest request)
        {
            string accessKey = "F8BBA842ECF85";
            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&extraData=" + request.extraData + "&ipnUrl=" + request.ipnUrl
                            + "&orderId=" + request.orderId + "&orderInfo=" + request.orderInfo + "&partnerCode=" + request.partnerCode
                            + "&redirectUrl=" + request.redirectUrl + "&requestId=" + request.requestId + "&requestType=" + request.requestType;
            request.signature = getSignature(rawSignature, secretKey);

            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);
            var contents = quickPayResponse.Content.ReadAsStringAsync().Result;
            var responseDto = JsonSerializer.Deserialize<PaymentCollectionLinkResponse>(contents);
            return responseDto;
        }

        private static string getSignature(string text, string key)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            byte[] textBytes = encoding.GetBytes(text);
            byte[] keyBytes = encoding.GetBytes(key);

            byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
