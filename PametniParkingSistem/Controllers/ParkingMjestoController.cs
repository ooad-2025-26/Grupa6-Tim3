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
    public class ParkingMjestoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParkingMjestoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ParkingMjesto
        public async Task<IActionResult> Index()
        {
            return View(await _context.ParkingMjesta.ToListAsync());
        }

        // GET: ParkingMjesto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingMjesto = await _context.ParkingMjesta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkingMjesto == null)
            {
                return NotFound();
            }

            return View(parkingMjesto);
        }

        // GET: ParkingMjesto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ParkingMjesto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Oznaka,Status,TipMjesta,Natkriveno,UdaljenostOdUlaza,CijenaPoSatu,ParkingZonaId")] ParkingMjesto parkingMjesto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parkingMjesto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(parkingMjesto);
        }

        // GET: ParkingMjesto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingMjesto = await _context.ParkingMjesta.FindAsync(id);
            if (parkingMjesto == null)
            {
                return NotFound();
            }
            return View(parkingMjesto);
        }

        // POST: ParkingMjesto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Oznaka,Status,TipMjesta,Natkriveno,UdaljenostOdUlaza,CijenaPoSatu,ParkingZonaId")] ParkingMjesto parkingMjesto)
        {
            if (id != parkingMjesto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parkingMjesto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkingMjestoExists(parkingMjesto.Id))
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
            return View(parkingMjesto);
        }

        // GET: ParkingMjesto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkingMjesto = await _context.ParkingMjesta
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkingMjesto == null)
            {
                return NotFound();
            }

            return View(parkingMjesto);
        }

        // POST: ParkingMjesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parkingMjesto = await _context.ParkingMjesta.FindAsync(id);
            if (parkingMjesto != null)
            {
                _context.ParkingMjesta.Remove(parkingMjesto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParkingMjestoExists(int id)
        {
            return _context.ParkingMjesta.Any(e => e.Id == id);
        }
    }
}
