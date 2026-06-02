using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.ViewModels.Account;

namespace PametniParkingSistem.Controllers
{
    // Kontroler za registraciju, prijavu, odjavu i kontrolu pristupa korisnika
    public class AccountController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;

        public AccountController(
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var korisnik = new Korisnik
            {
                Ime = model.Ime,
                Prezime = model.Prezime,
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = true,
                DatumRegistracije = DateTime.Now,
                StatusNaloga = StatusNaloga.Aktivan,
                Uloga = Uloga.RegistrovaniKorisnik
            };

            var result = await _userManager.CreateAsync(korisnik, model.Lozinka);

            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(
                    korisnik,
                    Uloga.RegistrovaniKorisnik.ToString()
                );

                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, PrevediIdentityGresku(error));
                    }

                    return View(model);
                }

                await _signInManager.SignInAsync(korisnik, isPersistent: false);

                return RedirectByRole(korisnik);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, PrevediIdentityGresku(error));
            }

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var korisnik = await _userManager.FindByEmailAsync(model.Email);

            if (korisnik == null)
            {
                ModelState.AddModelError(string.Empty, "Neispravan email ili lozinka.");
                return View(model);
            }

            if (korisnik.StatusNaloga != StatusNaloga.Aktivan)
            {
                ModelState.AddModelError(string.Empty, "Korisnički nalog nije aktivan.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Lozinka,
                model.ZapamtiMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return RedirectByRole(korisnik);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Korisnički nalog je privremeno zaključan.");
                return View(model);
            }

            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Prijava trenutno nije dozvoljena za ovaj nalog.");
                return View(model);
            }

            if (result.RequiresTwoFactor)
            {
                ModelState.AddModelError(string.Empty, "Za ovaj nalog je potrebna dodatna potvrda prijave.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Neispravan email ili lozinka.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectByRole(Korisnik korisnik)
        {
            if (korisnik.Uloga == Uloga.Administrator)
                return RedirectToAction("Index", "Korisnik");

            if (korisnik.Uloga == Uloga.Operater)
                return RedirectToAction("Index", "ParkingMjesto");

            return RedirectToAction("Index", "Home");
        }

        // Prevodi Identity validacijske greške na bosanski jezik
        private string PrevediIdentityGresku(IdentityError error)
        {
            return error.Code switch
            {
                "DefaultError" => "Došlo je do greške. Pokušajte ponovo.",
                "ConcurrencyFailure" => "Došlo je do konflikta pri spremanju podataka. Pokušajte ponovo.",
                "PasswordMismatch" => "Lozinka nije ispravna.",
                "InvalidToken" => "Token nije ispravan.",
                "LoginAlreadyAssociated" => "Ova prijava je već povezana sa drugim korisnikom.",
                "InvalidUserName" => "Korisničko ime nije ispravno.",
                "InvalidEmail" => "Email adresa nije ispravna.",
                "DuplicateUserName" => "Korisnik sa ovom email adresom već postoji.",
                "DuplicateEmail" => "Korisnik sa ovom email adresom već postoji.",
                "InvalidRoleName" => "Naziv uloge nije ispravan.",
                "DuplicateRoleName" => "Ova uloga već postoji.",
                "UserAlreadyHasPassword" => "Korisnik već ima postavljenu lozinku.",
                "UserLockoutNotEnabled" => "Zaključavanje korisničkog naloga nije omogućeno.",
                "UserAlreadyInRole" => "Korisnik već ima ovu ulogu.",
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