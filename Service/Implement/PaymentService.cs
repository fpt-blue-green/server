using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using Repositories;
using System.Transactions;
using static Quartz.Logging.OperationName;

namespace Service
{
    public class PaymentService : IPaymentService
    {
        private static readonly IPaymentRepository _paymentRepository = new PaymentRepository();
        private static readonly IUserRepository _userRepository = new UserRepository();
        private readonly IMapper _mapper;

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

                    if (withdrawRequestDTO.Amount <= 10000 || withdrawRequestDTO.Amount <= 900000000)
                    {
                        throw new InvalidOperationException("Số tiền phải lớn hơn 10000 và nhỏ hơn 900 triệu!");
                    }

                    var paymentHistory = new PaymentHistory()
                    {
                        UserId = userDto.Id,
                        Amount = withdrawRequestDTO.Amount,
                        BankInformation = withdrawRequestDTO.BankNumber + " " + withdrawRequestDTO.BankName.ToString(),
                        AdminMessage = null,
                        Type = (int)EPaymentType.WithDraw,
                    };

                    await _paymentRepository.CreatePaymentHistory(paymentHistory);
                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }

        }

        public async Task<PaymentResponseDTO> GeAllPayment(PaymentWithDrawFilterDTO filter)
        {
            IEnumerable<PaymentHistory> allPaymentHistories = Enumerable.Empty<PaymentHistory>();
            allPaymentHistories = await _paymentRepository.GetAll();

            #region Filter
            if (filter.PaymentType != null && filter.PaymentType.Contains(EPaymentType.WithDraw))
            {
                allPaymentHistories = allPaymentHistories.Where(i => i.Type == (int)EPaymentType.WithDraw);
            }

            if (filter.PaymentStatus != null && filter.PaymentStatus.Any())
            {
                allPaymentHistories = allPaymentHistories.Where(i => filter.PaymentStatus.Contains((EPaymentStatus)i.Status!)).ToList();
            }

            #endregion
            int totalCount = allPaymentHistories.Count();

            #region Paging
            int pageSize = filter.PageSize;
            var pagePaymentHistories = allPaymentHistories
                .Skip((filter.PageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            #endregion
            
            return new PaymentResponseDTO
            {
                TotalCount = totalCount,
                PaymentHistories = _mapper.Map<IEnumerable<PaymentHistoryDTO>>(pagePaymentHistories)
            };
        }

        public async Task ResponseWithDraw(AdminPaymentResponse adminPaymentResponse, Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var paymentHistory = await _paymentRepository.GetPaymentHistoryById(id);
                    if (paymentHistory == null)
                    {
                        throw new InvalidOperationException("Không tìm thấy lịch sử giao dịch.!");
                    }

                    if (adminPaymentResponse.IsApprove)
                    {
                        paymentHistory.Status = (int)EPaymentStatus.Done;

                    }
                    else
                    {
                        paymentHistory.Status = (int)EPaymentStatus.Rejected;
                    }

                    paymentHistory.AdminMessage = adminPaymentResponse.AdminMessage;
                    paymentHistory.ResponseAt = DateTime.Now;
                    await _paymentRepository.UpdatePaymentHistory(paymentHistory);

                    if (paymentHistory.UserId.HasValue)
                    {
                        var user = await _userRepository.GetUserById(paymentHistory.UserId.Value);
                        if (user != null)
                        {
                            user.Wallet = user.Wallet - (int)paymentHistory.Amount;
                            await _userRepository.UpdateUser(user);
                        }
                        else
                        {
                            throw new InvalidOperationException("Không tìm thấy người dùng.!");
                        }
                    }

                    scope.Complete();
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
