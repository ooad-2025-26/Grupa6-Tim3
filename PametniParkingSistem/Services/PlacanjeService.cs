using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class PlacanjeService : IPlacanjeService
    {
        private readonly IPlacanjeRepository _repository;

        public PlacanjeService(IPlacanjeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Placanje>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Placanje?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Placanje placanje)
        {
            await _repository.AddAsync(placanje);
        }

        public async Task UpdateAsync(Placanje placanje)
        {
            await _repository.UpdateAsync(placanje);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<bool> PostojiPlacanjeZaRezervacijuAsync(int rezervacijaId)
        {
            return await _repository.PostojiPlacanjeZaRezervacijuAsync(rezervacijaId);
        }

        public async Task<Placanje?> GetUspjesnoPlacanjeZaRezervacijuAsync(int rezervacijaId)
        {
            return await _repository.GetUspjesnoPlacanjeZaRezervacijuAsync(rezervacijaId);
        }
    }
}