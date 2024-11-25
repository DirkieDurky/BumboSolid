using System.Globalization;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc;

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
		public async Task<IActionResult> Index(int Year, int WeekNr, Employee Employee)
		{
			// This is to be deleted when the agenda has been inplemented
			Year = 2024;
			WeekNr = 47;
			Employee = _context.Employees.ToArray()[0];

			DateOnly startDate = FirstDateOfWeek(Year, WeekNr);

			List<AvailabilityRuleViewModel> availabilityViewModels = new List<AvailabilityRuleViewModel>();

			// Create list of availabilityRules
            foreach (AvailabilityRule availabilityRule in _context.AvailabilityRules.Where(ar => ar.Date >= startDate && ar.Date < startDate.AddDays(7)).ToList()) // Hier moet alleen de availabilityrules van de jusite medewerker en de geselceteerde week worden meegegeven
            {
				String Day = availabilityRule.Date.DayOfWeek.ToString();

				AvailabilityRuleViewModel availabilityViewModel = new AvailabilityRuleViewModel() {
					Day = Day,

					StartTime = availabilityRule.StartTime,
					EndTime = availabilityRule.EndTime,

					Available = Convert.ToBoolean(availabilityRule.Available),
					School = Convert.ToBoolean(availabilityRule.School)
				};

                availabilityViewModels.Add(availabilityViewModel);
            }

			ViewBag.year = Year;
			ViewBag.weekNr = WeekNr;
			ViewBag.employeeId = Employee.AspNetUserId;

			return View(availabilityViewModels);
        }

        // GET: AvailiabilityController/Create
        public async Task<IActionResult> Create(int Year, int WeekNr, int EmployeeId)
        {
			ViewBag.year = Year;
			ViewBag.weekNr = WeekNr;
			ViewBag.employeeId = EmployeeId;

			return View(new AvailabilityRuleViewModel());
        }

		// Post: AvailiabilityController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
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
				return RedirectToAction(nameof(Index));
			}

			return View(availabilityRuleViewModel);
		}

		// GET: AvailiabilityController/Edit
		public async Task<IActionResult> Edit(int availabilityId, int Year, int WeekNr)
		{
			if (availabilityId == null)
			{
				return NotFound();
			}

			var norm = await _context.Norms.FindAsync(availabilityId);
			if (norm == null)
			{
				return NotFound();
			}

			ViewBag.year = Year;
			ViewBag.weekNr = WeekNr;
			ViewBag.employeeId = EmployeeId;

			return View(new AvailabilityRuleViewModel());
		}

		// Post: AvailiabilityController/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(AvailabilityRuleViewModel availabilityRuleViewModel, int Year, int WeekNr, int EmployeeId, string Availability)
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
				return RedirectToAction(nameof(Index));
			}

			return View(availabilityRuleViewModel);
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
