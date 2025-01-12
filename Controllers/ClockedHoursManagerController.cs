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

        [HttpGet("Overzicht/{employeeId:int}/{weekId:int}")]
        public async Task<IActionResult> Overview(int employeeId, int weekId)
        {
            var allClockedHours = await _context.ClockedHours
                .Where(ch => ch.EmployeeId == employeeId && ch.WeekId == weekId)
                .OrderByDescending(ch => ch.WeekId)
                .ThenByDescending(ch => ch.Weekday)
                .ThenByDescending(ch => ch.StartTime)
                .ToListAsync();

            var week = _context.Weeks.FirstOrDefault(week=>week.Id == weekId);
            if (week == null)
            {
                ModelState.AddModelError(string.Empty, "Geselecteerde week niet gevonden");
                return RedirectToAction("Index", "Schedule");
            }

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
                WeekId = weekId,
            };

            return View(overviewViewModel);
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