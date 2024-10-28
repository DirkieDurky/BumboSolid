using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;

namespace BumboSolid.Controllers
{
    public class WeerController : Controller
    {
        private readonly BumboDbContext _context;

        public WeerController(BumboDbContext context)
        {
            _context = context;
        }

        // GET: Weer/Bewerken/5
        public IActionResult Bewerken()
        {
            return View(new WeatherManageViewModel() { Impacts = _context.Weathers.Select(w => w.Impact).ToArray() });
        }

        // POST: Weer/Bewerken/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bewerken(short[] impacts)
        {
            if (ModelState.IsValid)
            {
                for (byte i=0;i<7;i++) {
                    try
                    {
                        _context.Update(new Weather() { Id = i, Impact = impacts[i] });
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!WeatherExists(i))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
				}
				return RedirectToAction("Index", "Prognoses");
			}

            return View(impacts);
        }

        private bool WeatherExists(byte id)
        {
            return _context.Weathers.Any(e => e.Id == id);
        }
    }
}
