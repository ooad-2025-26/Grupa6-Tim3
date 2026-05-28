using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;

namespace PametniParkingSistem.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly IKorisnikService _service;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IRezervacijaService _rezervacijaService;

       
        public KorisnikController(
            IKorisnikService service,
            UserManager<Korisnik> userManager,
            IRezervacijaService rezervacijaService)
        {
            _service = service;
            _userManager = userManager;
            _rezervacijaService = rezervacijaService;
        }

  
        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Index()
        {
            var korisnici = await _service.GetAllAsync();

            if (User.IsInRole("Operater") && !User.IsInRole("Administrator"))
            {
                korisnici = korisnici
                    .Where(k => k.Uloga != Enums.Uloga.Administrator)
                    .ToList();
            }

            return View(korisnici);
        }

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _service.GetByIdAsync(id);
            if (korisnik == null) return NotFound();

            if (User.IsInRole("Operater") &&
                !User.IsInRole("Administrator") &&
                korisnik.Uloga == Enums.Uloga.Administrator)
            {
                return Forbid();
            }

            return View(korisnik);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]

    
        public async Task<IActionResult> Create([Bind("Id,Ime,Prezime,Email,PhoneNumber,DatumRegistracije,StatusNaloga,Uloga")] Korisnik korisnik)
        {
            if (ModelState.IsValid)
            {
                korisnik.UserName = korisnik.Email;
                await _service.AddAsync(korisnik);

                TempData["Success"] = "Korisnik uspješno kreiran.";
                return RedirectToAction(nameof(Index));
            }

            return View(korisnik);
        }

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _service.GetByIdAsync(id);
            if (korisnik == null) return NotFound();

            if (User.IsInRole("Operater") &&
                !User.IsInRole("Administrator") &&
                korisnik.Uloga == Enums.Uloga.Administrator)
            {
                return Forbid();
            }

            return View(korisnik);
        }

        [Authorize(Roles = "Administrator,Operater")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Enums.StatusNaloga statusNaloga, Enums.Uloga? uloga)
        {
            var korisnik = await _service.GetByIdAsync(id);
            if (korisnik == null) return NotFound();

            if (User.IsInRole("Operater") &&
                !User.IsInRole("Administrator") &&
                korisnik.Uloga == Enums.Uloga.Administrator)
            {
                return Forbid();
            }

            korisnik.StatusNaloga = statusNaloga;

            if (User.IsInRole("Administrator") && uloga.HasValue)
            {
                korisnik.Uloga = uloga.Value;

                var trenutneRole = await _userManager.GetRolesAsync(korisnik);
                await _userManager.RemoveFromRolesAsync(korisnik, trenutneRole);

                await _userManager.AddToRoleAsync(korisnik, uloga.Value.ToString());
            }

            await _service.UpdateAsync(korisnik);

            TempData["Success"] = "Podaci naloga su uspješno ažurirani.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _service.GetByIdAsync(id);
            if (korisnik == null) return NotFound();

            return View(korisnik);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _service.DeleteAsync(id);

            TempData["Success"] = "Korisnik obrisan.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Profil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var korisnik = await _service.GetByIdAsync(userId);
            if (korisnik == null) return NotFound();

            var rezervacije = await _rezervacijaService.GetByKorisnikIdAsync(userId);
            ViewBag.BrojRezervacija = rezervacije.Count;

            return View(korisnik);
        }

        [Authorize]
        public async Task<IActionResult> EditProfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var korisnik = await _service.GetByIdAsync(userId);
            if (korisnik == null) return NotFound();

            return View(korisnik);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfil(Korisnik model, IFormFile? profilnaSlika)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var korisnik = await _service.GetByIdAsync(userId);
            if (korisnik == null) return NotFound();

            korisnik.Ime = model.Ime;
            korisnik.Prezime = model.Prezime;
            korisnik.Email = model.Email;
            korisnik.UserName = model.Email;
            korisnik.PhoneNumber = model.PhoneNumber;

            if (profilnaSlika != null && profilnaSlika.Length > 0)
            {
                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "profili"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}_{profilnaSlika.FileName}";
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilnaSlika.CopyToAsync(stream);
                }

                korisnik.ProfilnaSlikaUrl = $"/uploads/profili/{fileName}";
            }

            await _service.UpdateAsync(korisnik);

            TempData["Success"] = "Profil uspješno ažuriran.";
            return RedirectToAction(nameof(Profil));
        }

        [Authorize]
        public IActionResult PromijeniLozinku()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromijeniLozinku(
            string trenutnaLozinka,
            string novaLozinka,
            string potvrdaLozinke)
        {
            if (novaLozinka != potvrdaLozinke)
            {
                ModelState.AddModelError("", "Lozinke se ne podudaraju.");
                return View();
            }

            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(
                korisnik,
                trenutnaLozinka,
                novaLozinka
            );

            if (result.Succeeded)
            {
                TempData["Success"] = "Lozinka uspješno promijenjena.";
                return RedirectToAction(nameof(Profil));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Blokiraj(string id)
        {
            var korisnik = await _service.GetByIdAsync(id);
            if (korisnik == null) return NotFound();

            if (User.IsInRole("Operater") &&
                !User.IsInRole("Administrator") &&
                korisnik.Uloga == Enums.Uloga.Administrator)
            {
                return Forbid();
            }

            korisnik.StatusNaloga = Enums.StatusNaloga.Blokiran;
            await _service.UpdateAsync(korisnik);

            TempData["Success"] = "Korisnik je blokiran.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator,Operater")]
        public async Task<IActionResult> Aktiviraj(string id)
        {
            var korisnik = await _service.GetByIdAsync(id);
            if (korisnik == null) return NotFound();

            if (User.IsInRole("Operater") &&
                !User.IsInRole("Administrator") &&
                korisnik.Uloga == Enums.Uloga.Administrator)
            {
                return Forbid();
            }

            korisnik.StatusNaloga = Enums.StatusNaloga.Aktivan;
            await _service.UpdateAsync(korisnik);

            TempData["Success"] = "Korisnik je aktiviran.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> KorisnikExists(string id)
        {
            return await _service.GetByIdAsync(id) != null;
        }
    }
}