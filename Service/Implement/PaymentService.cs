﻿using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Service.Helper;
using System.Transactions;
using Serilog;
using System.Drawing.Drawing2D;

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
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();
        #endregion

        public async Task CreatePaymentWithDraw(UserDTO userDto, WithdrawRequestDTO withdrawRequestDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var user = await _userRepository.GetUserById(userDto.Id) ?? throw new KeyNotFoundException();
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
                        BankInformation = withdrawRequestDTO.BankNumber + " " + withdrawRequestDTO.BankName.ToString(),
                        Type = (int)EPaymentType.WithDraw,
                    };

                    await _paymentRepository.CreatePaymentHistory(paymentHistory);
                    await SendMailRequestWithDraw(user, withdrawRequestDTO);
                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }

        }

        public async Task<FilterListResponse<PaymentHistoryDTO>> GetAllPayment(PaymentWithDrawFilterDTO filter)
        {
            IEnumerable<PaymentHistory> allPaymentHistories = Enumerable.Empty<PaymentHistory>();
            allPaymentHistories = await _paymentRepository.GetAll();

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

        public async Task ProcessWithdrawalApproval(Guid paymentId,AdminPaymentResponse adminPaymentResponse, UserDTO userDto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var paymentHistory = await _paymentRepository.GetPaymentHistoryPedingById(paymentId) ?? throw new InvalidOperationException("Giao dịch đã được xử lý!");
                    paymentHistory.AdminMessage = adminPaymentResponse.AdminMessage;
                    var user = paymentHistory.User ?? throw new Exception("Không tìm thấy người dùng.!");

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
                        if(adminPaymentResponse.AdminMessage.IsNullOrEmpty())
                        {
                            throw new InvalidOperationException("Lý do từ chối không được để trống.");
                        }
                    }

                    paymentHistory.ResponseAt = DateTime.Now;
                    await _paymentRepository.UpdatePaymentHistory(paymentHistory);

                    paymentHistory.User = null;
                    await _adminActionNotificationHelper.CreateNotification<PaymentHistory>(userDto,
                        (adminPaymentResponse.IsApprove ? EAdminActionType.ApproveWithDraw : EAdminActionType.RejectWithDraw)
                        ,paymentHistory, null) ;

                    await SendMailResponseWithDraw(user, paymentHistory);
                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }
        }

        protected async Task<decimal> GetWithDrawFee()
        {
            var fee = await _systemSettingRepository.GetSystemSetting(_configManager.WithDrawFeeKey) ?? throw new Exception("Has error when get WithDraw Fee");
            return decimal.Parse(fee.KeyValue!.ToString()!);
        }

        public async Task UpdateVipPaymentRequest(Guid userId,UpdateVipRequestDTO updateVipRequest)
        {
            var user = await _userRepository.GetUserById(userId) ?? throw new KeyNotFoundException();
            //tren 3 thang thi discount 15%
            var discount = await _systemSettingRepository.GetSystemSetting(_configManager.UpdatePremiumDiscount) ?? throw new Exception("Has error when get Discount");
            var discountValue = decimal.Parse(discount.KeyValue!.ToString()!);
            var amount = await _systemSettingRepository.GetSystemSetting(_configManager.PremiumPrice) ?? throw new Exception("Has error when get Discount");
            var amountValue = decimal.Parse(amount.KeyValue!.ToString()!);

            var totalAmount = updateVipRequest.NumberMonthsRegis <3 ? amountValue * updateVipRequest.NumberMonthsRegis: amountValue * updateVipRequest.NumberMonthsRegis *(1 - discountValue);
            var paymentHistory = new PaymentHistory()
            {
                UserId = userId,
                Amount = totalAmount,
                BankInformation = user.Email,
                NetAmount = totalAmount,
                Type =(int) EPaymentType.BuyPremium,
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
                    .Replace("{BankAccount}", withdrawRequestDTO.BankNumber + " " + withdrawRequestDTO.BankName.ToString())
                    .Replace("{CreatedAt}", DateTime.Now.ToString())
                    .Replace("{Link}",  "")
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
                    .Replace("{BankAccount}",payment.BankInformation)
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
        public async Task SendMailResponseUpdatePremium(Brand user, PaymentHistory payment,bool isApprove,string adminMessage)
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

                _ = Task.Run(async () => await _emailService.SendEmail(new List<string> { user.User.Email }, subject, body ));
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
    }
}
