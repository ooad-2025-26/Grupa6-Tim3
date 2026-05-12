using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IRecenzijaService
    {
        Task<List<Recenzija>> GetAllAsync();
        Task<Recenzija?> GetByIdAsync(int id);
        Task AddAsync(Recenzija recenzija);
        Task UpdateAsync(Recenzija recenzija);
        Task DeleteAsync(int id);
    }
}