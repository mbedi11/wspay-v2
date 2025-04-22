using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using wspay_v2.Models;

namespace wspay_v2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        /* public IActionResult Error()
         {
             //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
             var errorViewModel = new ErrorViewModel
             {
                 RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                 ShowRequestId = !string.IsNullOrEmpty(Activity.Current?.Id)
             };
             return View(errorViewModel);

         }*/
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var errorViewModel = new ErrorViewModel { RequestId = requestId };
            return View(errorViewModel);
        }
    }
}
