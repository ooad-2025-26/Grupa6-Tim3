using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Controllers
{
    public class ParkingZonaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingZonaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ParkingZona
        public async Task<IActionResult> Index()
        {
            return View(await _context.ParkingZone.ToListAsync());
        }

        // GET: ParkingZona/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingZona = await _context.ParkingZone
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkingZona == null)
            {
                return NotFound();
            }

            return View(parkingZona);
        }

        // GET: ParkingZona/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ParkingZona/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,Lokacija,Opis,ProsjecnaOcjena")] ParkingZona parkingZona)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parkingZona);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(parkingZona);
        }

        // GET: ParkingZona/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingZona = await _context.ParkingZone.FindAsync(id);
            if (parkingZona == null)
            {
                return NotFound();
            }
            return View(parkingZona);
        }

        // POST: ParkingZona/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Lokacija,Opis,ProsjecnaOcjena")] ParkingZona parkingZona)
        {
            if (id != parkingZona.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parkingZona);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkingZonaExists(parkingZona.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(parkingZona);
        }

        // GET: ParkingZona/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingZona = await _context.ParkingZone
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkingZona == null)
            {
                return NotFound();
            }

            return View(parkingZona);
        }

        // POST: ParkingZona/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parkingZona = await _context.ParkingZone.FindAsync(id);
            if (parkingZona != null)
            {
                _context.ParkingZone.Remove(parkingZona);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParkingZonaExists(int id)
        {
            return _context.ParkingZone.Any(e => e.Id == id);
        }
    }
}
