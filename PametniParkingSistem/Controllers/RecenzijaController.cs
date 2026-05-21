using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    [Authorize]
    public class RecenzijaController : Controller
    {
        private readonly IRecenzijaService _service;
        private readonly IRezervacijaService _rezervacijaService;
        private readonly UserManager<Korisnik> _userManager;

        public RecenzijaController(
            IRecenzijaService service,
            IRezervacijaService rezervacijaService,
            UserManager<Korisnik> userManager)
        {
            _service = service;
            _rezervacijaService = rezervacijaService;
            _userManager = userManager;
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

        public async Task<IActionResult> Create(int rezervacijaId)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
                return Unauthorized();

            var rezervacija = await _rezervacijaService.GetByIdAsync(rezervacijaId);

            if (rezervacija == null)
                return NotFound();

            if (rezervacija.KorisnikId != korisnik.Id)
                return Forbid();

            if (rezervacija.StatusRezervacije != StatusRezervacije.Zavrsena)
            {
                TempData["Error"] = "Recenziju možete ostaviti samo za završenu rezervaciju.";
                return RedirectToAction("Details", "Rezervacija", new { id = rezervacijaId });
            }

            if (await _service.ExistsForRezervacijaAsync(rezervacijaId))
            {
                TempData["Error"] = "Za ovu rezervaciju već postoji recenzija.";
                return RedirectToAction("Details", "Rezervacija", new { id = rezervacijaId });
            }

            ViewBag.RezervacijaId = rezervacijaId;

            return View(new Recenzija
            {
                RezervacijaId = rezervacijaId,
                Ocjena = 5
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int rezervacijaId, Recenzija recenzija)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
                return Unauthorized();

            var rezervacija = await _rezervacijaService.GetByIdAsync(rezervacijaId);

            if (rezervacija == null)
                return NotFound();

            if (rezervacija.KorisnikId != korisnik.Id)
                return Forbid();

            if (rezervacija.StatusRezervacije != StatusRezervacije.Zavrsena)
            {
                TempData["Error"] = "Recenziju možete ostaviti samo za završenu rezervaciju.";
                return RedirectToAction("Details", "Rezervacija", new { id = rezervacijaId });
            }

            if (await _service.ExistsForRezervacijaAsync(rezervacijaId))
            {
                TempData["Error"] = "Za ovu rezervaciju već postoji recenzija.";
                return RedirectToAction("Details", "Rezervacija", new { id = rezervacijaId });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RezervacijaId = rezervacijaId;
                return View(recenzija);
            }

            recenzija.KorisnikId = korisnik.Id;
            recenzija.RezervacijaId = rezervacijaId;
            recenzija.Datum = DateTime.Now;
            recenzija.Obrisan = false;

            await _service.AddAsync(recenzija);

            TempData["Success"] = "Recenzija je uspješno dodana.";

            return RedirectToAction("Details", "Rezervacija", new { id = rezervacijaId });
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
            var recenzija = await _service.GetByIdAsync(id);

            if (recenzija == null)
                return NotFound();

            if (!User.IsInRole("Administrator") && !User.IsInRole("Operater"))
                return Forbid();

            await _service.DeleteAsync(id);

            TempData["Success"] = "Recenzija je obrisana.";

            return RedirectToAction(nameof(Index));
        }
    }
}