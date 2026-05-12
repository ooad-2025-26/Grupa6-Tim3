using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Repositories
{
    public class ParkingMjestoRepository : IParkingMjestoRepository
    {
        private readonly ApplicationDbContext _context;

        public ParkingMjestoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParkingMjesto>> GetAllAsync()
        {
            return await _context.ParkingMjesta.ToListAsync();
        }

        public async Task<ParkingMjesto?> GetByIdAsync(int id)
        {
            return await _context.ParkingMjesta.FindAsync(id);
        }

        public async Task AddAsync(ParkingMjesto parkingMjesto)
        {
            await _context.ParkingMjesta.AddAsync(parkingMjesto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ParkingMjesto parkingMjesto)
        {
            _context.ParkingMjesta.Update(parkingMjesto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var parkingMjesto = await _context.ParkingMjesta.FindAsync(id);

            if (parkingMjesto != null)
            {
                _context.ParkingMjesta.Remove(parkingMjesto);
                await _context.SaveChangesAsync();
            }
        }
    }
}