using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Controllers
{
    public class EmailObavijestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmailObavijestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmailObavijest
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmailObavijesti.ToListAsync());
        }

        // GET: EmailObavijest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailObavijest = await _context.EmailObavijesti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailObavijest == null)
            {
                return NotFound();
            }

            return View(emailObavijest);
        }

        // GET: EmailObavijest/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmailObavijest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Primalac,Naslov,Sadrzaj,DatumSlanja,TipObavijesti,StatusEmaila")] EmailObavijest emailObavijest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emailObavijest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emailObavijest);
        }

        // GET: EmailObavijest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailObavijest = await _context.EmailObavijesti.FindAsync(id);
            if (emailObavijest == null)
            {
                return NotFound();
            }
            return View(emailObavijest);
        }

        // POST: EmailObavijest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Primalac,Naslov,Sadrzaj,DatumSlanja,TipObavijesti,StatusEmaila")] EmailObavijest emailObavijest)
        {
            if (id != emailObavijest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emailObavijest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailObavijestExists(emailObavijest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(emailObavijest);
        }

        // GET: EmailObavijest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailObavijest = await _context.EmailObavijesti
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailObavijest == null)
            {
                return NotFound();
            }

            return View(emailObavijest);
        }

        // POST: EmailObavijest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emailObavijest = await _context.EmailObavijesti.FindAsync(id);
            if (emailObavijest != null)
            {
                _context.EmailObavijesti.Remove(emailObavijest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmailObavijestExists(int id)
        {
            return _context.EmailObavijesti.Any(e => e.Id == id);
        }
    }
}
