using BusnLogicVecozo;
using DALMSSQL;
using DALMSSQL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using VecozoWeb.Models;
using VecozoWep.Models;

namespace VecozoWep.Controllers
{
    public class MedewerkerController : Controller
    {
        private readonly MedewerkerContainer MC;
        private readonly VaardigheidContainer VC;
        private readonly IConfiguration config;

        public MedewerkerController(IConfiguration config)
        {
            this.config = config;
            MC = new(new MedewerkerDAL(this.config["db:ConnectionString"]));
            VC = new(new VaardigheidDAL(this.config["db:ConnectionString"]));
        }
        /// <summary>
        /// Geeft de medewerker pagina met zijn ratings 
        /// </summary>
        /// <returns>Return de indexview van de medewerker</returns>
        public IActionResult Index()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null && HttpContext.Session.GetInt32("IsAdmin") != null)
                {
                    if(Convert.ToInt32(HttpContext.Session.GetInt32("IsAdmin")) == 0)
                    {
                        int id = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                        Medewerker med = MC.FindById(id);
                        med.Ratings = VC.FindByMedewerker(med.UserID);
                        MedewerkerVM vm = new(med);
                        return View(vm);
                    }
                }
                return RedirectToAction("Index", "Login");
            }
            catch (TemporaryException ex)
            {
                return View("SqlErrorMessage");
            }
            catch (PermanentException ex)
            {
                return View("PermanentError");
            }
        }

        /// <summary>
        /// Toont een pagina waar medewerker een vaardigheid kan toevoegen met een rating
        /// </summary>
        /// <returns>Return een partial view waar de medewerker zijn vaardigheid en rating kan toevoegen</returns>
        [HttpGet]
        public IActionResult VaardigheidToevoegen()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    RatingVM rating = new();
                    rating.Vaardigheid = new();
                    return PartialView("_VaardigheidToevoegenParial", rating);
                }
                return RedirectToAction("Index", "Login");
            }
            catch (TemporaryException ex)
            {
                return View("SqlErrorMessage");
            }
            catch (PermanentException ex)
            {
                return View("PermanentError");
            }
        }


        /// <summary>
        /// De medewerker kan een vaardigheid kiezen en daaraan een rating koppelen
        /// </summary>
        /// <param name="r">De rating die wordt meegegeven</param>
        /// <returns>Return de indexpagina vd medewerker</returns>
        [HttpPost]
        public IActionResult VaardigheidToevoegen(RatingVM r)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? id = HttpContext.Session.GetInt32("UserId");
                    Medewerker med = MC.FindById(id.Value);
                    r.Vaardigheid = new VaardigheidVM(r.vaardigheidNaam);
                    Rating rating = r.GetRating();
                    VC.VoegVaardigheidToeAanMedewerker(med, rating);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", "Login");
            }
            catch (TemporaryException ex)
            {
                return View("SqlErrorMessage");
            }
            catch (PermanentException ex)
            {
                return View("PermanentError");
            }
        }

        /// <summary>
        /// Geeft een pagina waar de medewerker een vaardigheid kan verwijderen
        /// </summary>
        /// <param name="VaardigheidId">De vaardigheidsID die wordt meegegeven</param>
        /// <returns>Return een partial view waar de vaardigheid kan worden toegevoegd</returns>
        [HttpGet]
        public IActionResult VaardigheidVerwijderen(int VaardigheidId)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? Userid = HttpContext.Session.GetInt32("UserId");
                    Rating r = VC.FindRating(Userid.Value, VaardigheidId);
                    RatingVM rating = new(r);
                    return PartialView("_VaardigheidVerwijderenPartial", rating);
                }
                return RedirectToAction("Index", "Login");
            }
            catch (TemporaryException ex)
            {
                return View("SqlErrorMessage");
            }
            catch (PermanentException ex)
            {
                return View("PermanentError");
            }
        }

        /// <summary>
        /// De medewerker kan een vaardigheid naar keuze verwijderen
        /// </summary>
        /// <param name="r">De rating die wordt meegegeven</param>
        /// <param name="VaardigheidId">De vaardigheidsID die wordt meegegeven</param>
        /// <returns>Return de indexpagina vd medewerker</returns>
        [HttpPost]
        public IActionResult VaardigheidVerwijderen(RatingVM r, int VaardigheidId)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? id = HttpContext.Session.GetInt32("UserId");
                    Medewerker med = MC.FindById(id.Value);
                    VC.VerwijderVaarigheidVanMedewerker(med, VaardigheidId);
                    RatingVM rating = new();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", "Login");
            }
            catch (TemporaryException ex)
            {
                return View("SqlErrorMessage");
            }
            catch (PermanentException ex)
            {
                return View("PermanentError");
            }
        }

        /// <summary>
        /// Geeft een pagina waar de medewerker een vaardigheid kan updaten
        /// </summary>
        /// <param name="VaardigheidId">De vaardigheidsID die wordt meegegeven</param>
        /// <returns>Return een partial view waar de vaardigheid kan worden geupdatet</returns>
        [HttpGet]
        public IActionResult VaardigheidEdit(int VaardigheidId)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? Userid = HttpContext.Session.GetInt32("UserId");
                    Rating r = VC.FindRating(Userid.Value, VaardigheidId);
                    RatingVM rating = new(r);
                    return PartialView("_VaardigheidEditParial", rating);
                }
                return RedirectToAction("Index", "Login");
            }
            catch (TemporaryException ex)
            {
                return View("SqlErrorMessage");
            }
            catch (PermanentException ex)
            {
                return View("PermanentError");
            }
        }

        /// <summary>
        /// De medewerker kan een vaardigheid naar keuze updaten
        /// </summary>
        /// <param name="r">De rating die wordt meegegeven</param>
        /// <param name="VaardigheidId">De vaardigheidsID die wordt meegegeven</param>
        /// <returns>Return de indexpagina vd medewerker</returns>
        [HttpPost]
        public IActionResult VaardigheidEdit(RatingVM r)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? id = HttpContext.Session.GetInt32("UserId");
                    Medewerker med = MC.FindById(id.Value);
                    r.Vaardigheid = new VaardigheidVM(r.vaardigheidNaam, r.vaardigheidId);
                    Rating rating = r.GetRating();
                    VC.UpdateRating(med, rating);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", "Login");
            }
            catch (TemporaryException ex)
            {
                return View("SqlErrorMessage");
            }
            catch (PermanentException ex)
            {
                return View("PermanentError");
            }
        }
    }
}
