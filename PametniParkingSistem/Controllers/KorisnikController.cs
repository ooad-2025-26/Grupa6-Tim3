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
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly IRezervacijaService _rezervacijaService;

        public KorisnikController(
            IKorisnikService service,
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager,
            IRezervacijaService rezervacijaService)
        {
            _service = service;
            _userManager = userManager;
            _signInManager = signInManager;
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
            if (string.IsNullOrWhiteSpace(korisnik.Ime))
                ModelState.AddModelError("Ime", "Ime je obavezno.");

            if (string.IsNullOrWhiteSpace(korisnik.Prezime))
                ModelState.AddModelError("Prezime", "Prezime je obavezno.");

            if (string.IsNullOrWhiteSpace(korisnik.Email))
                ModelState.AddModelError("Email", "Email adresa je obavezna.");

            if (!ModelState.IsValid)
                return View(korisnik);

            korisnik.Ime = korisnik.Ime.Trim();
            korisnik.Prezime = korisnik.Prezime.Trim();
            korisnik.Email = korisnik.Email.Trim();
            korisnik.UserName = korisnik.Email;
            korisnik.DatumRegistracije = DateTime.Now;

            await _service.AddAsync(korisnik);

            TempData["Success"] = "Korisnik uspješno kreiran.";
            return RedirectToAction(nameof(Index));
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

                if (trenutneRole.Any())
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

            if (string.IsNullOrWhiteSpace(model.Ime))
                ModelState.AddModelError("Ime", "Ime je obavezno.");

            if (string.IsNullOrWhiteSpace(model.Prezime))
                ModelState.AddModelError("Prezime", "Prezime je obavezno.");

            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError("Email", "Email adresa je obavezna.");

            if (!ModelState.IsValid)
                return View(korisnik);

            korisnik.Ime = model.Ime.Trim();
            korisnik.Prezime = model.Prezime.Trim();
            korisnik.Email = model.Email.Trim();
            korisnik.UserName = model.Email.Trim();
            korisnik.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber)
                ? null
                : model.PhoneNumber.Trim();

            if (profilnaSlika != null && profilnaSlika.Length > 0)
            {
                var dozvoljeneEkstenzije = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ekstenzija = Path.GetExtension(profilnaSlika.FileName).ToLower();

                if (!dozvoljeneEkstenzije.Contains(ekstenzija))
                {
                    ModelState.AddModelError("ProfilnaSlikaUrl", "Dozvoljeni formati slike su JPG, JPEG, PNG i WEBP.");
                    return View(korisnik);
                }

                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "profili"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{ekstenzija}";
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilnaSlika.CopyToAsync(stream);
                }

                korisnik.ProfilnaSlikaUrl = $"/uploads/profili/{fileName}";
            }

            var result = await _userManager.UpdateAsync(korisnik);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(korisnik);

                TempData["Success"] = "Profil uspješno ažuriran.";
                return RedirectToAction(nameof(Profil));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, PrevediIdentityGresku(error));
            }

            return View(korisnik);
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
            string? trenutnaLozinka,
            string? novaLozinka,
            string? potvrdaLozinke)
        {
            if (string.IsNullOrWhiteSpace(trenutnaLozinka))
                ModelState.AddModelError("trenutnaLozinka", "Trenutna lozinka je obavezna.");

            if (string.IsNullOrWhiteSpace(novaLozinka))
                ModelState.AddModelError("novaLozinka", "Nova lozinka je obavezna.");

            if (string.IsNullOrWhiteSpace(potvrdaLozinke))
                ModelState.AddModelError("potvrdaLozinke", "Potvrda nove lozinke je obavezna.");

            if (!string.IsNullOrWhiteSpace(novaLozinka) &&
                !string.IsNullOrWhiteSpace(potvrdaLozinke) &&
                novaLozinka != potvrdaLozinke)
            {
                ModelState.AddModelError("potvrdaLozinke", "Lozinke se ne podudaraju.");
            }

            if (!ModelState.IsValid)
                return View();

            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(
                korisnik,
                trenutnaLozinka!,
                novaLozinka!
            );

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(korisnik);

                TempData["Success"] = "Lozinka uspješno promijenjena.";
                return RedirectToAction(nameof(Profil));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, PrevediIdentityGresku(error));
            }

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

        private string PrevediIdentityGresku(IdentityError error)
        {
            return error.Code switch
            {
                "DefaultError" => "Došlo je do greške. Pokušajte ponovo.",
                "ConcurrencyFailure" => "Došlo je do konflikta pri spremanju podataka. Pokušajte ponovo.",
                "PasswordMismatch" => "Trenutna lozinka nije ispravna.",
                "InvalidToken" => "Token nije ispravan.",
                "LoginAlreadyAssociated" => "Ova prijava je ve? povezana sa drugim korisnikom.",
                "InvalidUserName" => "Korisni?ko ime nije ispravno.",
                "InvalidEmail" => "Email adresa nije ispravna.",
                "DuplicateUserName" => "Korisnik sa ovom email adresom ve? postoji.",
                "DuplicateEmail" => "Korisnik sa ovom email adresom ve? postoji.",
                "InvalidRoleName" => "Naziv uloge nije ispravan.",
                "DuplicateRoleName" => "Ova uloga ve? postoji.",
                "UserAlreadyHasPassword" => "Korisnik ve? ima postavljenu lozinku.",
                "UserLockoutNotEnabled" => "Zaklju?avanje korisni?kog naloga nije omogu?eno.",
                "UserAlreadyInRole" => "Korisnik ve? ima ovu ulogu.",
                "UserNotInRole" => "Korisnik nema ovu ulogu.",
                "PasswordTooShort" => "Lozinka mora imati najmanje 6 karaktera.",
                "PasswordRequiresNonAlphanumeric" => "Lozinka mora sadržavati barem jedan specijalni znak.",
                "PasswordRequiresDigit" => "Lozinka mora sadržavati barem jedan broj.",
                "PasswordRequiresLower" => "Lozinka mora sadržavati barem jedno malo slovo.",
                "PasswordRequiresUpper" => "Lozinka mora sadržavati barem jedno veliko slovo.",
                _ => "Došlo je do greške. Provjerite unesene podatke i pokušajte ponovo."
            };
        }
    }
}