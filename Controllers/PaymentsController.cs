using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using wspay_v2.Data;
using wspay_v2.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection.Metadata;
using wspay_v2.Services;
using System.Diagnostics;
namespace wspay_v2.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IPaymentService _paymentService;


        public PaymentsController(ApplicationDbContext context, ILogger<PaymentsController> logger, IPaymentService paymentService)
        {
            _context = context;
            _logger = logger;
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));

        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Payment.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> PaymentForm(int? id)
        {

            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            };
            

            // Validation

            if (id == null)
            {
                return NotFound();
            }

        
            // Buildling model


            if (_paymentService == null)
            {
                _logger.LogError("Payment service is not initialized.");
                return View("Error");
            }

            var payment = new Payment();

            try
            {
                //var result =
                //    await _paymentService.GetPaymentDetailsDto(id);
                var result = await _paymentService.GetPaymentDetailsDto(id);
                if (result.IsSuccess)
                {
                    return View(result.Value);
                }
                else
                {
                    _logger.LogError($"Payment details for id {id} not found");
                    return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = "Payment not found" });
                }
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, $"ArgumentNullException occurred while retrieving payment with id {id}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, $"ArgumentOutOfRangeException occurred while retrieving payment with id {id}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
            }
           

            //if (result.IsSuccess)
            //{
            //    Logging
            //    return View(result.Value);
            //}
            //else
            //{
            //    // Logging

            //    _logger.LogError($"Payment details for id {id} not found");
            //     return View(errorViewModel);
            //    return View("Error");
            //}
        }

        //[Route("Payments")]
        [HttpGet]
        [Route("Payments/Return")]
        public async Task<IActionResult> Return()
        {

            // Log all query parameters
            foreach (var key in HttpContext.Request.Query.Keys)
            {
                _logger.LogInformation($"Query param: {key} = {HttpContext.Request.Query[key]}");
            }

            string approvalCode = HttpContext.Request.Query["ApprovalCode"];
            ViewBag.accessCode = approvalCode;
            return View("Return");
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,ShopID,ShoppingCartID,Version,TotalAmount,Signature,ReturnURL,CancelURL,ReturnErrorURL")] Payment payment)
        {
            if (!ModelState.IsValid)
                return Problem("Model state is invalid");

            var paymentResponse = _paymentService.CreatePaymentAsync(payment);

            if (paymentResponse?.Result!=null && paymentResponse.Result.IsSuccess)
            {
                // Log success
                _logger.LogInformation($"Payment created successfully with ID: {payment.PaymentId}");
            }
            else
            {
                // Log failure
                _logger.LogError(
                    $"Failed to create payment: {paymentResponse.Result.Message}");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = paymentResponse.Result.Message });
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,ShopID,ShoppingCartID,Version,TotalAmount,Signature,ReturnURL,CancelURL,ReturnErrorURL")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    payment.GenerateSignature();
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            if (payment != null)
            {
                _context.Payment.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.PaymentId == id);
        }
    }
}
