using Microsoft.AspNetCore.Identity;
using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class Korisnik : IdentityUser
    {
        public string Ime { get; set; } = string.Empty;

        public string Prezime { get; set; } = string.Empty;

        public DateTime DatumRegistracije { get; set; }

        public StatusNaloga StatusNaloga { get; set; }

        public Uloga Uloga { get; set; }
    }
}