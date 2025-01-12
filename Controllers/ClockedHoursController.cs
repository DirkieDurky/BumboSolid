using BumboSolid.Data.Models;
using BumboSolid.Data;
using BumboSolid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace BumboSolid.Controllers
{
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
            int weekNr = new CultureInfo("en-US").Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

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
                { 1, "Maandag" },
                { 2, "Dinsdag" },
                { 3, "Woensdag" },
                { 4, "Donderdag" },
                { 5, "Vrijdag" },
                { 6, "Zaterdag" },
                { 7, "Zondag" }
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
            };

            return View(clockedHoursViewModel);
        }

        [HttpPost("Inklokken")]
        public async Task<IActionResult> ClockIn(List<string> selectedDepartments)
        {
            var user = await _userManager.GetUserAsync(User);
            int userId = user!.Id;

            //Clock out old clockedHour
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
            int weekNr = new CultureInfo("en-US").Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            DateOnly startDate = FirstDateOfWeek(year, weekNr);

            if (selectedDepartments == null || !selectedDepartments.Any())
            {
                ModelState.AddModelError(string.Empty, "Kies eerst een afdeling.");
                return View();
            }

            var departments = await _context.Departments
                .Where(d => selectedDepartments.Contains(d.Name))
                .ToListAsync();

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

            var lastClockedHours = await _context.ClockedHours
                .Where(ch => ch.EmployeeId == userId)
                .OrderByDescending(ch => ch.Id)
                .ThenByDescending(ch => ch.Weekday)
                .ThenByDescending(ch => ch.StartTime)
                .FirstOrDefaultAsync();

            if (lastClockedHours != null && lastClockedHours.EndTime == TimeOnly.MinValue)
            {
                lastClockedHours.EndTime = TimeOnly.FromDateTime(DateTime.Now);
                _context.ClockedHours.Update(lastClockedHours);
                await _context.SaveChangesAsync();
            }

            var newClockedHour = new ClockedHours
            {
                WeekId = currentWeek.Id,
                Weekday = (byte)DateTime.Now.DayOfWeek,
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

        [HttpGet("Overzicht/{weekId:int}")]
        public async Task<IActionResult> Overview(int? weekId)
        {
            var user = await _userManager.GetUserAsync(User);
            int userId = user!.Id;

            if (weekId == null)
            {
                CultureInfo ci = new CultureInfo("nl-NL");
                Calendar calendar = ci.Calendar;

                short year = (short)DateTime.Now.Year;
                int weekNumber = (byte)calendar.GetWeekOfYear(DateTime.Now, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
                weekId = _context.Weeks.First(w => w.Year == year && w.WeekNumber == weekNumber).Id;
            }

            var week = _context.Weeks.FirstOrDefault(week => week.Id == weekId);

            if (week == null)
            {
                ModelState.AddModelError(string.Empty, "Geselecteerde week niet gevonden");
                return RedirectToAction("Index", "Schedule");
            }

            var allClockedHours = await _context.ClockedHours
                .Where(ch => ch.EmployeeId == userId && ch.WeekId == weekId)
                .OrderByDescending(ch => ch.WeekId)
                .ThenByDescending(ch => ch.Weekday)
                .ThenByDescending(ch => ch.StartTime)
                .ToListAsync();

            var weekdayDictionary = new Dictionary<byte, string>
            {
                { 1, "Maandag" },
                { 2, "Dinsdag" },
                { 3, "Woensdag" },
                { 4, "Donderdag" },
                { 5, "Vrijdag" },
                { 6, "Zaterdag" },
                { 7, "Zondag" }
            };

            DateOnly startDate = FirstDateOfWeek(week.Year, week.WeekNumber);
            ClockedHoursOverviewViewModel overviewViewModel = new ClockedHoursOverviewViewModel
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(6),
                ClockedHours = allClockedHours,
                WeekdayDictionary = weekdayDictionary,
                WeekId = (int)weekId,
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
                Weekday = (byte)DateTime.Now.DayOfWeek,
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
                Weekday = (byte)DateTime.Now.DayOfWeek,
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

            if (firstDayOfWeek.Year < year) firstDayOfWeek = firstDayOfWeek.AddDays(7);

            return firstDayOfWeek;
        }
    }
}