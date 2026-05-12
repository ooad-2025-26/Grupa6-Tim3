using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class KorisnikRepository : IKorisnikRepository
    {
        private readonly ApplicationDbContext _context;

        public KorisnikRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Korisnik>> GetAllAsync()
        {
            return await _context.Korisnici.ToListAsync();
        }

        public async Task<Korisnik?> GetByIdAsync(int id)
        {
            return await _context.Korisnici.FindAsync(id);
        }

        public async Task AddAsync(Korisnik korisnik)
        {
            await _context.Korisnici.AddAsync(korisnik);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Korisnik korisnik)
        {
            _context.Korisnici.Update(korisnik);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var korisnik = await _context.Korisnici.FindAsync(id);

            if (korisnik != null)
            {
                _context.Korisnici.Remove(korisnik);
                await _context.SaveChangesAsync();
            }
        }
    }
}