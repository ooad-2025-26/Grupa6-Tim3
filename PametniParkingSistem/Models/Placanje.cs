using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class Placanje
    {
        public int Id { get; set; }
        public string ImeVlasnikaKartice { get; set; }
        public string BrojKarticeMaskiran { get; set; }
        public DateTime DatumPlacanja { get; set; }
        public double Iznos { get; set; }
        public StatusPlacanja StatusPlacanja { get; set; }
        public string TransakcijskiBroj { get; set; }

        public int RezervacijaId { get; set; }
        public Rezervacija Rezervacija { get; set; }
    }
}
