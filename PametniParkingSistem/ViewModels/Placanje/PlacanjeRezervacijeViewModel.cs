using System.ComponentModel.DataAnnotations;

namespace PametniParkingSistem.ViewModels.Placanje
{
    public class PlacanjeRezervacijeViewModel
    {
        public double Iznos { get; set; }

        [Required]
        public string ImeVlasnikaKartice { get; set; } = string.Empty;

        [Required]
        public string BrojKartice { get; set; } = string.Empty;

        [Required]
        public string DatumIsteka { get; set; } = string.Empty;

        [Required]
        public string CVV { get; set; } = string.Empty;
    }
}