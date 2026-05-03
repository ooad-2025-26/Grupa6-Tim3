using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models

{
    public class Korisnik
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Lozinka { get; set; }
        public string Telefon { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public StatusNaloga StatusNaloga { get; set; }
        public Uloga Uloga { get; set; }

        public List<Rezervacija> Rezervacije { get; set; }
    }
}
