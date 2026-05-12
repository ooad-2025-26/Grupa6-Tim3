using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public interface IRecenzijaRepository
    {
        Task<List<Recenzija>> GetAllAsync();
        Task<Recenzija?> GetByIdAsync(int id);
        Task AddAsync(Recenzija recenzija);
        Task UpdateAsync(Recenzija recenzija);
        Task DeleteAsync(int id);
    }
}