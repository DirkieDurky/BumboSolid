using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using System.Globalization;
using BumboSolid.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace BumboSolid.Controllers
{
	[Route("Prognoses")]
    public class PrognosesController : Controller
	{
		private readonly BumboDbContext _context;

		public PrognosesController(BumboDbContext context)
		{
			_context = context;
		}

		// GET: Prognoses
		[HttpGet("~/")]
		[HttpGet("{id:int?}")]
		public async Task<IActionResult> Index(int? id)
		{
			List<Week> prognoses = await _context.Weeks
				.Include(p => p.PrognosisDays)
					.ThenInclude(p => p.PrognosisDepartments)
				.Include(p => p.PrognosisDays)
					.ThenInclude(pd => pd.Factors)
					.OrderByDescending(p => p.Year)
					   .ThenByDescending(p => p.WeekNumber)
				.ToListAsync();

			int? lastPrognosisId = null;
			if (id == null && prognoses.Count != 0)
			{
				lastPrognosisId = id ?? prognoses.OrderBy(x => x.Year).ThenBy(c => c.WeekNumber).Last().Id;
			}

			var viewModel = new PrognosesViewModel
			{
				Prognoses = prognoses,
				Id = id ?? lastPrognosisId
			};

			return View(viewModel);
		}

		// GET: Prognoses/Aanmaken
		[HttpGet("Aanmaken")]
		public IActionResult Create()
		{
			CultureInfo ci = new CultureInfo("nl-NL");
			Calendar calendar = ci.Calendar;

			//Create prognosis for next week
			DateTime nextWeek = DateTime.Now.AddDays(7);
			short year = (short)nextWeek.Year;
			byte week = (byte)calendar.GetWeekOfYear(nextWeek, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);

			//If a prognosis already exists for this week, add another week (and keep doing that until we find one that isn't used yet)
			while (_context.Weeks.Any(p => p.Year == year && p.WeekNumber == week))
			{
				year = (short)nextWeek.Year;
				week = (byte)calendar.GetWeekOfYear(nextWeek, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
				nextWeek = nextWeek.AddDays(7);
			}

			Week newWeek = new Week()
			{
				Id = _context.Weeks.Count() > 0 ? _context.Weeks.Max(x => x.Id) + 1 : 0,
				Year = year,
				WeekNumber = week,
			};

			//Fill holidays in accordingly, and make the rest zeroes
			for (byte i = 0; i < 7; i++)
			{
				PrognosisDay prognosisDay = new PrognosisDay()
				{
					PrognosisId = newWeek.Id,
					Weekday = i,
					VisitorEstimate = 0,
				};

				//Get current day from year, week and weekday
				DateTime startOfYear = new DateTime(newWeek.Year, 1, 1);
				DateTime currentDay = calendar.AddWeeks(startOfYear, newWeek.WeekNumber - 1).AddDays(i - (int)startOfYear.DayOfWeek + 1);

				var temp = _context.HolidayDays.ToList();
				List<HolidayDay> holidayInfo = temp.Where(d => d.Date == DateOnly.FromDateTime(currentDay)).ToList();

				prognosisDay.Factors.Add(new Factor()
				{
					PrognosisId = prognosisDay.PrognosisId,
					Type = "Feestdagen",
					Weekday = prognosisDay.Weekday,
					Impact = (short)(holidayInfo.Count() == 0 ? 0 : holidayInfo.First().Impact),
				});

				prognosisDay.Factors.Add(new Factor()
				{
					PrognosisId = prognosisDay.PrognosisId,
					Type = "Weer",
					Weekday = prognosisDay.Weekday,
					Impact = 3,
				});

				prognosisDay.Factors.Add(new Factor()
				{
					PrognosisId = prognosisDay.PrognosisId,
					Type = "Overig",
					Weekday = prognosisDay.Weekday,
					Impact = 0,
				});

				newWeek.PrognosisDays.Add(prognosisDay);
			}

			IEnumerable<Week> prognoses = _context.Weeks.Include(p => p.PrognosisDays).OrderBy(x => x.Year).ThenBy(x => x.WeekNumber).ToList();

			CreatePrognosisViewModel CreatePrognosisViewModel = new CreatePrognosisViewModel()
			{
				Prognosis = newWeek,
				VisitorEstimatePerDay = prognoses.Count() > 0 ? prognoses.Last().PrognosisDays.ToDictionary(p => p.Weekday, p => p.VisitorEstimate) : null,
				WeatherValues = _context.Weathers.ToList(),
				Norms = _context.Norms.ToList(),
			};

			return View(CreatePrognosisViewModel);
		}

		// POST: Prognoses/Aanmaken
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[ValidateAntiForgeryToken]
		[HttpPost("Aanmaken")]
		public async Task<IActionResult> Create(Week prognosis, List<int> visitorEstimates, List<int> holidays, List<int> weather, List<int> other, List<String> description)
		{
			var norms = _context.Norms
				.Select(norm => new { norm.Duration, norm.AvgDailyPerformances, norm.PerVisitor, norm.Department })
				.ToList();
			string cashregister = _context.Departments.ToList()[0].Name;
			string shelves = _context.Departments.ToList()[1].Name;
			string fresh = _context.Departments.ToList()[2].Name;

			double normCashVisitor = norms.Where(norm => norm.PerVisitor && norm.Department.Equals(cashregister))
				.Select(norm => norm.Duration * norm.AvgDailyPerformances)
				.Sum();
			double normCashDay = norms.Where(norm => !norm.PerVisitor && norm.Department.Equals(cashregister))
				.Select(norm => norm.Duration * norm.AvgDailyPerformances)
				.Sum();
			double normShelvesVisitor = norms.Where(norm => norm.PerVisitor && norm.Department.Equals(shelves))
				.Select(norm => norm.Duration * norm.AvgDailyPerformances)
				.Sum();
			double normShelvesDay = norms.Where(norm => !norm.PerVisitor && norm.Department.Equals(shelves))
				.Select(norm => norm.Duration * norm.AvgDailyPerformances)
				.Sum();
			double normFreshVisitor = norms.Where(norm => norm.PerVisitor && norm.Department.Equals(fresh))
				.Select(norm => norm.Duration * norm.AvgDailyPerformances)
				.Sum();
			double normFreshDay = norms.Where(norm => !norm.PerVisitor && norm.Department.Equals(fresh))
				.Select(norm => norm.Duration * norm.AvgDailyPerformances)
				.Sum();


			if (!ModelState.IsValid) return View(prognosis);

			_context.Add(prognosis);

			await _context.SaveChangesAsync();

			for (int i = 0; i < 7; i++)
			{
				double holidayd = holidays[i] / 100.00 + 1.0;
				double weatherd = _context.Weathers.First(x => x.Id == (byte)weather[i]).Impact / 100 + 1.0;
				double otherd = other[i] / 100.00 + 1;

				_context.Add(new PrognosisDay()
				{
					PrognosisId = prognosis.Id,
					Weekday = (byte)i,
					VisitorEstimate = visitorEstimates[i],
				});

				await _context.SaveChangesAsync();

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
					WeatherId = (byte)weather[i],
					Impact = _context.Weathers.First(x => x.Id == (byte)weather[i]).Impact,
				});

				_context.Add(new Factor()
				{
					PrognosisId = prognosis.Id,
					Type = "Overig",
					Weekday = (byte)i,
					Impact = (short)other[i],
					Description = description[i],
				});

				_context.Add(new PrognosisDepartment()
				{
					PrognosisId = prognosis.Id,
					Department = cashregister,
					Weekday = (byte)i,
					WorkHours = (short)((normCashDay + (normCashVisitor * visitorEstimates[i])) * (holidayd * weatherd * otherd) / 3600)
				});

				_context.Add(new PrognosisDepartment()
				{
					PrognosisId = prognosis.Id,
					Department = shelves,
					Weekday = (byte)i,
					WorkHours = (short)((normShelvesDay + (normShelvesVisitor * visitorEstimates[i])) * (holidayd * weatherd * otherd) / 3600)
				});


				_context.Add(new PrognosisDepartment()
				{
					PrognosisId = prognosis.Id,
					Department = fresh,
					Weekday = (byte)i,
					WorkHours = (short)((normFreshDay + (normFreshVisitor * visitorEstimates[i])) * (holidayd * weatherd * otherd) / 3600)
				});
			}
			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Prognoses");
		}

		// GET: Prognoses/Bewerken/5
		[HttpGet("Bewerken/{id:int?}")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var prognosis = await _context.Weeks.FindAsync(id);
			if (prognosis == null)
			{
				return NotFound();
			}
			return View(prognosis);
		}

		// POST: Prognoses/Bewerken/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[ValidateAntiForgeryToken]
		[HttpPost("Bewerken/{id:int?}")]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Year,Week")] Week prognosis)
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

		private bool PrognosisExists(int id)
		{
			return _context.Weeks.Any(e => e.Id == id);
		}
	}
}
