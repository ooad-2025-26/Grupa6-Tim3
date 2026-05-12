using PametniParkingSistem.Models;

namespace PametniParkingSistem.Services.Interfaces
{
    public interface IParkingMjestoService
    {
        Task<List<ParkingMjesto>> GetAllAsync();
        Task<ParkingMjesto?> GetByIdAsync(int id);
        Task AddAsync(ParkingMjesto parkingMjesto);
        Task UpdateAsync(ParkingMjesto parkingMjesto);
        Task DeleteAsync(int id);
    }
}