using BusnLogicVecozo;
using DALMSSQL;
using DALMSSQL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VecozoWeb.Models;
using VecozoWep.Models;

namespace VecozoWep.Controllers
{
    public class AdminController : Controller
    {
        private LeidingGevendeContainer LC = new(new LeidinggevendenDAL());
        private MedewerkerContainer MC = new(new MedewerkerDAL());
        private VaardigheidContainer VC = new(new VaardigheidDAL());
        private TeamContainer TC = new(new TeamDAL());

        /// <summary>
        /// Haalt alle medewerkers op van een bepaalde leidinggevende
        /// </summary>
        /// <returns>Return de leidinggevende met zijn medewerkers als er een ID is gevonden, anders return loginpagina</returns>
        public IActionResult Index()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int id = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
                    LeidinggevendenVM vm = new(LC.FindById(id));
                    vm.Medewerkers = MC.HaalAlleMedewerkersOp().Select(x => new MedewerkerVM(x)).ToList();

                    foreach (MedewerkerVM m in vm.Medewerkers)
                    {
                        m.Ratings = VC.FindByMedewerker(m.UserID).Select(x => new RatingVM(x)).ToList();
                        m.MijnTeam = new(TC.FindById(m.UserID));
                    }
                    return View(vm);
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
        /// Geeft een overzicht van een medewerker
        /// </summary>
        /// <param name="mwid">De medewerkersID die wordt meegegeven</param>
        /// <returns>Return een medewerker</returns>
        public IActionResult MedewerkerOverzicht(int mwid)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    HttpContext.Session.SetInt32("MwId", mwid);
                    MedewerkerVM vm = new(MC.FindById(mwid));
                    vm.Ratings = VC.FindByMedewerker(vm.UserID).Select(x => new RatingVM(x)).ToList();
                    //vm.MijnTeam = new(TC.FindById(vm.UserID));
                    return View(vm);
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
        /// Geeft een vaardigheid toevoeg pagina
        /// </summary>
        /// <returns>Return een partial view waar vaardigheden kunnen worden toegevoegd</returns>
        [HttpGet]
        public IActionResult VaardigheidToevoegen()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    RatingVM rating = new();
                    rating.Vaardigheid = new();
                    return PartialView("_VaardigheidTvgPartial", rating);
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
        /// Gebruiker kan een vaardigheid toevoegen
        /// </summary>
        /// <param name="r">De rating die wordt meegegeven</param>
        /// <returns>Return de index pagina</returns>
        [HttpPost]
        public IActionResult VaardigheidToevoegen(RatingVM r)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? id = HttpContext.Session.GetInt32("MwId");
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
        /// Geeft de vaardigheid verwijder pagina
        /// </summary>
        /// <param name="VaardigheidId">De vaardigheidID die wordt meegegeven</param>
        /// <returns>Return de partialview voor vaardigheid verwijderen</returns
        [HttpGet]
        public IActionResult VaardigheidVerwijderen(int VaardigheidId)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? Userid = HttpContext.Session.GetInt32("MwId");
                    Rating r = VC.FindRating(Userid.Value, VaardigheidId);
                    RatingVM rating = new(r);
                    return PartialView("_VaardigheidVwdPartial", rating);
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
        /// Hier kan de vaardigheid worden verwijderd
        /// </summary>
        /// <param name="r">De rating die wordt meegegeven</param>
        /// <param name="VaardigheidId">De vaardigheidID die wordt meegegeven</param>
        /// <returns>Return de index pagina</returns>
        [HttpPost]
        public IActionResult VaardigheidVerwijderen(RatingVM r, int VaardigheidId)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? id = HttpContext.Session.GetInt32("MwId");
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
        /// Geeft de vaardigheid updaten pagina
        /// </summary>
        /// <param name="VaardigheidId">De vaardigheidID die wordt meegegeven</param>
        /// <returns>Return de partial view om de vaardigheid te updaten</returns>
        [HttpGet]
        public IActionResult VaardigheidEdit(int VaardigheidId)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? Userid = HttpContext.Session.GetInt32("MwId");
                    Rating r = VC.FindRating(Userid.Value, VaardigheidId);
                    RatingVM rating = new(r);
                    return PartialView("_VaardigheidEditPartial", rating);
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
        /// Hier kan de vaardigheid worden aangepast
        /// </summary>
        /// <param name="r">De rating die wordt meegegeven</param>
        /// <returns>Return de index pagina</returns>
        [HttpPost]
        public IActionResult VaardigheidEdit(RatingVM r)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    int? id = HttpContext.Session.GetInt32("MwId");
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
        //[HttpGet]
        //public IActionResult VaardigheidToevoegen(int mwid)
        //{
        //    RatingVM rating = new();
        //    rating.Vaardigheid = new();
        //    return PartialView("_VaardigheidToevoegenParial", rating);
        //}
        //[HttpPost]
        //public IActionResult VaardigheidTvgBijMW(RatingVM r, int mwid)
        //{
        //    Medewerker med = MC.FindById(mwid);
        //    r.Vaardigheid = new VaardigheidVM(r.vaardigheidNaam);
        //    Rating rating = r.GetRating();
        //    VC.VoegVaardigheidToeAanMedewerker(med, rating);
        //    return RedirectToAction("MedewerkerOverzicht", mwid);
        //}

        //[HttpGet]
        //public IActionResult VaardigheidVerwijderen(int mwid, int VaardigheidId)
        //{
        //    Rating r = VC.FindRating(mwid, VaardigheidId);
        //    RatingVM rating = new(r);
        //    return PartialView("_VaardigheidVerwijderenPartial", rating);
        //}

        //[HttpPost]
        //public IActionResult VaardigheidVerwijderen(int mwid, RatingVM r, int VaardigheidId)
        //{
        //    Medewerker med = MC.FindById(mwid);
        //    VC.VerwijderVaarigheidVanMedewerker(med, VaardigheidId);
        //    RatingVM rating = new();
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public IActionResult VaardigheidEdit(int mwid, int VaardigheidId)
        //{
        //    Rating r = VC.FindRating(mwid, VaardigheidId);
        //    RatingVM rating = new(r);
        //    return PartialView("_VaardigheidEditParial", rating);
        //}

        //public IActionResult VaardigheidEdit(int mwid, RatingVM r)
        //{
        //    Medewerker med = MC.FindById(mwid);
        //    r.Vaardigheid = new VaardigheidVM(r.vaardigheidNaam, r.vaardigheidId);
        //    Rating rating = r.GetRating();
        //    VC.UpdateRating(med, rating);
        //    return RedirectToAction("Index");
        //}
    }
}
