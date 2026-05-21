using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class RezervacijaRepository : IRezervacijaRepository
    {
        private readonly ApplicationDbContext _context;

        public RezervacijaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Rezervacija>> GetAllAsync()
        {
            return await _context.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.ParkingMjesto)
                .ToListAsync();
        }

        public async Task<List<Rezervacija>> GetByKorisnikIdAsync(string korisnikId)
        {
            return await _context.Rezervacije
                .Include(r => r.ParkingMjesto)
                .Where(r => r.KorisnikId == korisnikId)
                .ToListAsync();
        }

        public async Task<Rezervacija?> GetByIdAsync(int id)
        {
            return await _context.Rezervacije
                .Include(r => r.Korisnik)
                .Include(r => r.ParkingMjesto)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Rezervacija rezervacija)
        {
            await _context.Rezervacije.AddAsync(rezervacija);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rezervacija rezervacija)
        {
            _context.Rezervacije.Update(rezervacija);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rezervacija = await _context.Rezervacije.FindAsync(id);

            if (rezervacija != null)
            {
                _context.Rezervacije.Remove(rezervacija);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> PostojiPreklapanjeTerminaAsync(int parkingMjestoId, DateTime pocetak, DateTime kraj)
        {
            return await _context.Rezervacije.AnyAsync(r =>
                r.ParkingMjestoId == parkingMjestoId &&
                r.StatusRezervacije != PametniParkingSistem.Enums.StatusRezervacije.Otkazana &&
                pocetak < r.VrijemeKraja &&
                kraj > r.VrijemePocetka);
        }

        public async Task<List<Rezervacija>> GetIstekleAktivneRezervacijeAsync()
        {
            return await _context.Rezervacije
                .Include(r => r.ParkingMjesto)
                .Where(r =>
                    r.VrijemeKraja < DateTime.Now &&
                    r.StatusRezervacije != PametniParkingSistem.Enums.StatusRezervacije.Otkazana &&
                    r.StatusRezervacije != PametniParkingSistem.Enums.StatusRezervacije.Zavrsena)
                .ToListAsync();
        }

    }
}