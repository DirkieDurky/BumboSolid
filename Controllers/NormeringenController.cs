using BumboSolid.Data;
using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Controllers
{
    public class NormeringenController : Controller
    {
        private readonly BumboDbContext _context;

        public NormeringenController(BumboDbContext context)
        {
            _context = context;
        }

        // GET: Normeringen/Index/5
        public ActionResult Index()
        {
            var normList = _context.Norms.ToList();
            return View(normList);
        }

        // GET: Normeringen/Aanmaken
        public ActionResult Aanmaken()
        {
            ViewBag.Function = new SelectList(_context.Functions.Select(f => f.Name).ToList());
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });

            return View(new Norm());
        }

        // POST: Normeringen/Aanmaken
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aanmaken(Norm norm, string DurationUnit)
        {
            var selectedFunction = _context.Functions.FirstOrDefault(f => f.Name == norm.Function);

            if (selectedFunction == null)
            {
                ModelState.AddModelError("Function", "The selected function is not valid.");
                ViewBag.Function = new SelectList(_context.Functions.Select(f => f.Name).ToList());
                ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
                return View(norm);
            }

            norm.FunctionNavigation = selectedFunction;

            // Define the maximum allowable duration in seconds
            const int maxIntValue = int.MaxValue;
            int calculatedDuration = norm.Duration;

            // Convert Duration to seconds based on the selected DurationUnit
            switch (DurationUnit.ToLower())
            {
                case "minutes":
                    // Check for potential overflow
                    if (calculatedDuration > maxIntValue / 60)
                    {
                        ModelState.AddModelError("Duration", "Duur is een te groot getal na het converteren naar seconden.");
                        ViewBag.Function = new SelectList(_context.Functions.Select(f => f.Name).ToList());
                        ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
                        return View(norm);
                    }
                    calculatedDuration *= 60;
                    break;

                case "hours":
                    // Check for potential overflow
                    if (calculatedDuration > maxIntValue / 3600)
                    {
                        ModelState.AddModelError("Duration", "Duur is een te groot getal na het converteren naar seconden.");
                        ViewBag.Function = new SelectList(_context.Functions.Select(f => f.Name).ToList());
                        ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
                        return View(norm);
                    }
                    calculatedDuration *= 3600;
                    break;
            }

            norm.Duration = calculatedDuration;

            // Assign auto incremented Id
            int maxId = _context.Norms.Any() ? _context.Norms.Max(n => n.Id) : 0;
            norm.Id = maxId + 1;

            // Check if the model state is still valid before saving to the database
            if (ModelState.IsValid)
            {
                _context.Norms.Add(norm);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Function = new SelectList(_context.Functions.Select(f => f.Name).ToList());
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
            return View(norm);
        }

        // GET: Normeringen/Bewerken/5
        public async Task<IActionResult> Bewerken(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var norm = await _context.Norms.FindAsync(id);
            if (norm == null)
            {
                return NotFound();
            }

            ViewBag.Function = new SelectList(_context.Functions.Select(f => f.Name).ToList(), norm.Function);
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });

            return View(norm);
        }

        // POST: Normeringen/Bewerken/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bewerken(int id, Norm norm, string DurationUnit)
        {
            if (id != norm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Define the maximum allowable duration in seconds
                    const int maxIntValue = int.MaxValue;
                    int calculatedDuration = norm.Duration;

                    // Convert Duration to seconds based on the selected DurationUnit
                    switch (DurationUnit.ToLower())
                    {
                        case "minutes":
                            // Check for potential overflow
                            if (calculatedDuration > maxIntValue / 60)
                            {
                                ModelState.AddModelError("Duration", "Duur is een te groot getal na het converteren naar minuten.");
                                ViewBag.Function = new SelectList(new List<string> { "Vers", "Kassa", "Vakkenvullen" }, norm.Function);
                                ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
                                return View(norm);
                            }
                            calculatedDuration *= 60;
                            break;

                        case "hours":
                            // Check for potential overflow
                            if (calculatedDuration > maxIntValue / 3600)
                            {
                                ModelState.AddModelError("Duration", "Duur is een te groot getal na het converteren naar seconden.");
                                ViewBag.Function = new SelectList(new List<string> { "Vers", "Kassa", "Vakkenvullen" }, norm.Function);
                                ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
                                return View(norm);
                            }
                            calculatedDuration *= 3600;
                            break;
                    }

                    norm.Duration = calculatedDuration;

                    _context.Update(norm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Norms.Any(e => e.Id == norm.Id))
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

            ViewBag.Function = new SelectList(new List<string> { "Vers", "Kassa", "Vakkenvullen" }, norm.Function);
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
            return View(norm);
        }


        // GET: Normeringen/Verwijderen/5
        public async Task<IActionResult> Verwijderen(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var norm = await _context.Norms.FirstOrDefaultAsync(n => n.Id == id);
            if (norm == null)
            {
                return NotFound();
            }

            return View(norm);
        }

        // POST: Normeringen/Verwijderen/5
        [HttpPost, ActionName("Verwijderen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerwijderenConfirmed(int id)
        {
            var norm = await _context.Norms.FindAsync(id);
            if (norm != null)
            {
                _context.Norms.Remove(norm);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}