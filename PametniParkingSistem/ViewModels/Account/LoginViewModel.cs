using System.ComponentModel.DataAnnotations;

namespace PametniParkingSistem.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;

        public bool ZapamtiMe { get; set; }
    }
}