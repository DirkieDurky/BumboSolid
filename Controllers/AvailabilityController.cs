using System.Globalization;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Controllers
{
	public class AvailabilityController : Controller
	{
		private readonly BumboDbContext _context;

		public AvailabilityController(BumboDbContext context)
		{
			_context = context;
		}

		// GET: AvailiabilityController/Index
		[HttpGet("Werkgelegenheden")]
		public async Task<IActionResult> Index(int Year, int WeekNr)
		{
			// This is to be deleted when the agenda has been inplemented
			Year = 2024;
			WeekNr = 47;
			int EmployeeId = 1;

			DateOnly startDate = FirstDateOfWeek(Year, WeekNr);

			List<AvailabilityRuleViewModel> availabilityViewModels = new List<AvailabilityRuleViewModel>();

			// Create list of availabilityRules
            foreach (AvailabilityRule availabilityRule in _context.AvailabilityRules.Where(ar => ar.Date >= startDate && ar.Date < startDate.AddDays(7)).ToList()) // Hier moet alleen de availabilityrules van de jusite medewerker en de geselceteerde week worden meegegeven
            {
				AvailabilityRuleViewModel availabilityViewModel = new AvailabilityRuleViewModel() {
					Id = availabilityRule.Id,
					Employee = availabilityRule.Employee,

					Day = availabilityRule.Date.DayOfWeek.ToString(),
					StartTime = availabilityRule.StartTime,
					EndTime = availabilityRule.EndTime,

					Available = Convert.ToBoolean(availabilityRule.Available),
					School = Convert.ToBoolean(availabilityRule.School)
				};

                availabilityViewModels.Add(availabilityViewModel);
            }

			ViewBag.year = Year;
			ViewBag.weekNr = WeekNr;
			ViewBag.employeeId = EmployeeId;

			return View(availabilityViewModels);
        }

		// GET: AvailiabilityController/Create
		[HttpGet("Aanmaken")]
		public async Task<IActionResult> Create(int Year, int WeekNr, int EmployeeId)
        {
			ViewBag.year = Year;
			ViewBag.weekNr = WeekNr;
			ViewBag.employeeId = EmployeeId;

			return View(new AvailabilityRuleViewModel());
        }

		// Post: AvailiabilityController/Create
		[ValidateAntiForgeryToken]
		[HttpPost("Aanmaken")]
		public async Task<IActionResult> Create(AvailabilityRuleViewModel availabilityRuleViewModel, int Year, int WeekNr, int EmployeeId, string Availability)
		{
			AvailabilityRule availabilityRule = new AvailabilityRule();

			ViewBag.year = Year;
			ViewBag.weekNr = WeekNr;
			ViewBag.employeeId = EmployeeId;

			// Making sure that EndTime is not before StartTime
			if (availabilityRuleViewModel.EndTime < availabilityRuleViewModel.StartTime)
			{
				ModelState.AddModelError("EndTime", "De eind tijd moet hetzelfde of later zijn dan de start tijd");
				return View(availabilityRuleViewModel);
			}

			// Convert string to DayOfWeek
			switch (availabilityRuleViewModel.Day)
			{
				case "Monday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr);
					break;
				case "Tuesday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(1);
					break;
				case "Wednesday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(2);
					break;
				case "Thursday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(3);
					break;
				case "Friday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(4);
					break;
				case "Saturday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(5);
					break;
				case "Sunday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(6);
					break;
			}

			// Convert Availability to Available or School
			switch (Availability)
			{
				case "Available":
					availabilityRule.Available = 1;
					break;
				case "School":
					availabilityRule.School = 1;
					break;
			}

			availabilityRule.Employee = EmployeeId;

			availabilityRule.StartTime = availabilityRuleViewModel.StartTime;
			availabilityRule.EndTime = availabilityRuleViewModel.EndTime;

            // Check if the model state is still valid before saving to the database
            if (ModelState.IsValid)
			{
				_context.AvailabilityRules.Add(availabilityRule);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(availabilityRuleViewModel);
		}

		// GET: AvailiabilityController/Edit
		[HttpGet("Bewerken/{AvailabilityId:int}")]
		public async Task<IActionResult> Edit(int AvailabilityId, int Year, int WeekNr)
		{
			if (AvailabilityId == null) return NotFound();

			var availabilityRule = await _context.AvailabilityRules.FindAsync(AvailabilityId);
			if (availabilityRule == null) return NotFound();

			// Apply data from availabilityRuile to ViewModel
			AvailabilityRuleViewModel availabilityViewModel = new AvailabilityRuleViewModel()
			{
                Id = availabilityRule.Id,

                Day = availabilityRule.Date.DayOfWeek.ToString(),
				StartTime = availabilityRule.StartTime,
				EndTime = availabilityRule.EndTime,

				Available = Convert.ToBoolean(availabilityRule.Available),
				School = Convert.ToBoolean(availabilityRule.School)
			};


			ViewBag.year = Year;
			ViewBag.weekNr = WeekNr;

			// Convert Available or School to Availability
			ViewBag.availability = "Unavailable";
			if (availabilityRule.Available == 1) ViewBag.availability = "Available";
			if (availabilityRule.School == 1) ViewBag.availability = "School";

			return View(availabilityViewModel);
		}

		// Post: AvailiabilityController/Edit
		[ValidateAntiForgeryToken]
		[HttpPost("Bewerken")]
		public async Task<IActionResult> Edit(AvailabilityRuleViewModel availabilityRuleViewModel, int Year, int WeekNr, string Availability)
		{
			ViewBag.year = Year;
			ViewBag.weekNr = WeekNr;
			ViewBag.availability = Availability;

			if (availabilityRuleViewModel.Id == null) return NotFound();

            var availabilityRule = await _context.AvailabilityRules.FindAsync(availabilityRuleViewModel.Id);
            if (availabilityRule == null) return NotFound();

			// Making sure that EndTime is not before StartTime
			if (availabilityRuleViewModel.EndTime < availabilityRuleViewModel.StartTime)
			{
				ModelState.AddModelError("EndTime", "De eind tijd moet hetzelfde of later zijn dan de start tijd");
				return View(availabilityRuleViewModel);
			}

			// Convert string to DayOfWeek
			switch (availabilityRuleViewModel.Day)
			{
				case "Monday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr);
					break;
				case "Tuesday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(1);
					break;
				case "Wednesday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(2);
					break;
				case "Thursday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(3);
					break;
				case "Friday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(4);
					break;
				case "Saturday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(5);
					break;
				case "Sunday":
					availabilityRule.Date = FirstDateOfWeek(Year, WeekNr).AddDays(6);
					break;
			}

            // Convert Availability to Available or School
			switch (Availability)
			{
				case "Available":
					availabilityRule.Available = 1;
					availabilityRule.School = 0;
					break;
				case "School":
					availabilityRule.Available = 0;
					availabilityRule.School = 1;
					break;
				default:
					availabilityRule.Available = 0;
					availabilityRule.School = 0;
					break;
			}

			availabilityRule.Employee = availabilityRule.Employee;
			availabilityRule.StartTime = availabilityRuleViewModel.StartTime;
			availabilityRule.EndTime = availabilityRuleViewModel.EndTime;

			// Check if the model state is still valid before saving to the database
			if (ModelState.IsValid)
			{
                _context.AvailabilityRules.Update(availabilityRule);
                _context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(availabilityRuleViewModel);
		}

		// GET: AvailiabilityController/Delete
		[HttpGet("Verwijderen/{AvailabilityId:int}")]
		public async Task<IActionResult> Delete(int AvailabilityId, int Year, int WeekNr)
		{
            if (AvailabilityId == null) return NotFound();

            var availabilityRule = await _context.AvailabilityRules.FindAsync(AvailabilityId);
            if (availabilityRule == null) return NotFound();

            ViewBag.year = Year;
            ViewBag.weekNr = WeekNr;

            return View(availabilityRule);
		}

		// POST: AvailiabilityController/Delete
		[ActionName("Verwijderen")]
		[ValidateAntiForgeryToken]
		[HttpPost("Verwijderen/{AvailabilityId:int}")]
		public async Task<IActionResult> DeleteConfirmed(int AvailabilityId, int Year, int WeekNr)
        {
            if (AvailabilityId == null) return NotFound();

            var availabilityRule = await _context.AvailabilityRules.FindAsync(AvailabilityId);

            if (availabilityRule != null)
			{
				_context.AvailabilityRules.Remove(availabilityRule);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}

		// Get the date of the first day of the week
		public static DateOnly FirstDateOfWeek(int Year, int WeekNr)
		{
			var firstOfJan = new DateTime(Year, 1, 1);
			var firstThursdayOfYear = firstOfJan.AddDays(DayOfWeek.Thursday - firstOfJan.DayOfWeek);
			var firstWeekOfYear = new GregorianCalendar().GetWeekOfYear(
				firstThursdayOfYear,
				CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday
			);

			if (firstWeekOfYear == 1) WeekNr -= 1;

			return DateOnly.FromDateTime(firstThursdayOfYear.AddDays(WeekNr * 7 - 3));
		}
	}
}
