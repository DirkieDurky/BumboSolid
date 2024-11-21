using BumboSolid.Data;
using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Controllers
{
    [Route("Normeringen")]
    public class NormsController : Controller
    {
        private readonly BumboDbContext _context;

        public NormsController(BumboDbContext context)
        {
            _context = context;
        }

        // GET: Normeringen/Index/5
        [HttpGet("")]
        public ActionResult Index()
        {
            var normList = _context.Norms.ToList();
            return View(normList);
        }

		// GET: Normeringen/Aanmaken
		[HttpGet("Aanmaken")]
		public ActionResult Create()
        {
            ViewBag.Department = new SelectList(_context.Departments.Select(f => f.Name).ToList());
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });

            return View(new Norm());
        }

        // POST: Normeringen/Aanmaken
        [ValidateAntiForgeryToken]
        [HttpPost("Aanmaken")]
        public ActionResult Create(Norm norm, string DurationUnit)
        {
            var selectedDepartment = _context.Departments.FirstOrDefault(f => f.Name == norm.Department);

            if (selectedDepartment == null)
            {
                ModelState.AddModelError("Function", "The selected function is not valid.");
                ViewBag.Department = new SelectList(_context.Departments.Select(f => f.Name).ToList());
                ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
                return View(norm);
            }

            norm.DepartmentNavigation = selectedDepartment;

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
                        ViewBag.Function = new SelectList(_context.Departments.Select(f => f.Name).ToList());
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
                        ViewBag.Function = new SelectList(_context.Departments.Select(f => f.Name).ToList());
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

            ViewBag.Department = new SelectList(_context.Departments.Select(f => f.Name).ToList());
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
            return View(norm);
        }

        // GET: Normeringen/Bewerken/5
        [HttpGet("Bewerken/{id:int?}")]
        public async Task<IActionResult> Edit(int? id)
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

            ViewBag.Department = new SelectList(_context.Departments.Select(f => f.Name).ToList(), norm.Department);
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });

            return View(norm);
        }

        // POST: Normeringen/Bewerken/5
        [ValidateAntiForgeryToken]
		[HttpPost("Bewerken/{id:int?}")]
		public async Task<IActionResult> Edit(int id, Norm norm, string DurationUnit)
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
                                ViewBag.Department = new SelectList(new List<string> { "Vers", "Kassa", "Vakkenvullen" }, norm.Department);
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
                                ViewBag.Department = new SelectList(new List<string> { "Vers", "Kassa", "Vakkenvullen" }, norm.Department);
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

            ViewBag.Function = new SelectList(new List<string> { "Vers", "Kassa", "Vakkenvullen" }, norm.Department);
            ViewBag.TimeUnits = new SelectList(new List<string> { "Seconden", "Minuten", "Uren" });
            return View(norm);
        }

		// GET: Normeringen/Verwijderen/5
		[HttpGet("Verwijderen/{id:int}")]
		public async Task<IActionResult> Delete(int? id)
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
        [ActionName("Verwijderen")]
        [ValidateAntiForgeryToken]
		[HttpPost("Verwijderen/{id:int}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
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