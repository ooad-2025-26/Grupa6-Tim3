using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class Korisnik
    {
        public int Id { get; set; }

        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Lozinka { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;

        public DateTime DatumRegistracije { get; set; }

        public StatusNaloga StatusNaloga { get; set; }
        public Uloga Uloga { get; set; }
    }
}