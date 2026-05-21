using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Services
{
    public class RezervacijaService : IRezervacijaService
    {
        private readonly IRezervacijaRepository _repository;

        public RezervacijaService(IRezervacijaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Rezervacija>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Rezervacija?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(Rezervacija rezervacija)
        {
            await _repository.AddAsync(rezervacija);
        }

        public async Task UpdateAsync(Rezervacija rezervacija)
        {
            await _repository.UpdateAsync(rezervacija);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<bool> ProvjeriDostupnostAsync(int parkingMjestoId, DateTime pocetak, DateTime kraj)
        {
            var postojiPreklapanje = await _repository.PostojiPreklapanjeTerminaAsync(parkingMjestoId, pocetak, kraj);
            return !postojiPreklapanje;
        }

        public double IzracunajCijenu(DateTime pocetak, DateTime kraj, double cijenaPoSatu)
        {
            var trajanjeUSatima = (kraj - pocetak).TotalHours;

            if (trajanjeUSatima <= 0)
                return 0;

            return Math.Ceiling(trajanjeUSatima) * cijenaPoSatu;
        }

        public async Task<List<Rezervacija>> GetByKorisnikIdAsync(string korisnikId)
        {
            return await _repository.GetByKorisnikIdAsync(korisnikId);
        }

        public async Task<List<Rezervacija>> GetIstekleAktivneRezervacijeAsync()
        {
            return await _repository.GetIstekleAktivneRezervacijeAsync();
        }
    }
}