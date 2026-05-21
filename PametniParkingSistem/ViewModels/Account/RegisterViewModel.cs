using System.ComponentModel.DataAnnotations;
using PametniParkingSistem.Enums;

namespace PametniParkingSistem.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        public string Ime { get; set; } = string.Empty;

        [Required]
        public string Prezime { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Lozinka", ErrorMessage = "Lozinke se ne poklapaju.")]
        public string PotvrdiLozinku { get; set; } = string.Empty;

        public Uloga Uloga { get; set; } = Uloga.RegistrovaniKorisnik;
    }
}