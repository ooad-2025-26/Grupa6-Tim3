using PametniParkingSistem.Enums;

namespace PametniParkingSistem.Models
{
    public class EmailObavijest
    {
        public int Id { get; set; }
        public string Primalac { get; set; }
        public string Naslov { get; set; }
        public string Sadrzaj { get; set; }
        public DateTime DatumSlanja { get; set; }
        public TipObavijesti TipObavijesti { get; set; }
        public StatusEmaila StatusEmaila { get; set; }
    }
}
