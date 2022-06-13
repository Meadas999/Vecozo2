using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VecozoWep.Models;

namespace VecozoWep.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Geeft de homepagina vd website
        /// </summary>
        /// <returns>Return de view voor de homepagina</returns>
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Geeft de privacy pagina vd website
        /// </summary>
        /// <returns>Return de view voor de privacypagina</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}