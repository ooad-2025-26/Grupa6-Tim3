using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class RecenzijaRepository : IRecenzijaRepository
    {
        private readonly ApplicationDbContext _context;

        public RecenzijaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recenzija>> GetAllAsync()
        {
            return await _context.Recenzije.ToListAsync();
        }

        public async Task<Recenzija?> GetByIdAsync(int id)
        {
            return await _context.Recenzije.FindAsync(id);
        }

        public async Task AddAsync(Recenzija recenzija)
        {
            await _context.Recenzije.AddAsync(recenzija);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Recenzija recenzija)
        {
            _context.Recenzije.Update(recenzija);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var recenzija = await _context.Recenzije.FindAsync(id);

            if (recenzija != null)
            {
                _context.Recenzije.Remove(recenzija);
                await _context.SaveChangesAsync();
            }
        }
    }
}