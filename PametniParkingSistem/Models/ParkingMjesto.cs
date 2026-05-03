using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class ParkingMjesto
    {
        public int Id { get; set; }
        public string Oznaka { get; set; }
        public StatusMjesta Status { get; set; }
        public TipMjesta TipMjesta { get; set; }
        public bool Natkriveno { get; set; }
        public double UdaljenostOdUlaza { get; set; }
        public double CijenaPoSatu { get; set; }

        public int ParkingZonaId { get; set; }
        public ParkingZona ParkingZona { get; set; }

        public List<Rezervacija> Rezervacije { get; set; }
    }
}
