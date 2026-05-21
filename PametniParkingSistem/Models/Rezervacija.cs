using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class Rezervacija
    {
        public int Id { get; set; }

        public DateTime DatumKreiranja { get; set; }
        public DateTime VrijemePocetka { get; set; }
        public DateTime VrijemeKraja { get; set; }

        public string RegistracijskeTablice { get; set; } = string.Empty;
        public string KontaktTelefon { get; set; } = string.Empty;
        public string EmailZaObavijest { get; set; } = string.Empty;

        public double UkupnaCijena { get; set; }

        public StatusRezervacije StatusRezervacije { get; set; }

        // FK
        public string KorisnikId { get; set; } = string.Empty;
        public int ParkingMjestoId { get; set; }

        //Navigation properties
        public Korisnik? Korisnik { get; set; }
        public ParkingMjesto? ParkingMjesto { get; set; }
    }
}