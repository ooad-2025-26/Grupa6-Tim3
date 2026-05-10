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
        public int KorisnikId { get; set; }
        public int ParkingMjestoId { get; set; }
    }
}