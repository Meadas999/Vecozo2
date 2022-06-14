using DALMSSQL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VecozoWep.Models;

namespace VecozoWep.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration config;

        public HomeController(ILogger<HomeController> logger,IConfiguration config)
        {
            _logger = logger;
            this.config = config;
        }


        /// <summary>
        /// Geeft de homepagina vd website
        /// </summary>
        /// <returns>Return de view voor de homepagina</returns>
        public IActionResult Index()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    return View();
                }
                return RedirectToAction("Index", "Login");
            }
            catch (TemporaryException ex)
            {
                return View("SqlErrorMessage");
            }
            catch (Exception ex)
            {
                return View("PermanentError");
            }
        }


        /// <summary>
        /// Geeft de privacy pagina vd website
        /// </summary>
        /// <returns>Return de view voor de privacypagina</returns>
        public IActionResult Privacy()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    return View();
                }
                return RedirectToAction("Index", "Login");
            }
            catch (TemporaryException ex)
            {
                return View("SqlErrorMessage");
            }
            catch (Exception ex)
            {
                return View("PermanentError");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}