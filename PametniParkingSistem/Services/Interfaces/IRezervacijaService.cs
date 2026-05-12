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
    }
}