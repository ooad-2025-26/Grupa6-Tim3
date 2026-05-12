using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    public class PlacanjeController : Controller
    {
        private readonly IPlacanjeService _service;

        public PlacanjeController(IPlacanjeService service)
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

            var placanje = await _service.GetByIdAsync(id.Value);

            if (placanje == null) return NotFound();

            return View(placanje);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImeVlasnikaKartice,BrojKarticeMaskiran,DatumPlacanja,Iznos,StatusPlacanja,TransakcijskiBroj,RezervacijaId")] Placanje placanje)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(placanje);
                return RedirectToAction(nameof(Index));
            }

            return View(placanje);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var placanje = await _service.GetByIdAsync(id.Value);

            if (placanje == null) return NotFound();

            return View(placanje);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImeVlasnikaKartice,BrojKarticeMaskiran,DatumPlacanja,Iznos,StatusPlacanja,TransakcijskiBroj,RezervacijaId")] Placanje placanje)
        {
            if (id != placanje.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(placanje);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PlacanjeExists(placanje.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(placanje);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var placanje = await _service.GetByIdAsync(id.Value);

            if (placanje == null) return NotFound();

            return View(placanje);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PlacanjeExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}