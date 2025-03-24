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
namespace wspay_v2.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Payment.ToListAsync());
        }

        public async Task<IActionResult> PaymentForm(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return View(payment);
        }
        //[Route("Payments")]
        [HttpGet("Return")]
        public async Task<IActionResult> Return()
        {
            // Čitanje samo approval koda
            string approvalCode = HttpContext.Request.Query["ApprovalCode"];

            // Koristite approval kod prema potrebi
            ViewBag.ApprovalCode = approvalCode;
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
            if (ModelState.IsValid)
            {
                payment.Signature = GenerateSignature(payment);
                payment.ReturnURL = $"{Request.Scheme}://{Request.Host}/Return";
                payment.ReturnErrorURL = $"{Request.Scheme}://{Request.Host}/returnError";
                payment.CancelURL = $"{Request.Scheme}://{Request.Host}/cancel";
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }
        
        /////////////////////////////////////////////////////////MALO NA BRZINU HACK SAMO ZA KREIRANJE MODELA DA GA MOZEMO POZVAT U FORMI
        private string GenerateSignature(Payment payment)
        {
            string secretKey = "074c6ca334fb4W";
            string rawData = payment.ShopID + secretKey + payment.ShoppingCartID + secretKey + payment.TotalAmount.ToString().Replace(",", "") + secretKey;
            return sha512(rawData);
        }

        private string sha512(string input)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        ////////////////////////////////////////////////////////////generate signature create function
        

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
                    payment.Signature = GenerateSignature(payment);
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
