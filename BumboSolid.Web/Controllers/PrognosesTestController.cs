using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;

namespace BumboSolid.Web.Controllers
{
    public class PrognosesTestController : Controller
    {
        private readonly BumboDbContext _context;

        public PrognosesTestController(BumboDbContext context)
        {
            _context = context;
        }

        // GET: PrognosesTest
        public async Task<IActionResult> Index()
        {
            return View(await _context.Prognoses.ToListAsync());
        }

        // GET: PrognosesTest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prognosis = await _context.Prognoses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prognosis == null)
            {
                return NotFound();
            }

            return View(prognosis);
        }

        // GET: PrognosesTest/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PrognosesTest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Year,Week")] Prognosis prognosis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prognosis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prognosis);
        }

        // GET: PrognosesTest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prognosis = await _context.Prognoses.FindAsync(id);
            if (prognosis == null)
            {
                return NotFound();
            }
            return View(prognosis);
        }

        // POST: PrognosesTest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Year,Week")] Prognosis prognosis)
        {
            if (id != prognosis.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prognosis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrognosisExists(prognosis.Id))
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
            return View(prognosis);
        }

        // GET: PrognosesTest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prognosis = await _context.Prognoses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prognosis == null)
            {
                return NotFound();
            }

            return View(prognosis);
        }

        // POST: PrognosesTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prognosis = await _context.Prognoses.FindAsync(id);
            if (prognosis != null)
            {
                _context.Prognoses.Remove(prognosis);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrognosisExists(int id)
        {
            return _context.Prognoses.Any(e => e.Id == id);
        }
    }
}
