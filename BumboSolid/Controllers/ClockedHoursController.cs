using BumboSolid.Data.Models;
using BumboSolid.Data;
using BumboSolid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace BumboSolid.Controllers;

[Authorize(Roles = "Employee")]
[Route("Uren klokken")]
public class ClockedHoursController : Controller
{
    private readonly BumboDbContext _context;
    private readonly UserManager<User> _userManager;

    public ClockedHoursController(BumboDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: ScheduleEmployeeController
    [HttpGet("Klokken")]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        int userId = user!.Id;

        int year = DateTime.Now.Year;
        int weekNr = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

        var currentWeek = await _context.Weeks
            .Where(w => w.Year == year && w.WeekNumber == weekNr)
            .FirstOrDefaultAsync();

        if (currentWeek == null)
        {
            currentWeek = new Week
            {
                Year = (short)year,
                WeekNumber = (byte)weekNr,
            };

            _context.Weeks.Add(currentWeek);
            await _context.SaveChangesAsync();
        }

        int currentWeekId = currentWeek.Id;

        var clockedHours = await _context.ClockedHours
            .Where(ch => ch.EmployeeId == userId && ch.WeekId == currentWeekId)
            .OrderByDescending(ch => ch.Weekday)
            .ThenByDescending(ch => ch.StartTime)
            .ToListAsync();

        var weekdayDictionary = new Dictionary<byte, string>
        {
            { 0, "Maandag" },
            { 1, "Dinsdag" },
            { 2, "Woensdag" },
            { 3, "Donderdag" },
            { 4, "Vrijdag" },
            { 5, "Zaterdag" },
            { 6, "Zondag" }
        };

        var departments = await _context.Departments.ToListAsync();

        var lastClockedHour = await _context.ClockedHours
            .Where(ch => ch.EmployeeId == userId)
            .OrderByDescending(ch => ch.Id)
            .ThenByDescending(ch => ch.Weekday)
            .ThenByDescending(ch => ch.StartTime).FirstOrDefaultAsync();

        var lastDepartment = lastClockedHour == null ? null : await _context.Departments
            .Where(d => d.Name == lastClockedHour.Department)
            .FirstOrDefaultAsync();

        ClockedHoursViewModel clockedHoursViewModel = new ClockedHoursViewModel()
        {
            StartDate = FirstDateOfWeek(year, weekNr),
            EndDate = FirstDateOfWeek(year, weekNr).AddDays(6),
            ClockedHours = clockedHours,
            WeekdayDictionary = weekdayDictionary,
            Departments = departments,
            SelectedDepartments = new List<string> { departments.FirstOrDefault()?.Name },
            LastDepartment = lastDepartment,
            WeekId = currentWeekId,
        };

        return View(clockedHoursViewModel);
    }

