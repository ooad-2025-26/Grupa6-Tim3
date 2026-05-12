using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public interface IRezervacijaRepository
    {
        Task<List<Rezervacija>> GetAllAsync();
        Task<Rezervacija?> GetByIdAsync(int id);
        Task AddAsync(Rezervacija rezervacija);
        Task UpdateAsync(Rezervacija rezervacija);
        Task DeleteAsync(int id);
    }
}