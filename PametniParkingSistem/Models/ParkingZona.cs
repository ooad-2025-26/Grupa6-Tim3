namespace PametniParkingSistem.Models
{
    public class ParkingZona
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Lokacija { get; set; }
        public string Opis { get; set; }
        public double ProsjecnaOcjena { get; set; }

        public List<ParkingMjesto> ParkingMjesta { get; set; }
    }
}
