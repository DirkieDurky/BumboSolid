using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using System.Globalization;
using BumboSolid.Models;
using Microsoft.AspNetCore.Authorization;

namespace BumboSolid.Controllers;

[Authorize(Roles = "Manager")]
[Route("Prognoses")]
public class PrognosesController : Controller
{
    private readonly BumboDbContext _context;

    public PrognosesController(BumboDbContext context)
    {
        _context = context;
    }

    // GET: Prognoses
    [HttpGet("")]
    [HttpGet("Index/{id:int?}")]
    public async Task<IActionResult> Index(int? id)
    {
        var prognoses = await _context.Weeks
        .Include(p => p.PrognosisDays)
            .ThenInclude(p => p.PrognosisDepartments)
        .Include(p => p.PrognosisDays)
            .ThenInclude(pd => pd.Factors)
        .OrderByDescending(p => p.Year)
        .ThenByDescending(p => p.WeekNumber)
        .ToListAsync();

        var currentWeek = await GetCurrentWeek(id);

        ViewBag.Departments = _context.Departments.Select(d => d.Name).ToList();

        var culture = CultureInfo.CurrentCulture;
        var today = DateTime.Now;
        var currentYear = (short)today.Year;
        var currentWeekNumber = (byte)culture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        var previousWeek = await _context.Weeks
            .Where(w =>
                (w.Year == currentWeek.Year && w.WeekNumber == currentWeek.WeekNumber - 1) ||
                (w.Year == currentWeek.Year - 1 && currentWeek.WeekNumber == 1 && w.WeekNumber == 52))
            .OrderByDescending(w => w.Year)
            .ThenByDescending(w => w.WeekNumber)
            .FirstOrDefaultAsync();

        var nextWeek = await _context.Weeks
            .Where(w =>
                (w.Year == currentWeek.Year && w.WeekNumber == currentWeek.WeekNumber + 1) ||
                (w.Year == currentWeek.Year + 1 && currentWeek.WeekNumber == 52 && w.WeekNumber == 1))
            .OrderBy(w => w.Year)
            .ThenBy(w => w.WeekNumber)
            .FirstOrDefaultAsync();

        var viewModel = new PrognosesViewModel
        {
            Prognose = currentWeek,
            Weeks = prognoses,
            WeekId = currentWeek.Id,
            PreviousWeekId = previousWeek?.Id,
            NextWeekId = nextWeek?.Id,
            CurrentWeekNumber = currentWeekNumber,
            IsCurrentWeek = (currentWeek.Year == currentYear && currentWeek.WeekNumber == currentWeekNumber)
        };

        return View(viewModel);
    }

    private async Task<Week> GetCurrentWeek(int? id)
    {
        var culture = CultureInfo.CurrentCulture;
        var today = DateTime.Now;
        var currentYear = (short)today.Year;
        var currentWeekNumber = (byte)culture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        var currentWeek = await _context.Weeks
            .Include(w => w.Shifts)
            .ThenInclude(s => s.Employee)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (currentWeek == null)
        {
            currentWeek = await _context.Weeks
                .Include(w => w.Shifts)
                .ThenInclude(s => s.Employee)
                .FirstOrDefaultAsync(w => w.Year == currentYear && w.WeekNumber == currentWeekNumber);

            if (currentWeek == null) return null;
        }

        return currentWeek;
    }

    // GET: Prognoses/Aanmaken
    [HttpGet("Aanmaken")]
    public IActionResult Create()
    {
        CultureInfo ci = CultureInfo.CurrentCulture;
        Calendar calendar = ci.Calendar;

        // Create prognosis for the next week
        DateTime nextWeek = DateTime.Now.AddDays(7);
        short year = (short)nextWeek.Year;
        byte week = (byte)calendar.GetWeekOfYear(nextWeek, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);

        // If a prognosis already exists for this week, add another week
        while (_context.Weeks.Any(p => p.Year == year && p.WeekNumber == week))
        {
            nextWeek = nextWeek.AddDays(7);
            year = (short)nextWeek.Year;
            week = (byte)calendar.GetWeekOfYear(nextWeek, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
        }

        Week newWeek = new Week()
        {
            Year = year,
            WeekNumber = week,
        };

        // Fetch the latest completed week's visitor estimates
        Week? previousWeek = _context.Weeks
            .Include(w => w.PrognosisDays)
            .OrderByDescending(w => w.Year)
            .ThenByDescending(w => w.WeekNumber)
            .FirstOrDefault(w => w.Year < year || (w.Year == year && w.WeekNumber < week));

        // Fill holidays, weather, and other factors, and set visitor estimates based on the previous week
        for (byte i = 0; i < 7; i++)
        {
            PrognosisDay prognosisDay = new PrognosisDay()
            {
                Weekday = i,
                VisitorEstimate = previousWeek?.PrognosisDays.FirstOrDefault(d => d.Weekday == i)?.VisitorEstimate ?? 0, // Use the previous week's value or default to 0
            };

            // Get the current day for the new week
            DateTime startOfYear = new DateTime(newWeek.Year, 1, 1);
            DateTime currentDay = calendar.AddWeeks(startOfYear, newWeek.WeekNumber - 1)
                .AddDays(i - (int)startOfYear.DayOfWeek + 1);

            var holidays = _context.HolidayDays
                .Where(d => d.Date == DateOnly.FromDateTime(currentDay))
                .ToList();

            prognosisDay.Factors.Add(new Factor()
            {
                Type = "Feestdagen",
                Weekday = prognosisDay.Weekday,
                Impact = (short)(holidays.Count == 0 ? 0 : holidays.First().Impact),
            });

            prognosisDay.Factors.Add(new Factor()
            {
                Type = "Weer",
                Weekday = prognosisDay.Weekday,
                Impact = 3, // Default weather impact
            });

            prognosisDay.Factors.Add(new Factor()
            {
                Type = "Overig",
                Weekday = prognosisDay.Weekday,
                Impact = 0, // Default other impact
            });

            newWeek.PrognosisDays.Add(prognosisDay);
        }

        IEnumerable<Week> prognoses = _context.Weeks
            .Include(p => p.PrognosisDays)
            .OrderBy(x => x.Year)
            .ThenBy(x => x.WeekNumber)
            .ToList();

        CreatePrognosisViewModel CreatePrognosisViewModel = new CreatePrognosisViewModel()
        {
            Prognosis = newWeek,
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
}
