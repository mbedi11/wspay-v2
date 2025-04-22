
using wspay_v2.Result;

namespace wspay_v2.Services
{
    public class FakePaymentService : IPaymentService
    {
        public Task<Result<PaymentDetailsDto>> GetPaymentDetailsDto(int? id)
        {
            return Task.FromResult(Result<PaymentDetailsDto>.Success(
                new PaymentDetailsDto()
                {
                    ShopID = "123",
                    TotalAmount = "100"

                }));
        }
    }
}
