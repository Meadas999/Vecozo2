using BusnLogicVecozo;
using System.ComponentModel.DataAnnotations;

namespace VecozoWep.Models
{
    public class LeidinggevendenVM
    {
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Voer een voornaam in met een juist aantal karakters(Min. 1).")]
        [Required(ErrorMessage = "Voer uw Voornaam in.")]
        public string Voornaam { get; set; }
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Voer een voornaam in met een juist aantal karakters(Min. 1).")]
        public string? Tussenvoegsel { get; set; }
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Voer een achternaam in met een juist aantal karakters(Min. 1).")]
        [Required(ErrorMessage = "Voer uw achternaam in.")]
        public string Achternaam { get;  set; }
        public int UserID { get; set; }
        public List<MedewerkerVM> Medewerkers { get; set; }
        public bool IsAdmin { get; set; }

        public LeidinggevendenVM(string voornaam, string? tussenvoegsel, string achternaam, int userID)
        {
            Voornaam = voornaam;
            Tussenvoegsel = tussenvoegsel;
            Achternaam = achternaam;
            UserID = userID;
        }

        public LeidinggevendenVM(string voornaam, string? tussenvoegsel, string achternaam, bool isAdmin)
        {
            Voornaam = voornaam;
            Tussenvoegsel = tussenvoegsel;
            Achternaam = achternaam;
            IsAdmin = isAdmin;
        }
        
        public LeidinggevendenVM(LeidingGevende admin)
        {
            Voornaam = admin.Voornaam;
            Tussenvoegsel = admin.Tussenvoegsel;
            Achternaam = admin.Achternaam;
            UserID = admin.UserID;
        }

        //public LeidinggevendenVM(Medewerker medewerker)
        //{
        //    Voornaam = admin.Voornaam;
        //    Tussenvoegsel = admin.Tussenvoegsel;
        //    Achternaam = admin.Achternaam;
        //    UserID = admin.UserID;
        //}
        public LeidinggevendenVM()
        {

        }
        
        public override string ToString()
        {
            return $"{Voornaam} {Tussenvoegsel} {Achternaam}";
        }
    }
}

