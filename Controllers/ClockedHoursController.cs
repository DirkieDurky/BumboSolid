using BumboSolid.Data.Models;
using BumboSolid.Data;
using BumboSolid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BumboSolid.Migrations;

namespace BumboSolid.Controllers
{
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
			int userId = user.Id;

			int year = DateTime.Now.Year;
			int weekNr = new CultureInfo("en-US").Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
			DateOnly startDate = FirstDateOfWeek(year, weekNr);

			var clockedHours = await _context.ClockedHours
				.Where(ch => ch.Employee == user)
				.OrderByDescending(ch => ch.Id)
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

			ClockedHoursViewModel clockedHoursViewModel = new ClockedHoursViewModel()
			{
				StartDate = startDate,
				EndDate = startDate.AddDays(6),
				ClockedHours = clockedHours,
				WeekdayDictionary = weekdayDictionary,
				Departments = departments,
				SelectedDepartments = new List<string> { departments.FirstOrDefault()?.Name }
			};

			return View(clockedHoursViewModel);
		}

		[HttpPost("Inklokken")]
		public async Task<IActionResult> ClockIn(List<string> selectedDepartments)
		{
			var user = await _userManager.GetUserAsync(User);
			int userId = user.Id;

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

		[HttpPost("Uitklokken")]
		public async Task<IActionResult> ClockOut()
		{
			var user = await _userManager.GetUserAsync(User);
			int userId = user.Id;

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

			if (currentClockedHour.EndTime != TimeOnly.MinValue)
			{
				ModelState.AddModelError(string.Empty, "Deze dienst is al uit geklokt.");
				return RedirectToAction("Index");
			}

			currentClockedHour.EndTime = TimeOnly.FromDateTime(DateTime.Now);

			_context.ClockedHours.Update(currentClockedHour);
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

		String Weekday(int year, int week, int day)
		{
			var jan1 = new DateOnly(year, 1, 1);
			var firstDayOfWeek = jan1.AddDays((week - 1) * 7 - (int)jan1.DayOfWeek + (int)DayOfWeek.Monday);

			if (firstDayOfWeek.Year < year) firstDayOfWeek = firstDayOfWeek.AddDays(7);

			return firstDayOfWeek.AddDays(day).DayOfWeek.ToString();
		}
	}
}