using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PametniParkingSistem.Controllers
{
    public class ParkingMjestoController : Controller
    {
        private readonly IParkingMjestoService _service;

        public ParkingMjestoController(IParkingMjestoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(
            int? zonaId,
            TipMjesta? tipMjesta,
            bool? natkriveno,
            double? minCijena,
            double? maxCijena,
            double? maxUdaljenost)
        {
            var parkingMjesta = await _service.GetAllAsync();

            if (zonaId.HasValue)
                parkingMjesta = parkingMjesta.Where(p => p.ParkingZonaId == zonaId.Value).ToList();

            if (tipMjesta.HasValue)
                parkingMjesta = parkingMjesta.Where(p => p.TipMjesta == tipMjesta.Value).ToList();

            if (natkriveno.HasValue)
                parkingMjesta = parkingMjesta.Where(p => p.Natkriveno == natkriveno.Value).ToList();

            if (minCijena.HasValue)
                parkingMjesta = parkingMjesta.Where(p => p.CijenaPoSatu >= minCijena.Value).ToList();

            if (maxCijena.HasValue)
                parkingMjesta = parkingMjesta.Where(p => p.CijenaPoSatu <= maxCijena.Value).ToList();

            if (maxUdaljenost.HasValue)
                parkingMjesta = parkingMjesta.Where(p => p.UdaljenostOdUlaza <= maxUdaljenost.Value).ToList();

            return View(parkingMjesta);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);
            if (parkingMjesto == null) return NotFound();

            return View(parkingMjesto);
        }

        [Authorize(Roles = "Administrator,Operater")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator,Operater")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Oznaka,Status,TipMjesta,Natkriveno,UdaljenostOdUlaza,CijenaPoSatu,ParkingZonaId")] ParkingMjesto parkingMjesto)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(parkingMjesto);
                TempData["Success"] = "Parking mjesto je uspješno dodano.";
                return RedirectToAction(nameof(Index));
            }

            return View(parkingMjesto);
        }

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);
            if (parkingMjesto == null) return NotFound();

            return View(parkingMjesto);
        }

        [Authorize(Roles = "Administrator,Operater")]

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
                    TempData["Success"] = "Parking mjesto je uspješno ažurirano.";
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

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);
            if (parkingMjesto == null) return NotFound();

            return View(parkingMjesto);
        }

        [Authorize(Roles = "Administrator,Operater")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Parking mjesto je uspješno obrisano.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ParkingMjestoExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}