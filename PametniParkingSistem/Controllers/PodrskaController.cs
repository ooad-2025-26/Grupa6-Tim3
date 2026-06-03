using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Controllers
{
    [Authorize]
    public class PodrskaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public PodrskaController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? status)
        {
            IQueryable<PodrskaZahtjev> zahtjevi = _context.PodrskaZahtjevi
                .Include(z => z.Korisnik)
                .OrderByDescending(z => z.DatumKreiranja);

            if (!User.IsInRole("Administrator") && !User.IsInRole("Operater"))
            {
                var korisnik = await _userManager.GetUserAsync(User);
                if (korisnik == null) return Unauthorized();

                zahtjevi = zahtjevi
                    .Where(z => z.KorisnikId == korisnik.Id)
                    .OrderByDescending(z => z.DatumKreiranja);
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<StatusPodrske>(status, out var parsedStatus))
            {
                zahtjevi = zahtjevi
                    .Where(z => z.Status == parsedStatus)
                    .OrderByDescending(z => z.DatumKreiranja);
            }

            ViewBag.Status = status;

            return View(await zahtjevi.ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new PodrskaZahtjev
            {
                Prioritet = PrioritetPodrske.Srednji,
                Kategorija = KategorijaPodrske.Ostalo
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Naslov,Opis,Kategorija,Prioritet")] PodrskaZahtjev zahtjev)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            if (!ModelState.IsValid)
                return View(zahtjev);

            zahtjev.KorisnikId = korisnik.Id;
            zahtjev.Status = StatusPodrske.Otvoren;
            zahtjev.DatumKreiranja = DateTime.Now;
            zahtjev.DatumOdgovora = null;
            zahtjev.Odgovor = null;

            _context.PodrskaZahtjevi.Add(zahtjev);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Zahtjev za podršku je uspješno poslan.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var zahtjev = await _context.PodrskaZahtjevi
                .Include(z => z.Korisnik)
                .FirstOrDefaultAsync(z => z.Id == id.Value);

            if (zahtjev == null) return NotFound();

            if (!User.IsInRole("Administrator") && !User.IsInRole("Operater"))
            {
                var korisnik = await _userManager.GetUserAsync(User);

                if (korisnik == null || zahtjev.KorisnikId != korisnik.Id)
                    return Forbid();
            }

            return View(zahtjev);
        }

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Odgovori(int? id)
        {
            if (id == null) return NotFound();

            var zahtjev = await _context.PodrskaZahtjevi
                .Include(z => z.Korisnik)
                .FirstOrDefaultAsync(z => z.Id == id.Value);

            if (zahtjev == null) return NotFound();

            return View(zahtjev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Odgovori(int id, string odgovor, StatusPodrske status)
        {
            var zahtjev = await _context.PodrskaZahtjevi.FindAsync(id);

            if (zahtjev == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(odgovor))
            {
                TempData["Error"] = "Odgovor ne može biti prazan.";
                return RedirectToAction(nameof(Odgovori), new { id });
            }

            zahtjev.Odgovor = odgovor.Trim();
            zahtjev.Status = status;
            zahtjev.DatumOdgovora = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Zahtjev je ažuriran. Novi status: {FormatirajStatus(status)}.";

            return RedirectToAction(nameof(Index), new { status = status.ToString() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Obrisi(int id)
        {
            var zahtjev = await _context.PodrskaZahtjevi.FindAsync(id);

            if (zahtjev == null)
                return NotFound();

            _context.PodrskaZahtjevi.Remove(zahtjev);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Zahtjev za podršku je uspješno obrisan.";

            return RedirectToAction(nameof(Index));
        }

        private static string FormatirajStatus(StatusPodrske status)
        {
            return status switch
            {
                StatusPodrske.Otvoren => "Otvoren",
                StatusPodrske.UObradi => "U obradi",
                StatusPodrske.Rijesen => "Riješen",
                StatusPodrske.Zatvoren => "Zatvoren",
                _ => status.ToString()
            };
        }
    }
}