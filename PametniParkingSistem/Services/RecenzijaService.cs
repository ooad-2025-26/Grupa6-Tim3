using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class RecenzijaService : IRecenzijaService
    {
        private readonly IRecenzijaRepository _repository;

        public RecenzijaService(IRecenzijaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Recenzija>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Recenzija?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Recenzija?> GetByRezervacijaIdAsync(int rezervacijaId)
        {
            return await _repository.GetByRezervacijaIdAsync(rezervacijaId);
        }

        public async Task<bool> ExistsForRezervacijaAsync(int rezervacijaId)
        {
            return await _repository.ExistsForRezervacijaAsync(rezervacijaId);
        }

        public async Task AddAsync(Recenzija recenzija)
        {
            await _repository.AddAsync(recenzija);
        }

        public async Task UpdateAsync(Recenzija recenzija)
        {
            await _repository.UpdateAsync(recenzija);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}