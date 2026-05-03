namespace PametniParkingSistem.Models
{
    public class Recenzija
    {
        public int Id { get; set; }
        public int Ocjena { get; set; }
        public string Komentar { get; set; }
        public DateTime Datum { get; set; }
        public bool Obrisan { get; set; }

        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
    }
}
