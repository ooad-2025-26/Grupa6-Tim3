using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IRezervacijaService
    {
        Task<List<Rezervacija>> GetAllAsync();
        Task<Rezervacija?> GetByIdAsync(int id);
        Task AddAsync(Rezervacija rezervacija);
        Task UpdateAsync(Rezervacija rezervacija);
        Task DeleteAsync(int id);
        Task<bool> ProvjeriDostupnostAsync(int parkingMjestoId, DateTime pocetak, DateTime kraj);
        double IzracunajCijenu(DateTime pocetak, DateTime kraj, double cijenaPoSatu);
        Task<List<Rezervacija>> GetByKorisnikIdAsync(string korisnikId);
        Task<List<Rezervacija>> GetIstekleAktivneRezervacijeAsync();
    }
}