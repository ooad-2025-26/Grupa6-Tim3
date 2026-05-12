using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IKorisnikService
    {
        Task<List<Korisnik>> GetAllAsync();
        Task<Korisnik?> GetByIdAsync(int id);
        Task AddAsync(Korisnik korisnik);
        Task UpdateAsync(Korisnik korisnik);
        Task DeleteAsync(int id);
    }
}