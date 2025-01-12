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
    [Authorize(Roles = "Manager")]
    [Route("Geklokte uren")]
    public class ClockedHoursManagerController : Controller
    {
        private readonly BumboDbContext _context;
        private readonly UserManager<User> _userManager;

        public ClockedHoursManagerController(BumboDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("Overzicht/{employeeId:int}/{year:int?}/{weekNumber:int?}")]
        public async Task<IActionResult> Overview(int employeeId, int? year, int? weekNumber)
        {
            if (year == null || weekNumber == null)
            {
                CultureInfo ci = new CultureInfo("nl-NL");
                Calendar calendar = ci.Calendar;

                year = (short)DateTime.Now.Year;
                weekNumber = (byte)calendar.GetWeekOfYear(DateTime.Now, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            }

            var employee = _context.Users.FirstOrDefault(u => u.Id == employeeId);
            string? employeeName = employee == null ? null : employee.Name;

            var week = _context.Weeks.FirstOrDefault(w => w.Year == year && w.WeekNumber == weekNumber);
            DateOnly startDate;
            if (week == null)
            {
                startDate = FirstDateOfWeek((int)year, (int)weekNumber);
                ClockedHoursManagerOverviewViewModel emptyOverviewViewModel = new ClockedHoursManagerOverviewViewModel
                {
                    StartDate = startDate,
                    EndDate = startDate.AddDays(6),
                    ClockedHours = new(),
                    WeekdayDictionary = new(),
                    Year = (int)year,
                    WeekNumber = (int)weekNumber,
                    EmployeeId = employeeId,
                    EmployeeName = employeeName,
                };

                return View(emptyOverviewViewModel);
            }

            var allClockedHours = await _context.ClockedHours
                .Where(ch => ch.EmployeeId == employeeId && ch.WeekId == week.Id)
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

            startDate = FirstDateOfWeek(week.Year, week.WeekNumber);
            ClockedHoursManagerOverviewViewModel overviewViewModel = new ClockedHoursManagerOverviewViewModel
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(6),
                ClockedHours = allClockedHours,
                WeekdayDictionary = weekdayDictionary,
                Year = (int)year,
                WeekNumber = (int)weekNumber,
                EmployeeId = employeeId,
                EmployeeName = employeeName,
            };

            return View(overviewViewModel);
        }

        DateOnly FirstDateOfWeek(int year, int week)
        {
            var jan1 = new DateOnly(year, 1, 1);
            var firstDayOfWeek = jan1.AddDays((week - 1) * 7 - (int)jan1.DayOfWeek + (int)DayOfWeek.Monday);

            return firstDayOfWeek;
        }
    }
}