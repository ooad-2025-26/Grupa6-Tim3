using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class KorisnikService : IKorisnikService
    {
        private readonly IKorisnikRepository _repository;

        public KorisnikService(IKorisnikRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Korisnik>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Korisnik?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Korisnik korisnik)
        {
            await _repository.AddAsync(korisnik);
        }

        public async Task UpdateAsync(Korisnik korisnik)
        {
            await _repository.UpdateAsync(korisnik);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}