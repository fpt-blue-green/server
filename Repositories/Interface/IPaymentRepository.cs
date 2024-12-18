﻿using BusinessObjects.Models;

namespace Repositories
{
    public interface IPaymentRepository
    {
        Task CreatePaymentBooking(PaymentBooking paymentBooking);
        Task CreatePaymentHistory(PaymentHistory paymentHistory);
        Task<PaymentHistory> GetPaymentHistoryById(Guid id);
        Task UpdatePaymentHistory(PaymentHistory paymentHistory);
        Task<IEnumerable<PaymentHistory>> GetAll();
        Task<PaymentHistory> GetPaymentHistoryPedingById(Guid id);
        Task<IEnumerable<PaymentHistory>> GetAllProfitPayment();
        Task<IEnumerable<PaymentHistory>> GetAllProfitPaymentIgnoreFilter();
        Task<List<PaymentHistory>> GetWithDrawPaymentHistoryByUserId(Guid id);
        Task<IEnumerable<PaymentHistory>> GetAllPaymentsHistory();
        Task<PaymentBooking> GetInfluencerPaymentByJobId(Guid jobId);
        Task<PaymentBooking> GetBrandPaymentByJobId(Guid jobId);
    }
}
