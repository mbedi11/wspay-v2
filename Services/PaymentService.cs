
using Azure.Core;
using System.Diagnostics;
using wspay_v2.Data;
using wspay_v2.Models;
using wspay_v2.Result;

namespace wspay_v2.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(ApplicationDbContext context, ILogger<PaymentService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<Payment>> CreatePaymentAsync(Payment paymentDto)
        {
            Payment? payment = null;
            try
            {
                payment = Payment.Create(
                    paymentDto.ShopID,
                    paymentDto.ShoppingCartID,
                    paymentDto.Version,
                    paymentDto.TotalAmount,
                    paymentDto.ReturnURL,
                    paymentDto.CancelURL,
                    paymentDto.ReturnErrorURL
                    );
                _context.Add(payment);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException e)
            {
                var errorMessage = $"Argument null exception: {e.Message}";
                _logger.LogError(e, errorMessage);
                return Result<Payment>.Failure(errorMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = "Error occurred while creating payment";
                _logger.LogError(ex, errorMessage);
                throw new Exception(errorMessage, ex);
            }

            return Result<Payment>.Success(payment);
        }

        public async Task<Result<PaymentDetailsDto>> GetPaymentDetailsDto(int? id)
        {
            // Validation
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            bool flag = false;
            _context.Payment.ToList().ForEach(x =>
            {
                if (x.PaymentId == id)
                {
                    flag = true;
                }
            });

            if (!flag)
            {
                //return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = "Payment doesn't exist in DB" });
                throw new ArgumentOutOfRangeException(nameof(id), "Id doesn't exist in DB");
            }

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0");
            }

            if (id > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be less than 1000");
            }


            // Building model
            var payment = new Payment();
            try
            {
                // var payment =
                await _context.Payment.FindAsync(id);
                if (payment == null)
                {
                    _logger.LogError($"Payment with id {id} not found");
                    return Result<PaymentDetailsDto>.Failure();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while retrieving payment with id {id}");
                return Result<PaymentDetailsDto>.Failure();
            }
            _logger.LogInformation($"Payment with id {id} found");

            return Result<PaymentDetailsDto>.Success(
                new PaymentDetailsDto()
                {
                    ShopID = payment.ShopID,
                    TotalAmount = payment.TotalAmount,
                    ReturnURL = payment.ReturnURL,
                    Version = payment.Version,
                    ShoppingCartID = payment.ShoppingCartID,
                    Signature = payment.Signature,
                    CancelURL = payment.CancelURL,
                    ReturnErrorURL = payment.ReturnErrorURL

                });

        }
    }
}
