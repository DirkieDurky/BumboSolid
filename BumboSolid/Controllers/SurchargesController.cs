using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Globalization;

namespace BumboSolid.Controllers
{
	[Authorize(Roles = "Manager")]
	[Route("Toeslagen")]
	public class SurchargesController(BumboDbContext context) : Controller
	{
		private readonly BumboDbContext _context = context;

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

		private async Task<SchedulesSurchargesViewModel> GetSchedulesViewModel(Week currentWeek)
		{
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

			var departments = _context.Departments.ToList();

			return new SchedulesSurchargesViewModel
			{
				Weeks = await _context.Weeks
					.Include(w => w.Shifts)
					.ThenInclude(s => s.Employee)
					.Include(p => p.PrognosisDays)
					.ThenInclude(pd => pd.PrognosisDepartments)
					.OrderByDescending(w => w.Year)
					.ThenByDescending(w => w.WeekNumber)
					.ToListAsync(),
				WeekId = currentWeek.Id,
				PreviousWeekId = previousWeek?.Id,
				NextWeekId = nextWeek?.Id,
				IsCurrentWeek = (currentWeek.Year == currentYear && currentWeek.WeekNumber == currentWeekNumber),
				HasSchedule = currentWeek.HasSchedule != 0,
				Departments = departments
			};
		}

		[HttpGet("Toeslag/{id:int?}")]
		public async Task<IActionResult> Index(int? id)
		{
			var currentWeek = await GetCurrentWeek(id);

			if (currentWeek == null) return RedirectToAction(nameof(Create));

			var viewModel = await GetSchedulesViewModel(currentWeek);

			viewModel.Surcharges = await _context.CLASurchargeEntries.ToListAsync();
			viewModel.ClockedHours = await _context.ClockedHours.Where(clockedHours => clockedHours.WeekId == viewModel.WeekId)
				.Include(c => c.Employee)
				.ToListAsync();

			return View(viewModel);
		}
	}
}
