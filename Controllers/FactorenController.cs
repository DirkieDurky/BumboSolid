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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Bewerken(int id, EditPrognosisFactorsViewModel model)
		{
			var prognosis = await _context.Prognoses
				.Include(p => p.PrognosisDays)
				.ThenInclude(pd => pd.Factors)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (prognosis == null)
			{
				return NotFound();
			}

			for (int i = 0; i < model.VisitorEstimates.Length; i++)
			{
				var prognosisDay = prognosis.PrognosisDays.FirstOrDefault(pd => pd.Weekday == i);
				if (prognosisDay != null)
				{
					// Update Visitor Estimate
					prognosisDay.VisitorEstimate = model.VisitorEstimates[i];

					// Update Factors
					var holidayFactor = prognosisDay.Factors.FirstOrDefault(f => f.Type == "Feestdagen");
					if (holidayFactor != null)
					{
						holidayFactor.Impact = (short)model.Holidays[i];
					}

					var weatherFactor = prognosisDay.Factors.FirstOrDefault(f => f.Type == "Weer");
					if (weatherFactor != null)
					{
						weatherFactor.WeatherId = (byte)model.WeatherIds[i];
					}

					var otherFactor = prognosisDay.Factors.FirstOrDefault(f => f.Type == "Overig");
					if (otherFactor != null)
					{
						otherFactor.Impact = (short)model.Others[i];
						otherFactor.Description = model.Descriptions[i];
					}
				}
			}

			await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Prognoses");
        }

	}
}
