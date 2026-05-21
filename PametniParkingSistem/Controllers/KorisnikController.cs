using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;



namespace PametniParkingSistem.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class KorisnikController : Controller
    {
        private readonly IKorisnikService _service;

        public KorisnikController(IKorisnikService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAsync());
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _service.GetByIdAsync(id);

            if (korisnik == null) return NotFound();

            return View(korisnik);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime,Prezime,Email,PhoneNumber,DatumRegistracije,StatusNaloga,Uloga")] Korisnik korisnik)
        {
            if (ModelState.IsValid)
            {
                korisnik.UserName = korisnik.Email;

                await _service.AddAsync(korisnik);
                return RedirectToAction(nameof(Index));
            }

            return View(korisnik);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _service.GetByIdAsync(id);

            if (korisnik == null) return NotFound();

            return View(korisnik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Ime,Prezime,Email,PhoneNumber,DatumRegistracije,StatusNaloga,Uloga")] Korisnik korisnik)
        {
            if (id != korisnik.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    korisnik.UserName = korisnik.Email;

                    await _service.UpdateAsync(korisnik);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await KorisnikExists(korisnik.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(korisnik);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _service.GetByIdAsync(id);

            if (korisnik == null) return NotFound();

            return View(korisnik);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> KorisnikExists(string id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}