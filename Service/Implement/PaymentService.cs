using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Serilog;
using Service.Helper;
using Service.Implement.UtilityServices;
using Service.Interface.UtilityServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Transactions;

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

        public async Task<string> CreateWithDrawQR(WithdrawRequestDTO withdrawRequestDTO, Guid requestId)
        {
            string vietQR = @"https://img.vietqr.io/image/<BANK_ID>-<ACCOUNT_NO>-<TEMPLATE>.png?amount=<AMOUNT>&addInfo=<DESCRIPTION>&accountName=<ACCOUNT_NAME>";
            var request = await _paymentRepository.GetPaymentHistoryById(requestId);

            vietQR = vietQR.Replace("<ACCOUNT_NO>", withdrawRequestDTO.AccountNo)
                .Replace("<BANK_ID>", withdrawRequestDTO.BankId)
                .Replace("<TEMPLATE>", "compact2")
                .Replace("<AMOUNT>", withdrawRequestDTO.Amount.ToString())
                .Replace("<DESCRIPTION>", requestId.ToString())
                .Replace("<ACCOUNT_NAME>", request.User.DisplayName);

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

                    if (withdrawRequestDTO.Amount < 100000 || withdrawRequestDTO.Amount > 50000000)
                    {
                        throw new InvalidOperationException("Số tiền phải từ 100 nghìn đến dưới 50 triệu!");
                    }

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
                allPaymentHistories = allPaymentHistories.Where(i => filter.PaymentType.Contains((EPaymentType)i.Status!)).ToList();
            }

            if (filter.PaymentStatus != null && filter.PaymentStatus.Any())
            {
                allPaymentHistories = allPaymentHistories.Where(i => filter.PaymentStatus.Contains((EPaymentStatus)i.Status!)).ToList();
            }

            #endregion

            int totalCount = allPaymentHistories.Count();
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

        public async Task ProcessWithdrawalApproval(Guid paymentId, AdminPaymentResponse adminPaymentResponse, UserDTO userDto)
        {
            var paymentHistory = await _paymentRepository.GetPaymentHistoryPedingById(paymentId) ?? throw new InvalidOperationException("Giao dịch đã được xử lý!");
            paymentHistory.AdminMessage = adminPaymentResponse.AdminMessage;
            var user = paymentHistory.User ?? throw new Exception("Không tìm thấy người dùng.!");

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                  
                    if (adminPaymentResponse.IsApprove)
                    {
                        paymentHistory.Status = (int)EPaymentStatus.Done;
                        var fee = await GetWithDrawFee();
                        paymentHistory.AdminMessage = $"Do chính sách của trang web, mỗi giao dịch rút tiền sẽ chịu một khoản phí dịch vụ {fee}% trên tổng số tiền rút." +
                            $" Điều này nhằm đảm bảo cho các hoạt động vận hành và duy trì dịch vụ chất lượng." +
                            $" Vì vậy, với yêu cầu rút {paymentHistory.Amount!.ToString("N2")} VND của bạn. " +
                            $" Sau khi trừ phí, số tiền bạn sẽ nhận được thực tế là {(paymentHistory.Amount - (paymentHistory.Amount * fee / 100)).ToString("N2")} VND";
                        paymentHistory.NetAmount = (paymentHistory.Amount * fee / 100);
                        user.Wallet = user.Wallet - (paymentHistory.Amount);
                        await _userRepository.UpdateUser(user);
                    }
                    else
                    {
                        paymentHistory.Status = (int)EPaymentStatus.Rejected;
                        if (adminPaymentResponse.AdminMessage.IsNullOrEmpty())
                        {
                            throw new InvalidOperationException("Lý do từ chối không được để trống.");
                        }
                    }

                    paymentHistory.ResponseAt = DateTime.Now;
                    await _paymentRepository.UpdatePaymentHistory(paymentHistory);

                    paymentHistory.User = null;
                    await _adminActionNotificationHelper.CreateNotification<PaymentHistory>(userDto,
                        (adminPaymentResponse.IsApprove ? EAdminActionType.ApproveWithDraw : EAdminActionType.RejectWithDraw)
                        , paymentHistory, null);
                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }
            await SendMailResponseWithDraw(user, paymentHistory);
        }

        protected async Task<decimal> GetWithDrawFee()
        {
            var fee = await _systemSettingRepository.GetSystemSetting(_configManager.WithDrawFeeKey) ?? throw new Exception("Has error when get WithDraw Fee");
            return decimal.Parse(fee.KeyValue!.ToString()!);
        }

        public async Task UpdateVipPaymentRequest(string requestDto)
        {
            var request = JsonSerializer.Deserialize<ExtraDataDTO>(requestDto);
            var user = await _userRepository.GetUserById(request.BrandId) ?? throw new KeyNotFoundException();
            //tren 3 thang thi discount 15%
            var paymentHistory = new PaymentHistory()
            {
                UserId = user.Id,
                Amount = request.TotalAmount,
                BankInformation = user.Email ?? "",
                NetAmount = request.TotalAmount,
                Type = (int)EPaymentType.BuyPremium,
                Status = (int)EPaymentStatus.Pending,

            };
            await _paymentRepository.CreatePaymentHistory(paymentHistory);

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

        public async Task ProcessUpdatePremiumApproval(Guid paymentId, AdminPaymentResponse adminPaymentResponse, UserDTO userDto)
        {
            var paymentHistory = await _paymentRepository.GetPaymentHistoryPedingById(paymentId) ?? throw new InvalidOperationException("Giao dịch đã được xử lý!");
            var brand = await _brandRepository.GetByUserId(paymentHistory.UserId) ?? throw new Exception("Không tìm thấy người dùng.!");

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    paymentHistory.AdminMessage = adminPaymentResponse.AdminMessage;
                    if (adminPaymentResponse.IsApprove)
                    {
                        paymentHistory.Status = (int)EPaymentStatus.Done;
                        paymentHistory.AdminMessage = $"Yêu cầu trở thành nhãn hàng Premium đã được hoàn tất.";
                        var premiumPrice = await _systemSettingRepository.GetSystemSetting(_configManager.PremiumPrice) ?? throw new Exception("Has error when get Premium Price");
                        try
                        {
                            if (brand.PremiumValidTo.HasValue)
                            {
                                if (decimal.Parse(premiumPrice.KeyValue!.ToString()!) == paymentHistory.Amount)
                                {
                                    brand.PremiumValidTo = (brand.PremiumValidTo ?? DateTime.UtcNow).AddMonths(1);
                                }
                                else
                                {
                                    brand.PremiumValidTo = (brand.PremiumValidTo ?? DateTime.UtcNow).AddMonths(3);
                                }
                            }
                            else
                            {
                                if (decimal.Parse(premiumPrice.KeyValue!.ToString()!) == paymentHistory.Amount)
                                {
                                    (brand.PremiumValidTo ?? DateTime.UtcNow).AddMonths(1);
                                }
                                else
                                {
                                    (brand.PremiumValidTo ?? DateTime.UtcNow).AddMonths(3);
                                }
                            }
                        }
                        catch
                        {
                            throw new InvalidOperationException("Có lỗi khi nâng cấp tài khoản này lên tài khoản Premium , hãy kiểm tra lại.");
                        }

                        await _brandRepository.UpdateBrand(brand);
                    }
                    else
                    {
                        paymentHistory.Status = (int)EPaymentStatus.Rejected;
                        if (adminPaymentResponse.AdminMessage.IsNullOrEmpty())
                        {
                            throw new InvalidOperationException("Lý do từ chối không được để trống.");
                        }
                    }
                    paymentHistory.ResponseAt = DateTime.Now;
                    await _paymentRepository.UpdatePaymentHistory(paymentHistory);
                    paymentHistory.User = null;
                    await _adminActionNotificationHelper.CreateNotification<PaymentHistory>(userDto,
                        (adminPaymentResponse.IsApprove ? EAdminActionType.ApproveUpdatePremium : EAdminActionType.RejectUpdatePremium)
                        , paymentHistory, null);

                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }
            await SendMailResponseUpdatePremium(brand, paymentHistory, adminPaymentResponse.IsApprove, adminPaymentResponse.AdminMessage);
        }
      
        public async Task<PaymentCollectionLinkResponse> UpdatePremium(UpdatePremiumRequestDTO updatePremiumRequestDTO, UserDTO userDto)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            Guid brandId = userDto.Id;

            var amount = await _systemSettingRepository.GetSystemSetting(_configManager.PremiumPrice) ?? throw new Exception("Has error when get Discount");
            var amountValue = decimal.Parse(amount.KeyValue!.ToString()!);
            var totalAmount = amountValue * updatePremiumRequestDTO.NumberMonthsRegis ;
            if (updatePremiumRequestDTO.NumberMonthsRegis >= 3)
            {
                var discount = await _systemSettingRepository.GetSystemSetting(_configManager.UpdatePremiumDiscount) ?? throw new Exception("Has error when get Discount");
                var discountValue = 1 - decimal.Parse(discount.KeyValue!.ToString()!)/100;
                totalAmount = totalAmount * discountValue;
            }
            var extraData = new ExtraDataDTO()
            {
                BrandId = brandId,
                TotalAmount = totalAmount
            };
            CollectionLinkRequest request = new CollectionLinkRequest();
            request.orderInfo = "UPDATE PREMIUM";
            request.partnerCode = "MOMO";
            //TODO:
            request.ipnUrl = _envService.GetEnv("Payment_URL") + "api/Payment/updatePremium/callback";
            //TODO:
            request.redirectUrl = updatePremiumRequestDTO.redirectUrl;
            request.amount = (long) totalAmount;
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
            // su dung ngrok cua chinh ban than
            //TODO:
            request.ipnUrl = _envService.GetEnv("Payment_URL") + "api/Payment/deposit/callback";
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
                    user.Wallet += callbackDTO.amount * 0.8m;


                    var paymentHistory = new PaymentHistory()
                    {
                        UserId = user.Id,
                        Amount = callbackDTO.amount,
                        BankInformation = callbackDTO.partnerCode + " " + callbackDTO.payType,
                        Type = (int)EPaymentType.BrandPayment,
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

            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);
            var contents = quickPayResponse.Content.ReadAsStringAsync().Result;
            var responseDto = JsonSerializer.Deserialize<PaymentCollectionLinkResponse>(contents);
            return responseDto;
        }

        private static String getSignature(String text, String key)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

      
    }
}
