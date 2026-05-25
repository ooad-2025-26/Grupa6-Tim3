using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using PametniParkingSistem.ViewModels.Placanje;
using System.Text.Json;


namespace PametniParkingSistem.Controllers
{
    [Authorize]
    public class PlacanjeController : Controller
    {
        private readonly IPlacanjeService _service;
        private readonly IRezervacijaService _rezervacijaService;
        private readonly IParkingMjestoService _parkingMjestoService;
        private readonly IEmailSenderService _emailSender;

        public PlacanjeController(
            IPlacanjeService service,
            IRezervacijaService rezervacijaService,
            IParkingMjestoService parkingMjestoService,
            IEmailSenderService emailSender)
        {
            _service = service;
            _rezervacijaService = rezervacijaService;
            _parkingMjestoService = parkingMjestoService;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAsync());
        }

        public async Task<IActionResult> Create()
        {
            if (TempData.ContainsKey("RezervacijaZaPlacanje"))
            {
                var json = TempData["RezervacijaZaPlacanje"]?.ToString();

                if (string.IsNullOrEmpty(json))
                    return RedirectToAction("Index", "ParkingMjesto");

                TempData.Keep("RezervacijaZaPlacanje");

                var rezervacija = JsonSerializer.Deserialize<Rezervacija>(json);

                if (rezervacija == null)
                    return RedirectToAction("Index", "ParkingMjesto");

                var model = new PlacanjeRezervacijeViewModel
                {
                    Iznos = rezervacija.UkupnaCijena
                };

                ViewBag.Rezervacija = rezervacija;
                ViewBag.TipPlacanja = "NovaRezervacija";

                return View(model);
            }

            if (TempData.ContainsKey("IzmjenaRezervacijeZaPlacanje"))
            {
                var json = TempData["IzmjenaRezervacijeZaPlacanje"]?.ToString();

                if (string.IsNullOrEmpty(json))
                    return RedirectToAction("Index", "Rezervacija");

                TempData.Keep("IzmjenaRezervacijeZaPlacanje");
                TempData.Keep("IznosDoplate");

                var rezervacija = JsonSerializer.Deserialize<Rezervacija>(json);

                if (rezervacija == null)
                    return RedirectToAction("Index", "Rezervacija");

                double iznosDoplate = Convert.ToDouble(TempData["IznosDoplate"]);

                var model = new PlacanjeRezervacijeViewModel
                {
                    Iznos = iznosDoplate
                };

                ViewBag.Rezervacija = rezervacija;
                ViewBag.TipPlacanja = "Doplata";

                return View(model);
            }

            return RedirectToAction("Index", "ParkingMjesto");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlacanjeRezervacijeViewModel model)
        {
            string brojKartice = model.BrojKartice.Replace(" ", "");

            if (!ModelState.IsValid)
                return View(model);

            if (brojKartice.Length < 12)
            {
                ModelState.AddModelError("BrojKartice", "Broj kartice nije ispravan.");
                return View(model);
            }

            if (model.CVV.Length < 3)
            {
                ModelState.AddModelError("CVV", "CVV nije ispravan.");
                return View(model);
            }

            string zadnje4 = brojKartice.Substring(brojKartice.Length - 4);

            if (TempData.ContainsKey("RezervacijaZaPlacanje"))
            {
                var json = TempData["RezervacijaZaPlacanje"]?.ToString();

                if (string.IsNullOrEmpty(json))
                    return RedirectToAction("Index", "ParkingMjesto");

                var rezervacija = JsonSerializer.Deserialize<Rezervacija>(json);

                if (rezervacija == null)
                    return RedirectToAction("Index", "ParkingMjesto");

                rezervacija.StatusRezervacije = StatusRezervacije.Aktivna;

                await _rezervacijaService.AddAsync(rezervacija);

                var placanje = new Placanje
                {
                    ImeVlasnikaKartice = model.ImeVlasnikaKartice,
                    BrojKarticeMaskiran = $"**** **** **** {zadnje4}",
                    DatumPlacanja = DateTime.Now,
                    Iznos = rezervacija.UkupnaCijena,
                    StatusPlacanja = StatusPlacanja.Uspjesno,
                    TransakcijskiBroj = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    RezervacijaId = rezervacija.Id
                };


                await _service.AddAsync(placanje);


                var parkingMjesto = await _parkingMjestoService.GetByIdAsync(rezervacija.ParkingMjestoId);

                var qrText = $"REZERVACIJA-{rezervacija.Id}-{parkingMjesto?.Oznaka}-{rezervacija.RegistracijskeTablice}";

                var qrUrl =
      "https://api.qrserver.com/v1/create-qr-code/?size=220x220&data="
      + Uri.EscapeDataString(qrText);

                await _emailSender.SendEmailAsync(
                    rezervacija.EmailZaObavijest,
                    "Potvrda rezervacije - Pametni Parking Sistem",
                    KreirajEmailTemplate(
                        "Potvrda rezervacije",
                        "Vaša rezervacija je uspješno potvrđena",
                        $@"
        <p><b>Parking mjesto:</b> {parkingMjesto?.Oznaka}</p>
        <p><b>Početak:</b> {rezervacija.VrijemePocetka:dd.MM.yyyy HH:mm}</p>
        <p><b>Kraj:</b> {rezervacija.VrijemeKraja:dd.MM.yyyy HH:mm}</p>
        <p><b>Registracijske tablice:</b> {rezervacija.RegistracijskeTablice}</p>
        <p><b>Iznos plaćanja:</b> {rezervacija.UkupnaCijena:0.00} KM</p>
        <p><b>Transakcijski broj:</b> {placanje.TransakcijskiBroj}</p>
<hr />
<h3 style='color:#166534;'>QR kod za ulazak</h3>
<p>Skenirajte ovaj QR kod prilikom ulaska na parking.</p>
<div style='text-align:center; margin-top:15px;'>
    <img src='{qrUrl}' alt='QR kod rezervacije' width='220' height='220' />
</div>
<p style='font-size:13px; color:#64748b; text-align:center;'>
    Kod rezervacije: {qrText}
</p>
        "
                    )
                );


                if (parkingMjesto != null)
                {
                    parkingMjesto.Status = StatusMjesta.Rezervisano;
                    await _parkingMjestoService.UpdateAsync(parkingMjesto);
                }

                TempData.Remove("RezervacijaZaPlacanje");
                TempData["Success"] = "Rezervacija je uspješno plaćena i potvrđena.";

                return RedirectToAction("Index", "Rezervacija");
            }

            if (TempData.ContainsKey("IzmjenaRezervacijeZaPlacanje"))
            {
                var json = TempData["IzmjenaRezervacijeZaPlacanje"]?.ToString();

                if (string.IsNullOrEmpty(json))
                    return RedirectToAction("Index", "Rezervacija");

                var izmjena = JsonSerializer.Deserialize<Rezervacija>(json);

                if (izmjena == null)
                    return RedirectToAction("Index", "Rezervacija");

                var postojecaRezervacija = await _rezervacijaService.GetByIdAsync(izmjena.Id);

                if (postojecaRezervacija == null)
                    return NotFound();

                double iznosDoplate = Convert.ToDouble(TempData["IznosDoplate"]);

                postojecaRezervacija.VrijemePocetka = izmjena.VrijemePocetka;
                postojecaRezervacija.VrijemeKraja = izmjena.VrijemeKraja;
                postojecaRezervacija.RegistracijskeTablice = izmjena.RegistracijskeTablice;
                postojecaRezervacija.KontaktTelefon = izmjena.KontaktTelefon;
                postojecaRezervacija.EmailZaObavijest = izmjena.EmailZaObavijest;
                postojecaRezervacija.UkupnaCijena = izmjena.UkupnaCijena;
                postojecaRezervacija.StatusRezervacije = StatusRezervacije.Aktivna;

                await _rezervacijaService.UpdateAsync(postojecaRezervacija);

                var placanje = new Placanje
                {
                    ImeVlasnikaKartice = model.ImeVlasnikaKartice,
                    BrojKarticeMaskiran = $"**** **** **** {zadnje4}",
                    DatumPlacanja = DateTime.Now,
                    Iznos = iznosDoplate,
                    StatusPlacanja = StatusPlacanja.Uspjesno,
                    TransakcijskiBroj = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    RezervacijaId = postojecaRezervacija.Id
                };

                await _service.AddAsync(placanje);

                await _emailSender.SendEmailAsync(
                    postojecaRezervacija.EmailZaObavijest,
                    "Potvrda doplate - Pametni Parking Sistem",
                    KreirajEmailTemplate(
                        "Potvrda doplate",
                        "Doplata je uspješno izvršena",
                        $@"
        <p><b>Rezervacija je ažurirana.</b></p>
        <p><b>Novi početak:</b> {postojecaRezervacija.VrijemePocetka:dd.MM.yyyy HH:mm}</p>
        <p><b>Novi kraj:</b> {postojecaRezervacija.VrijemeKraja:dd.MM.yyyy HH:mm}</p>
        <p><b>Doplaćeni iznos:</b> {iznosDoplate:0.00} KM</p>
        <p><b>Ukupna cijena rezervacije:</b> {postojecaRezervacija.UkupnaCijena:0.00} KM</p>
        <p><b>Transakcijski broj:</b> {placanje.TransakcijskiBroj}</p>
        "
                    )
                );

                TempData.Remove("IzmjenaRezervacijeZaPlacanje");
                TempData.Remove("IznosDoplate");
                TempData["Success"] = "Doplata je uspješno izvršena i rezervacija je ažurirana.";

                return RedirectToAction("Index", "Rezervacija");
            }

            return RedirectToAction("Index", "ParkingMjesto");
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