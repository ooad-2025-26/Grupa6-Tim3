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
            return await _context.Rezervacije.ToListAsync();
        }

        public async Task<Rezervacija?> GetByIdAsync(int id)
        {
            return await _context.Rezervacije.FindAsync(id);
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
    }
}