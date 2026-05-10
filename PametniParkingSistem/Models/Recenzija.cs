namespace PametniParkingSistem.Models
{
    public class Recenzija
    {
        public int Id { get; set; }

        public int Ocjena { get; set; }
        public string Komentar { get; set; } = string.Empty;

        public DateTime Datum { get; set; }
        public bool Obrisan { get; set; }

        // FK
        public int KorisnikId { get; set; }
    }
}