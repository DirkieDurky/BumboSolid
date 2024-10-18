using BumboSolid.Data;
using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Web.Controllers
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
            ViewBag.Function = new SelectList(new List<string> { "Vers", "Kassa", "Vakkenvullen" });
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

            // Convert Duration to seconds based on the selected DurationUnit
            switch (DurationUnit.ToLower())
            {
                case "minutes":
                    norm.Duration *= 60;
                    break;
                case "hours":
                    norm.Duration *= 3600;
                    break;
            }

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

        // POST: Normeringen/Bewerken/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bewerken(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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