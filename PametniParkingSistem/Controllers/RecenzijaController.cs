using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    public class RecenzijaController : Controller
    {
        private readonly IRecenzijaService _service;

        public RecenzijaController(IRecenzijaService service)
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

            var recenzija = await _service.GetByIdAsync(id.Value);
            if (recenzija == null) return NotFound();

            return View(recenzija);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ocjena,Komentar,Datum,Obrisan,KorisnikId")] Recenzija recenzija)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(recenzija);
                return RedirectToAction(nameof(Index));
            }

            return View(recenzija);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var recenzija = await _service.GetByIdAsync(id.Value);
            if (recenzija == null) return NotFound();

            return View(recenzija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ocjena,Komentar,Datum,Obrisan,KorisnikId")] Recenzija recenzija)
        {
            if (id != recenzija.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(recenzija);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await RecenzijaExists(recenzija.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(recenzija);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var recenzija = await _service.GetByIdAsync(id.Value);
            if (recenzija == null) return NotFound();

            return View(recenzija);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RecenzijaExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}