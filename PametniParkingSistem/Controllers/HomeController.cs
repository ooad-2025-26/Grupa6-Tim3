using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using System.Diagnostics;

namespace PametniParkingSistem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRecenzijaService _recenzijaService;
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IParkingMjestoService _parkingMjestoService;
        private readonly IKorisnikService _korisnikService;
        private readonly IPlacanjeService _placanjeService;
        private readonly UserManager<Korisnik> _userManager;

        public HomeController(
            IRecenzijaService recenzijaService,
            IRezervacijaService rezervacijaService,
            IParkingMjestoService parkingMjestoService,
            IKorisnikService korisnikService,
            IPlacanjeService placanjeService,
            UserManager<Korisnik> userManager)
        {
            _recenzijaService = recenzijaService;
            _rezervacijaService = rezervacijaService;
            _parkingMjestoService = parkingMjestoService;
            _korisnikService = korisnikService;
            _placanjeService = placanjeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var recenzije = await _recenzijaService.GetAllAsync();
            var rezervacije = await _rezervacijaService.GetAllAsync();
            var parkingMjesta = await _parkingMjestoService.GetAllAsync();
            var korisnici = await _korisnikService.GetAllAsync();
            var placanja = await _placanjeService.GetAllAsync();

            ViewBag.ProsjecnaOcjena = recenzije.Any() ? recenzije.Average(r => r.Ocjena) : 0;
            ViewBag.BrojRecenzija = recenzije.Count;
            ViewBag.ZadnjeRecenzije = recenzije.OrderByDescending(r => r.Datum).Take(3).ToList();

            ViewBag.BrojRezervacija = rezervacije.Count;
            ViewBag.AktivneRezervacije = rezervacije.Count(r => r.StatusRezervacije == StatusRezervacije.Aktivna);
            ViewBag.ZavrseneRezervacije = rezervacije.Count(r => r.StatusRezervacije == StatusRezervacije.Zavrsena);
            ViewBag.OtkazaneRezervacije = rezervacije.Count(r => r.StatusRezervacije == StatusRezervacije.Otkazana);

            ViewBag.BrojParkingMjesta = parkingMjesta.Count;
            ViewBag.BrojKorisnika = korisnici.Count;
            ViewBag.UkupnaZarada = placanja
                .Where(p => p.StatusPlacanja == StatusPlacanja.Uspjesno)
                .Sum(p => p.Iznos);

            ViewBag.ZadnjeRezervacije = rezervacije
    .OrderByDescending(r => r.DatumKreiranja)
    .Take(5)
    .ToList();

            ViewBag.NoviKorisnici = korisnici
                .OrderByDescending(k => k.DatumRegistracije)
                .Take(5)
                .ToList();

            ViewBag.ZadnjaPlacanja = placanja
                .OrderByDescending(p => p.DatumPlacanja)
                .Take(5)
                .ToList();

            ViewBag.MjestaVanFunkcije = parkingMjesta
                .Where(p => p.Status == StatusMjesta.VanFunkcije)
                .Take(5)
                .ToList();

            if (User.Identity != null && User.Identity.IsAuthenticated && !User.IsInRole("Administrator") && !User.IsInRole("Operater"))
            {
                var korisnik = await _userManager.GetUserAsync(User);

                if (korisnik != null)
                {
                    var mojeRezervacije = await _rezervacijaService.GetByKorisnikIdAsync(korisnik.Id);

                    ViewBag.MojeRezervacijeCount = mojeRezervacije.Count;
                    ViewBag.MojaAktivnaRezervacija = mojeRezervacije
                        .Where(r => r.StatusRezervacije == StatusRezervacije.Aktivna)
                        .OrderBy(r => r.VrijemePocetka)
                        .FirstOrDefault();
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}