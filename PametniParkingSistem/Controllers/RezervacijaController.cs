using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using System.Text.Json;

namespace PametniParkingSistem.Controllers
{
    [Authorize]
    public class RezervacijaController : Controller
    {
        private readonly IRezervacijaService _service;
        private readonly IParkingMjestoService _parkingMjestoService;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IPlacanjeService _placanjeService;
        private readonly IEmailSenderService _emailSender;

        public RezervacijaController(
            IRezervacijaService service,
            IParkingMjestoService parkingMjestoService,
            UserManager<Korisnik> userManager,
            IPlacanjeService placanjeService,
            IEmailSenderService emailSender)
        {
            _service = service;
            _parkingMjestoService = parkingMjestoService;
            _userManager = userManager;
            _placanjeService = placanjeService;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            await AzurirajIstekleRezervacijeAsync();

            if (User.IsInRole("Administrator") || User.IsInRole("Operater"))
                return View(await _service.GetAllAsync());

            return RedirectToAction(nameof(Moje));
        }

        public async Task<IActionResult> Moje(string? status)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("Operater"))
                return RedirectToAction(nameof(Index));

            await AzurirajIstekleRezervacijeAsync();

            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            var rezervacije = await _service.GetByKorisnikIdAsync(korisnik.Id);

            rezervacije = rezervacije
                .Where(r => r.StatusRezervacije != StatusRezervacije.Zavrsena &&
                            r.StatusRezervacije != StatusRezervacije.Otkazana)
                .ToList();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<StatusRezervacije>(status, out var parsedStatus))
            {
                rezervacije = rezervacije
                    .Where(r => r.StatusRezervacije == parsedStatus)
                    .ToList();
            }

            ViewBag.Status = status;
            return View(rezervacije);
        }

        public async Task<IActionResult> Historija(string? status)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("Operater"))
                return RedirectToAction(nameof(Index));

            await AzurirajIstekleRezervacijeAsync();

            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            var rezervacije = await _service.GetByKorisnikIdAsync(korisnik.Id);

            rezervacije = rezervacije
                .Where(r => r.StatusRezervacije == StatusRezervacije.Zavrsena ||
                            r.StatusRezervacije == StatusRezervacije.Otkazana)
                .ToList();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<StatusRezervacije>(status, out var parsedStatus))
            {
                rezervacije = rezervacije
                    .Where(r => r.StatusRezervacije == parsedStatus)
                    .ToList();
            }

            ViewBag.Status = status;
            return View(rezervacije);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var rezervacija = await _service.GetByIdAsync(id.Value);
            if (rezervacija == null) return NotFound();

            if (!User.IsInRole("Administrator") && !User.IsInRole("Operater"))
            {
                var korisnik = await _userManager.GetUserAsync(User);

                if (korisnik == null || rezervacija.KorisnikId != korisnik.Id)
                    return Forbid();
            }

            return View(rezervacija);
        }

        public async Task<IActionResult> Create(int parkingMjestoId)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("Operater"))
            {
                TempData["Error"] = "Administrator i operater ne kreiraju lične rezervacije.";
                return RedirectToAction(nameof(Index));
            }

            var parkingMjesto = await _parkingMjestoService.GetByIdAsync(parkingMjestoId);
            if (parkingMjesto == null) return NotFound();

            var rezervacija = new Rezervacija
            {
                ParkingMjestoId = parkingMjestoId,
                DatumKreiranja = DateTime.Now,
                VrijemePocetka = DateTime.Now,
                VrijemeKraja = DateTime.Now.AddHours(1),
                EmailZaObavijest = User.Identity?.Name ?? ""
            };

            ViewBag.ParkingMjesto = parkingMjesto;
            return View(rezervacija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VrijemePocetka,VrijemeKraja,RegistracijskeTablice,KontaktTelefon,EmailZaObavijest,ParkingMjestoId")] Rezervacija rezervacija)
        {
            if (User.IsInRole("Administrator") || User.IsInRole("Operater"))
            {
                TempData["Error"] = "Administrator i operater ne kreiraju lične rezervacije.";
                return RedirectToAction(nameof(Index));
            }

            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            var parkingMjesto = await _parkingMjestoService.GetByIdAsync(rezervacija.ParkingMjestoId);
            if (parkingMjesto == null) return NotFound();

            if (rezervacija.VrijemeKraja <= rezervacija.VrijemePocetka)
            {
                ModelState.AddModelError("", "Vrijeme kraja mora biti nakon vremena početka.");
                ViewBag.ParkingMjesto = parkingMjesto;
                return View(rezervacija);
            }

            var dostupno = await _service.ProvjeriDostupnostAsync(
                rezervacija.ParkingMjestoId,
                rezervacija.VrijemePocetka,
                rezervacija.VrijemeKraja);

            if (!dostupno)
            {
                TempData["Error"] = "Parking mjesto je zauzeto za odabrani termin.";
                ModelState.AddModelError("", "Parking mjesto nije dostupno u odabranom terminu.");
                ViewBag.ParkingMjesto = parkingMjesto;
                return View(rezervacija);
            }

            rezervacija.KorisnikId = korisnik.Id;
            rezervacija.DatumKreiranja = DateTime.Now;
            rezervacija.StatusRezervacije = StatusRezervacije.Kreirana;
            rezervacija.UkupnaCijena = _service.IzracunajCijenu(
                rezervacija.VrijemePocetka,
                rezervacija.VrijemeKraja,
                parkingMjesto.CijenaPoSatu);

            TempData["RezervacijaZaPlacanje"] = JsonSerializer.Serialize(rezervacija);

            return RedirectToAction("Create", "Placanje");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rezervacija = await _service.GetByIdAsync(id.Value);
            if (rezervacija == null) return NotFound();

            if (!User.IsInRole("Administrator") && !User.IsInRole("Operater"))
            {
                var korisnik = await _userManager.GetUserAsync(User);

                if (korisnik == null || rezervacija.KorisnikId != korisnik.Id)
                    return Forbid();
            }

            return View(rezervacija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumKreiranja,VrijemePocetka,VrijemeKraja,RegistracijskeTablice,KontaktTelefon,EmailZaObavijest,UkupnaCijena,StatusRezervacije,KorisnikId,ParkingMjestoId")] Rezervacija rezervacija)
        {
            if (id != rezervacija.Id) return NotFound();

            var postojecaRezervacija = await _service.GetByIdAsync(id);
            if (postojecaRezervacija == null) return NotFound();

            if (!User.IsInRole("Administrator") && !User.IsInRole("Operater"))
            {
                var korisnik = await _userManager.GetUserAsync(User);

                if (korisnik == null || postojecaRezervacija.KorisnikId != korisnik.Id)
                    return Forbid();
            }

            var parkingMjesto = await _parkingMjestoService.GetByIdAsync(postojecaRezervacija.ParkingMjestoId);
            if (parkingMjesto == null) return NotFound();

            if (rezervacija.VrijemeKraja <= rezervacija.VrijemePocetka)
            {
                ModelState.AddModelError("", "Vrijeme kraja mora biti nakon vremena početka.");
                return View(rezervacija);
            }

            var staraCijena = postojecaRezervacija.UkupnaCijena;

            var novaCijena = _service.IzracunajCijenu(
                rezervacija.VrijemePocetka,
                rezervacija.VrijemeKraja,
                parkingMjesto.CijenaPoSatu);

            var razlikaZaDoplatu = novaCijena - staraCijena;

            if (razlikaZaDoplatu > 0)
            {
                var izmjenaZaPlacanje = new Rezervacija
                {
                    Id = postojecaRezervacija.Id,
                    KorisnikId = postojecaRezervacija.KorisnikId,
                    ParkingMjestoId = postojecaRezervacija.ParkingMjestoId,
                    DatumKreiranja = postojecaRezervacija.DatumKreiranja,
                    VrijemePocetka = rezervacija.VrijemePocetka,
                    VrijemeKraja = rezervacija.VrijemeKraja,
                    RegistracijskeTablice = rezervacija.RegistracijskeTablice,
                    KontaktTelefon = rezervacija.KontaktTelefon,
                    EmailZaObavijest = rezervacija.EmailZaObavijest,
                    UkupnaCijena = novaCijena,
                    StatusRezervacije = StatusRezervacije.Aktivna
                };

                TempData["IzmjenaRezervacijeZaPlacanje"] = JsonSerializer.Serialize(izmjenaZaPlacanje);
                TempData["IznosDoplate"] = razlikaZaDoplatu.ToString();

                return RedirectToAction("Create", "Placanje");
            }

            postojecaRezervacija.VrijemePocetka = rezervacija.VrijemePocetka;
            postojecaRezervacija.VrijemeKraja = rezervacija.VrijemeKraja;
            postojecaRezervacija.RegistracijskeTablice = rezervacija.RegistracijskeTablice;
            postojecaRezervacija.KontaktTelefon = rezervacija.KontaktTelefon;
            postojecaRezervacija.EmailZaObavijest = rezervacija.EmailZaObavijest;
            postojecaRezervacija.UkupnaCijena = novaCijena;
            postojecaRezervacija.StatusRezervacije = StatusRezervacije.Aktivna;

            try
            {
                await _service.UpdateAsync(postojecaRezervacija);
                TempData["Success"] = "Rezervacija je uspješno ažurirana.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RezervacijaExists(postojecaRezervacija.Id))
                    return NotFound();

                throw;
            }

            if (User.IsInRole("Administrator") || User.IsInRole("Operater"))
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Moje));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rezervacija = await _service.GetByIdAsync(id.Value);
            if (rezervacija == null) return NotFound();

            if (!User.IsInRole("Administrator") && !User.IsInRole("Operater"))
            {
                var korisnik = await _userManager.GetUserAsync(User);

                if (korisnik == null || rezervacija.KorisnikId != korisnik.Id)
                    return Forbid();
            }

            return View(rezervacija);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rezervacija = await _service.GetByIdAsync(id);
            if (rezervacija == null) return NotFound();

            if (!User.IsInRole("Administrator") && !User.IsInRole("Operater"))
            {
                var korisnik = await _userManager.GetUserAsync(User);

                if (korisnik == null || rezervacija.KorisnikId != korisnik.Id)
                    return Forbid();
            }

            rezervacija.StatusRezervacije = StatusRezervacije.Otkazana;

            await _service.UpdateAsync(rezervacija);

            var placanje = await _placanjeService.GetUspjesnoPlacanjeZaRezervacijuAsync(rezervacija.Id);

            if (placanje != null)
            {
                placanje.StatusPlacanja = StatusPlacanja.Refundirano;
                await _placanjeService.UpdateAsync(placanje);
            }

            TempData["Success"] = "Rezervacija je otkazana, a plaćanje je refundirano.";

            if (!string.IsNullOrWhiteSpace(rezervacija.EmailZaObavijest))
            {
                await _emailSender.SendEmailAsync(
                    rezervacija.EmailZaObavijest,
                    "Rezervacija otkazana - Pametni Parking Sistem",
                    KreirajEmailTemplate(
                        "Otkazivanje rezervacije",
                        "Vaša rezervacija je otkazana",
                        $@"
                        <p><b>Početak:</b> {rezervacija.VrijemePocetka:dd.MM.yyyy HH:mm}</p>
                        <p><b>Kraj:</b> {rezervacija.VrijemeKraja:dd.MM.yyyy HH:mm}</p>
                        <p><b>Registracijske tablice:</b> {rezervacija.RegistracijskeTablice}</p>
                        <p><b>Iznos:</b> {rezervacija.UkupnaCijena:0.00} KM</p>
                        <p><b>Status plaćanja:</b> Refundirano</p>",
                        "Novac je evidentiran kao refundiran u sistemu."
                    )
                );
            }

            if (User.IsInRole("Administrator") || User.IsInRole("Operater"))
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Historija));
        }

        private async Task<bool> RezervacijaExists(int id)
        {
            return await _service.GetByIdAsync(id) != null;
        }

        private async Task AzurirajIstekleRezervacijeAsync()
        {
            var istekleRezervacije = await _service.GetIstekleAktivneRezervacijeAsync();

            foreach (var rezervacija in istekleRezervacije)
            {
                rezervacija.StatusRezervacije = StatusRezervacije.Zavrsena;
                await _service.UpdateAsync(rezervacija);
            }
        }

        private string KreirajEmailTemplate(string naslov, string poruka, string sadrzaj, string footer = "Hvala što koristite Pametni Parking Sistem.")
        {
            return $@"
            <div style='font-family:Segoe UI, Arial, sans-serif; background:#f4f8f5; padding:30px; color:#1f2937;'>
                <div style='max-width:650px; margin:auto; background:white; border-radius:18px; overflow:hidden; box-shadow:0 8px 24px rgba(0,0,0,0.08);'>
                    <div style='background:linear-gradient(90deg,#1f7a4d,#2ea86b); padding:26px; color:white;'>
                        <h1 style='margin:0; font-size:26px;'>Pametni Parking Sistem</h1>
                        <p style='margin:6px 0 0 0; opacity:0.9;'>{naslov}</p>
                    </div>

                    <div style='padding:30px;'>
                        <h2 style='color:#166534; margin-top:0;'>{poruka}</h2>

                        <div style='background:#f8faf9; border:1px solid #e5e7eb; border-radius:14px; padding:20px; margin:20px 0;'>
                            {sadrzaj}
                        </div>

                        <p style='font-size:14px; color:#64748b;'>{footer}</p>
                    </div>

                    <div style='background:#f1f5f3; padding:18px; text-align:center; font-size:13px; color:#64748b;'>
                        Ovo je automatska poruka. Molimo ne odgovarajte na ovaj email.
                    </div>
                </div>
            </div>";
        }
    }
}