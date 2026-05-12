using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    public class ParkingMjestoController : Controller
    {
        private readonly IParkingMjestoService _service;

        public ParkingMjestoController(IParkingMjestoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);
            if (parkingMjesto == null) return NotFound();

            return View(parkingMjesto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Oznaka,Status,TipMjesta,Natkriveno,UdaljenostOdUlaza,CijenaPoSatu,ParkingZonaId")] ParkingMjesto parkingMjesto)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(parkingMjesto);
                return RedirectToAction(nameof(Index));
            }

            return View(parkingMjesto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);
            if (parkingMjesto == null) return NotFound();

            return View(parkingMjesto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Oznaka,Status,TipMjesta,Natkriveno,UdaljenostOdUlaza,CijenaPoSatu,ParkingZonaId")] ParkingMjesto parkingMjesto)
        {
            if (id != parkingMjesto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(parkingMjesto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ParkingMjestoExists(parkingMjesto.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(parkingMjesto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);
            if (parkingMjesto == null) return NotFound();

            return View(parkingMjesto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ParkingMjestoExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}