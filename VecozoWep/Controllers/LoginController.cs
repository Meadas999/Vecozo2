using BusnLogicVecozo;
using DALMSSQL;
using DALMSSQL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VecozoWeb.Models;
using VecozoWep.Models;

namespace VecozoWep.Controllers
{
    public class LoginController : Controller
    {
        private MedewerkerContainer MC = new(new MedewerkerDAL());
        private LeidingGevendeContainer LC = new(new LeidinggevendenDAL());
        private TeamContainer TC = new(new TeamDAL());

        /// <summary>
        /// Geeft de login pagina
        /// </summary>
        /// <returns>Return de view van het inloggen</returns>
        public IActionResult Index()
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetInt32("IsAdmin", -1);
                InlogVM vm = new();
                return View(vm);
            }
            catch (TemporaryException)
            {
                return View("SqlErrorMessage");
            }
            catch (PermanentException)
            {
                return View("PermanentError");
            }
        }

        [HttpPost]
        /// <summary>
        /// Checkt of de gegevens die worden meegegeven bestaan
        /// </summary>
        /// <param name="vm">De inlogVM die meegegeven wordt</param>
        /// <returns>Return de indexpagina vd medewerker als een medewerker is gevonden, anders een admin indexpagina.
        /// Als niks wordt gevonden return opniew proberen</returns>
        public IActionResult LogIn(InlogVM vm)
        {
            try
            {
                LeidingGevende admin = LC.Inloggen(vm.Email, vm.Password);
                Medewerker user = MC.Inloggen(vm.Email, vm.Password);
                if (user != null)
                {
                    MedewerkerVM mvm = new(user);
                    HttpContext.Session.SetInt32("UserId", mvm.UserID);
                    HttpContext.Session.SetInt32("IsAdmin", 0);
                    return RedirectToAction("Index", "Medewerker");
                }
                else if (admin != null)
                {
                    LeidingGevende lg = LC.Inloggen(vm.Email, vm.Password);
                    HttpContext.Session.SetInt32("UserId", lg.UserID);
                    HttpContext.Session.SetInt32("IsAdmin", 1);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    InlogVM newvm = new();
                    ModelState.AddModelError("", "Email of wachtwoord is incorrect");
                    newvm.Retry = true;
                    return View("Index", newvm);
                }
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
        /// Geeft de account aanmaken pagina
        /// </summary>
        /// <returns>Return de view voor account aanmaken en neemt een gebruiker mee</returns>
        public IActionResult Register()
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    GebruikersVM vm = new();
                    vm.Leidinggevenden = LC.HaalAlleLeidinggevendeOp().Select(x => new LeidinggevendenVM(x)).ToList();
                    vm.Teams = TC.GetAll().Select(x => new TeamVM(x)).ToList();
                    return View(vm);
                }
                return View("Index");
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
        /// Checkt of de gebruiker een admin is en logt hem in , admin kan medewerker aanmaken
        /// </summary>
        /// <param name="vm">De gebruiker die meegegeven wordt</param>
        /// <returns>Return de indexpagina voor de admin.
        /// Return de indexpagina voor de medewerker</returns>
        [HttpPost]
        public IActionResult Register(GebruikersVM vm)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    Medewerker med = new(vm.Medewerker.Email, vm.Medewerker.Voornaam, vm.Medewerker.Achternaam, vm.Medewerker.Tussenvoegsel);
                    if (vm.Medewerker.IsAdmin)
                    {
                        LeidingGevende l = new(med.Email, med.Voornaam, med.Achternaam, 0, med.Tussenvoegsel);
                        LC.Create(l, vm.Medewerker.Wachtwoord);
                        LeidingGevende leid = LC.Inloggen(l.Email, vm.Medewerker.Wachtwoord);
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        if (vm.Leidinggevende.UserID > 0 && vm.Team.Id > 0)
                        {
                            med.MijnTeam = TC.FindById(vm.Team.Id);
                            MC.Create(med, vm.Medewerker.Wachtwoord);
                            LeidingGevende leid = LC.FindById(vm.Leidinggevende.UserID);
                            Medewerker medewerker = MC.Inloggen(med.Email, vm.Medewerker.Wachtwoord);
                            MC.KoppelMedewerkerAanLeidinggevenden(medewerker, leid);
                            return RedirectToAction("Index", "Admin");
                        }
                        return RedirectToAction("Register", "Login");
                    }
                }
                return View("Index");
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
