using BumboSolid.Data;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Controllers
{
	public class FactorenController : Controller
	{
		private readonly BumboDbContext _context;

		public FactorenController(BumboDbContext context)
		{
			_context = context;
		}

		// GET: FactorenController/Bewerken/5
		public async Task<IActionResult> Bewerken(int id)
		{
			EditPrognosisFactorsViewModel editPrognosisFactorsViewModel = new EditPrognosisFactorsViewModel();

			var prognosis = await _context.Prognoses
				.Include(p => p.PrognosisDays)
					.ThenInclude(pd => pd.Factors)
						.ThenInclude(f => f.TypeNavigation)
							.FirstOrDefaultAsync(p => p.Id == id);

			editPrognosisFactorsViewModel.Prognosis = prognosis;
			editPrognosisFactorsViewModel.WeatherValues = _context.Weathers.ToList();

			return View(editPrognosisFactorsViewModel);
		}

		// POST: FactorenController/Bewerken/5
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
	}
}
