using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class PlacanjeRepository : IPlacanjeRepository
    {
        private readonly ApplicationDbContext _context;

        public PlacanjeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Placanje>> GetAllAsync()
        {
            return await _context.Placanja.ToListAsync();
        }

        public async Task<Placanje?> GetByIdAsync(int id)
        {
            return await _context.Placanja.FindAsync(id);
        }

        public async Task AddAsync(Placanje placanje)
        {
            await _context.Placanja.AddAsync(placanje);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Placanje placanje)
        {
            _context.Placanja.Update(placanje);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var placanje = await _context.Placanja.FindAsync(id);

            if (placanje != null)
            {
                _context.Placanja.Remove(placanje);
                await _context.SaveChangesAsync();
            }
        }
    }
}