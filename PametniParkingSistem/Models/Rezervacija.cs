using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class Rezervacija
    {
        public int Id { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public DateTime VrijemePocetka { get; set; }
        public DateTime VrijemeKraja { get; set; }
        public string RegistracijskeTablice { get; set; }
        public string KontaktTelefon { get; set; }
        public string EmailZaObavijest { get; set; }
        public double UkupnaCijena { get; set; }
        public StatusRezervacije StatusRezervacije { get; set; }

        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }

        public int ParkingMjestoId { get; set; }
        public ParkingMjesto ParkingMjesto { get; set; }

        public Placanje Placanje { get; set; }
    }
}
