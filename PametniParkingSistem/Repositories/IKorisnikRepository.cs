using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public interface IKorisnikRepository
    {
        Task<List<Korisnik>> GetAllAsync();
        Task<Korisnik?> GetByIdAsync(int id);
        Task AddAsync(Korisnik korisnik);
        Task UpdateAsync(Korisnik korisnik);
        Task DeleteAsync(int id);
    }
}