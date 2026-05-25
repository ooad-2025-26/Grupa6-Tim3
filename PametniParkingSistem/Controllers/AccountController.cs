using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.ViewModels.Account;

namespace PametniParkingSistem.Controllers
{//upravljam registracijom, loginom, pristupom, logoutom
    public class AccountController : Controller
    {
        //UserManager
        private readonly UserManager<Korisnik> _userManager;
        //SignInManager
        private readonly SignInManager<Korisnik> _signInManager;

        public AccountController(
            //kontroler UserManager
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

            //kreiranje k, pass
            var result = await _userManager.CreateAsync(korisnik, model.Lozinka);

            if (result.Succeeded)
            {
                //dodjeljuje 
                await _userManager.AddToRoleAsync(korisnik, Uloga.RegistrovaniKorisnik.ToString());

                //isPersistent određuje da li će ostati ulogovan nakon što izađe iz browsera, ako je true, to je RememberMe, pa će ostati ulogovan
                await _signInManager.SignInAsync(korisnik, isPersistent: false);

                return RedirectByRole(korisnik);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

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

            if (korisnik == null || korisnik.StatusNaloga != StatusNaloga.Aktivan)
            {
                ModelState.AddModelError("", "Korisnički nalog ne postoji ili nije aktivan.");
                return View(model);
            }

            //SignIn login
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Lozinka,
                model.ZapamtiMe, //ako stisne korisnik ovo, onda će isPersistent biti =true i ostati će ulogovan
                lockoutOnFailure: false);

            if (result.Succeeded)
            {

                return RedirectByRole(korisnik);
            }

            ModelState.AddModelError("", "Neispravan email ili lozinka.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //SignIn logout
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
    }


}