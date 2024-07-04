using DKAC.App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DKAC.App.Controllers
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
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View();
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
