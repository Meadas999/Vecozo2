using System.ComponentModel.DataAnnotations;

namespace VecozoWep.Models
{
    public class InlogVM
    {
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Voer een e-mailadres in met een juist aantal karakters.")]
        [Required(ErrorMessage = "Voer uw e-mailadres in.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Voer uw wachtwoord in.")]
        public string Password { get; set; }
        public bool Retry { get; set; }

        public InlogVM(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }

        public InlogVM()
        {

        }
    }
}
