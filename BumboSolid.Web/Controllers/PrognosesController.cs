using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using System.Globalization;
using BumboSolid.Web.Models;

namespace BumboSolid.Web.Controllers
{
	public class PrognosesController : Controller
	{
		private readonly BumboDbContext _context;

		public PrognosesController(BumboDbContext context)
		{
			_context = context;
		}

		// GET: Prognoses
		public async Task<IActionResult> Index()
		{
			return View(await _context.Prognoses.ToListAsync());
		}

		// GET: Prognoses/Details/5
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

		// GET: Prognoses/Create
		public IActionResult Create()
		{
			EditFactorsViewModel editFactorsViewModel = new EditFactorsViewModel();

			IEnumerable<Prognosis> prognoses = _context.Prognoses.Include(p => p.PrognosisDays).OrderBy(x => x.Year).ThenBy(x => x.Week).ToList();
			if (prognoses.Count() > 0)
			{
				editFactorsViewModel.VisitorEstimatePerDay = prognoses.Last().PrognosisDays.ToDictionary(p => p.Weekday, p => p.VisitorEstimate);
			}
			else
			{
				editFactorsViewModel.VisitorEstimatePerDay = null;
			}

			CultureInfo ci = new CultureInfo("nl-NL");
			Calendar calendar = ci.Calendar;
			DateTime nextWeek = DateTime.Now.AddDays(7);
			Prognosis newPrognosis = new Prognosis()
			{
				Id = _context.Prognoses.Max(x => x.Id) + 1,
				Year = (short)nextWeek.Year,
				Week = (byte)calendar.GetWeekOfYear(nextWeek, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek),
			};

			//Fill holidays in accordingly, and make the rest zeroes
			for (byte i = 0; i < 7; i++)
			{
				PrognosisDay prognosisDay = new PrognosisDay()
				{
					PrognosisId = newPrognosis.Id,
					Weekday = i,
					VisitorEstimate = 0,
				};

				//Get current day from year, week and weekday
				DateTime startOfYear = new DateTime(newPrognosis.Year, 1, 1);
				DateTime currentDay = calendar.AddWeeks(startOfYear, newPrognosis.Week - 1).AddDays(i - (int)startOfYear.DayOfWeek);

				var temp = _context.HolidayDays;
				IEnumerable<HolidayDay> holidayInfo = temp.Where(d => d.Date == DateOnly.FromDateTime(currentDay));

				prognosisDay.Factors.Add(new Factor()
				{
					PrognosisId = prognosisDay.PrognosisId,
					Type = "feestdagen",
					Weekday = prognosisDay.Weekday,
					Impact = (short)(holidayInfo.Count() == 0 ? 0 : holidayInfo.First().Impact),
				});

				prognosisDay.Factors.Add(new Factor()
				{
					PrognosisId = prognosisDay.PrognosisId,
					Type = "weer",
					Weekday = prognosisDay.Weekday,
					Impact = 0,
				});

				prognosisDay.Factors.Add(new Factor()
				{
					PrognosisId = prognosisDay.PrognosisId,
					Type = "overig",
					Weekday = prognosisDay.Weekday,
					Impact = 0,
				});

				newPrognosis.PrognosisDays.Add(prognosisDay);
			}

			editFactorsViewModel.Prognosis = newPrognosis;

			return View(editFactorsViewModel);
		}

		// POST: Prognoses/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Prognosis prognosis, List<int> visitorEstimates, List<int> holidays, List<int> weather, List<int> other)
		{
			if (ModelState.IsValid)
			{
				_context.Add(prognosis);

				for (int i = 0; i < 7; i++)
				{
					_context.Add(new PrognosisDay()
					{
						PrognosisId = prognosis.Id,
						Weekday = (byte)i,
						VisitorEstimate = visitorEstimates[i],
					});

					_context.Add(new Factor()
					{
						PrognosisId = prognosis.Id,
						Type = "Feestdagen",
						Weekday = (byte)i,
						Impact = (short)holidays[i],
					});

					_context.Add(new Factor()
					{
						PrognosisId = prognosis.Id,
						Type = "Weer",
						Weekday = (byte)i,
						Impact = (short)weather[i],
					});

					_context.Add(new Factor()
					{
						PrognosisId = prognosis.Id,
						Type = "Overig",
						Weekday = (byte)i,
						Impact = (short)other[i],
					});
				}
				await _context.SaveChangesAsync();
				return RedirectToAction("Index", "Home");
			}

			return View(prognosis);
		}

		// GET: Prognoses/Edit/5
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

		// POST: Prognoses/Edit/5
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

		// GET: Prognoses/Delete/5
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

		// POST: Prognoses/Delete/5
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
