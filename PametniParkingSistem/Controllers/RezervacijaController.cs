using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    public class RezervacijaController : Controller
    {
        private readonly IRezervacijaService _service;

        public RezervacijaController(IRezervacijaService service)
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

            var rezervacija = await _service.GetByIdAsync(id.Value);
            if (rezervacija == null) return NotFound();

            return View(rezervacija);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatumKreiranja,VrijemePocetka,VrijemeKraja,RegistracijskeTablice,KontaktTelefon,EmailZaObavijest,UkupnaCijena,StatusRezervacije,KorisnikId,ParkingMjestoId")] Rezervacija rezervacija)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(rezervacija);
                return RedirectToAction(nameof(Index));
            }

            return View(rezervacija);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rezervacija = await _service.GetByIdAsync(id.Value);
            if (rezervacija == null) return NotFound();

            return View(rezervacija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumKreiranja,VrijemePocetka,VrijemeKraja,RegistracijskeTablice,KontaktTelefon,EmailZaObavijest,UkupnaCijena,StatusRezervacije,KorisnikId,ParkingMjestoId")] Rezervacija rezervacija)
        {
            if (id != rezervacija.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(rezervacija);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await RezervacijaExists(rezervacija.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(rezervacija);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rezervacija = await _service.GetByIdAsync(id.Value);
            if (rezervacija == null) return NotFound();

            return View(rezervacija);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RezervacijaExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}