    [HttpPost("Inklokken")]
    public async Task<IActionResult> ClockIn(List<string> selectedDepartments)
    {
        var user = await _userManager.GetUserAsync(User);
        int userId = user!.Id;

        // Clock out old clockedHour
        var currentClockedHour = await _context.ClockedHours
            .Where(ch => ch.EmployeeId == userId)
            .OrderByDescending(ch => ch.Id)
            .ThenByDescending(ch => ch.Weekday)
            .ThenByDescending(ch => ch.StartTime).FirstOrDefaultAsync();

        if (currentClockedHour != null && currentClockedHour.EndTime == null)
        {
            currentClockedHour.EndTime = TimeOnly.FromDateTime(DateTime.Now);
            _context.ClockedHours.Update(currentClockedHour);
        }

        int year = DateTime.Now.Year;
        int weekNr = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

        var departments = await _context.Departments
            .Where(d => selectedDepartments.Contains(d.Name))
            .ToListAsync();

        // Check if the employee is allowed to work the given department
        if (selectedDepartments == null || !selectedDepartments.Any()) ModelState.AddModelError(string.Empty, "Kies eerst een afdeling.");
        User employee = await _context.Employees.Where(e => e.Id == userId).Include(e => e.Departments).FirstOrDefaultAsync();
        bool validDepartment = false;
        foreach (var department in employee.Departments) if (department.Name.Equals(departments.FirstOrDefault()?.Name)) validDepartment = true;
        if (validDepartment == false) ModelState.AddModelError("", "De medewerker mag niet werken bij deze afdeling");

        var currentWeek = await _context.Weeks.Where(w => w.Year == year && w.WeekNumber == weekNr).FirstOrDefaultAsync();
        if (currentWeek == null)
        {
            currentWeek = new Week()
            {
                Year = (short)year,
                WeekNumber = (byte)weekNr,
            };
            _context.Add(currentWeek);
            _context.SaveChanges();
        }

        var newClockedHour = new ClockedHours
        {
            WeekId = currentWeek.Id,
            Weekday = ConvertDayOfWeekToStartOnMonday(DateTime.Now.DayOfWeek),
            Department = departments.FirstOrDefault()?.Name,
            StartTime = TimeOnly.FromDateTime(DateTime.Now),
            EmployeeId = userId,
            IsBreak = 0
        };

        _context.ClockedHours.Add(newClockedHour);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet("Uitklokken")]
    public async Task<IActionResult> ClockOut()
    {
        var user = await _userManager.GetUserAsync(User);
        int userId = user!.Id;

        var currentClockedHour = await _context.ClockedHours
            .Where(ch => ch.EmployeeId == userId)
            .OrderByDescending(ch => ch.Id)
            .ThenByDescending(ch => ch.Weekday)
            .ThenByDescending(ch => ch.StartTime).FirstOrDefaultAsync();

        if (currentClockedHour == null)
        {
            ModelState.AddModelError(string.Empty, "Geen actieve dienst gevonden om uit te klokken.");
            return RedirectToAction("Index");
        }

        if (currentClockedHour.EndTime != null)
        {
            ModelState.AddModelError(string.Empty, "Deze dienst is al uit geklokt.");
            return RedirectToAction("Index");
        }

        currentClockedHour.EndTime = TimeOnly.FromDateTime(DateTime.Now);

        _context.ClockedHours.Update(currentClockedHour);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet("Overzicht/{year:int?}/{weekNumber:int?}")]
    public async Task<IActionResult> Overview(int? weekId)
    {
        var user = await _userManager.GetUserAsync(User);
        int userId = user!.Id;
        var currentWeek = await GetCurrentWeek(weekId);

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

        var startDate = FirstDateOfWeek(currentWeek.Year, currentWeek.WeekNumber);

        var allClockedHours = await _context.ClockedHours
        .Where(ch => ch.EmployeeId == userId && ch.WeekId == weekId)
        .OrderByDescending(ch => ch.WeekId)
        .ThenByDescending(ch => ch.Weekday)
        .ThenByDescending(ch => ch.StartTime)
        .ToListAsync();

        var weekdayDictionary = new Dictionary<byte, string>
        {
        { 0, "Maandag" },
        { 1, "Dinsdag" },
        { 2, "Woensdag" },
        { 3, "Donderdag" },
        { 4, "Vrijdag" },
        { 5, "Zaterdag" },
        { 6, "Zondag" }
        };

        ClockedHoursOverviewViewModel overviewViewModel = new ClockedHoursOverviewViewModel
        {
            StartDate = startDate,
            EndDate = startDate.AddDays(6),
            ClockedHours = allClockedHours,
            WeekdayDictionary = weekdayDictionary,
            WeekId = currentWeek.Id,
            PreviousWeekId = previousWeek?.Id,
            NextWeekId = nextWeek?.Id,
            IsCurrentWeek = (currentWeek.Year == currentYear && currentWeek.WeekNumber == currentWeekNumber),
        };

        return View(overviewViewModel);
    }

