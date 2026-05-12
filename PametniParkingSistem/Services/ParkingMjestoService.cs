using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class ParkingMjestoService : IParkingMjestoService
    {
        private readonly IParkingMjestoRepository _repository;

        public ParkingMjestoService(IParkingMjestoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ParkingMjesto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ParkingMjesto?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(ParkingMjesto parkingMjesto)
        {
            await _repository.AddAsync(parkingMjesto);
        }

        public async Task UpdateAsync(ParkingMjesto parkingMjesto)
        {
            await _repository.UpdateAsync(parkingMjesto);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}