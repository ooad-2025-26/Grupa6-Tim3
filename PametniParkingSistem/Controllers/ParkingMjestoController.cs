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

            bool korisnikJePretrazivao =
                Request.Query.ContainsKey("zonaId") ||
                Request.Query.ContainsKey("tipMjesta") ||
                Request.Query.ContainsKey("natkriveno") ||
                Request.Query.ContainsKey("minCijena") ||
                Request.Query.ContainsKey("maxCijena") ||
                Request.Query.ContainsKey("maxUdaljenost");

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

            if (korisnikJePretrazivao)
            {
                var preporucenoMjesto = parkingMjesta
                    .Where(p => p.Status != StatusMjesta.VanFunkcije)
                    .OrderBy(p => p.UdaljenostOdUlaza)
                    .ThenBy(p => p.CijenaPoSatu)
                    .FirstOrDefault();

                ViewBag.PreporucenoMjestoId = preporucenoMjesto?.Id;
            }
            else
            {
                ViewBag.PreporucenoMjestoId = null;
            }

            return View(parkingMjesta);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var parkingMjesto = await _service.GetByIdAsync(id.Value);
            if (parkingMjesto == null) return NotFound();

            return View(parkingMjesto);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]

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