    [HttpGet("Pauzeren")]
    public async Task<IActionResult> Pause()
    {
        var user = await _userManager.GetUserAsync(User);
        int userId = user!.Id;

        var currentClockedHour = await _context.ClockedHours
            .Where(ch => ch.EmployeeId == userId)
            .OrderByDescending(ch => ch.Id)
            .ThenByDescending(ch => ch.Weekday)
            .ThenByDescending(ch => ch.StartTime)
            .FirstOrDefaultAsync();

        if (currentClockedHour == null)
        {
            ModelState.AddModelError(string.Empty, "Geen actieve dienst gevonden om uit te klokken.");
            return RedirectToAction("Index");
        }

        if (currentClockedHour.EndTime != null)
        {
            ModelState.AddModelError(string.Empty, "Deze dienst is al uit geklokt.");
            return RedirectToAction("Index");
        }

        currentClockedHour.EndTime = TimeOnly.FromDateTime(DateTime.Now);

        _context.ClockedHours.Update(currentClockedHour);

        var newClockedHour = new ClockedHours
        {
            WeekId = currentClockedHour.WeekId,
            Weekday = ConvertDayOfWeekToStartOnMonday(DateTime.Now.DayOfWeek),
            Department = currentClockedHour.Department,
            StartTime = TimeOnly.FromDateTime(DateTime.Now),
            EmployeeId = userId,
            IsBreak = 1
        };

        _context.ClockedHours.Add(newClockedHour);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet("Hervatten")]
    public async Task<IActionResult> Unpause()
    {
        var user = await _userManager.GetUserAsync(User);
        int userId = user!.Id;

        var pauseEntry = await _context.ClockedHours
            .Where(ch => ch.EmployeeId == userId && ch.IsBreak == 1)
            .OrderByDescending(ch => ch.Id)
            .ThenByDescending(ch => ch.Weekday)
            .ThenByDescending(ch => ch.StartTime)
            .FirstOrDefaultAsync();

        if (pauseEntry == null)
        {
            ModelState.AddModelError(string.Empty, "Geen actieve dienst gevonden om uit te klokken.");
            return RedirectToAction("Index");
        }

        if (pauseEntry.EndTime != null)
        {
            ModelState.AddModelError(string.Empty, "De pauze is al gemarkeerd als voorbij");
            return RedirectToAction("Index");
        }

        pauseEntry.EndTime = TimeOnly.FromDateTime(DateTime.Now);

        _context.ClockedHours.Update(pauseEntry);

        var lastClockedHour = await _context.ClockedHours
            .Where(ch => ch.EmployeeId == userId && ch.IsBreak == 0)
            .OrderByDescending(ch => ch.Id)
            .ThenByDescending(ch => ch.Weekday)
            .ThenByDescending(ch => ch.StartTime)
            .FirstOrDefaultAsync();

        if (lastClockedHour == null)
        {
            ModelState.AddModelError(string.Empty, "Geen actieve dienst gevonden om te hervatten");
            return RedirectToAction("Index");
        }

        var newClockedHour = new ClockedHours
        {
            WeekId = lastClockedHour.WeekId,
            Weekday = ConvertDayOfWeekToStartOnMonday(DateTime.Now.DayOfWeek),
            Department = lastClockedHour.Department,
            StartTime = TimeOnly.FromDateTime(DateTime.Now),
            EmployeeId = userId,
            IsBreak = 0
        };

        _context.ClockedHours.Add(newClockedHour);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    DateOnly FirstDateOfWeek(int year, int week)
    {
        var jan1 = new DateOnly(year, 1, 1);
        var firstDayOfWeek = jan1.AddDays((week - 1) * 7 - (int)jan1.DayOfWeek + (int)DayOfWeek.Monday);

        return firstDayOfWeek;
    }

    byte ConvertDayOfWeekToStartOnMonday(DayOfWeek dayOfWeek)
    {
        dayOfWeek -= 1;
        return (byte)(dayOfWeek < 0 ? 6 : (byte)dayOfWeek);
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
}