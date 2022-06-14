using BusnLogicVecozo;
using InterfaceLib;
using System.ComponentModel.DataAnnotations;
using VecozoWep.Models;

namespace VecozoWeb.Models
{
    public class RatingVM
    {
        [Required(ErrorMessage = "Voer uw score in.")]
        public int Score { get; set; }
        [StringLength(250, MinimumLength = 10, ErrorMessage = "Voer een beschrijving in met minimaal 10 karakaters(max. 150 karakters).")]
        [Required(ErrorMessage = "Voer uw Voornaam in.")]
        public string Beschrijving { get; set; }
        public DateTime LaatsteDatum { get; set; }
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Voer een vaardigheidsnaam in met minimaal 1 karakaters(max. 150 karakters).")]
        [Required(ErrorMessage = "Voer uw vaardigheidsnaam in.")]
        public string vaardigheidNaam { get; set; }
        public int vaardigheidId { get; set; }
        public VaardigheidVM Vaardigheid { get; set; }


        public RatingVM(int score, string Beschrijving, DateTime datum, VecozoWep.Models.VaardigheidVM vaardigheid)
        {
            this.Score = score;
            this.Beschrijving = Beschrijving;
            this.LaatsteDatum = datum;
            this.vaardigheidNaam = vaardigheid.Naam;
            this.vaardigheidId = vaardigheid.Id;
            this.Vaardigheid = vaardigheid;
        }


        public RatingVM(Rating rating)
        {
            this.Score = rating.Score;
            this.Beschrijving = rating.Beschrijving;
            this.LaatsteDatum = rating.LaatsteDatum;
            this.Vaardigheid = new VaardigheidVM(rating.Vaardigheid.Naam,rating.Vaardigheid.Id);
        }
        public Rating GetRating()
        {
            Rating rating = new Rating(Score, Beschrijving, LaatsteDatum, Vaardigheid.GetVaardigheid());
            return rating;
        }
        public RatingVM()
        {

        }
    }
}
