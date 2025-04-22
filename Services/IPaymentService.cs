using wspay_v2.Models;
using wspay_v2.Result;

namespace wspay_v2.Services
{
    public interface IPaymentService
    {
        public Task<Result<PaymentDetailsDto>> GetPaymentDetailsDto(int? id);
        public Task<Result<Payment>> CreatePaymentAsync(Payment payment);
    }
}