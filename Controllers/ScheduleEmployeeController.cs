using System.Globalization;
using System;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Identity;
using System.Runtime.Intrinsics.Arm;
using System.Linq;

namespace BumboSolid.Controllers
{
    [Authorize(Roles = "Employee")]
    [Route("RoosterMedewerker")]
	public class ScheduleEmployeeController : Controller
	{
		private readonly BumboDbContext _context;
		private readonly UserManager<User> _userManager;

		public ScheduleEmployeeController(BumboDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

        // GET: ScheduleEmployeeController
        [HttpGet("")]
		public async Task<IActionResult> Schedule(int weekFromNow)
		{
            // Getting user id
            var user = await _userManager.GetUserAsync(User);
            int userId = user.Id;

			// Getting correct date
			int year = DateTime.Now.Year;
			int weekNr = new CultureInfo("en-US").Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday) + weekFromNow;
			DateOnly startDate = FirstDateOfWeek(year, weekNr);

			// Getting shifts
			List<ShiftViewModel> shifts = new List<ShiftViewModel>();
			foreach (var shift in await _context.Shifts.Where(s => s.Employee == user && s.Week.Year == year && s.Week.WeekNumber == weekNr).ToListAsync())
			{
				shifts.Add(new ShiftViewModel()
				{
					Id = shift.Id,

                    Weekday = Weekday(year, weekNr, shift.Weekday),
					StartTime = shift.StartTime,
					EndTime = shift.EndTime,

					Department = shift.Department
				});
			}

			EmployeeScheduleViewModel employeeScheduleViewModel = new EmployeeScheduleViewModel()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,

				StartDate = startDate,
				EndDate = startDate.AddDays(6),
                WeekFromNoW = weekFromNow,

                Shifts = shifts
            };

            return View(employeeScheduleViewModel);
		}

        // GET: ScheduleEmployeeController/OutgoingFillRequests
        [HttpGet("Uitgaande invalsverzoeken")]
		public async Task<IActionResult> OutgoingFillRequests()
		{
			// Getting user id
			var user = await _userManager.GetUserAsync(User);
			int userId = user.Id;

			var fillRequests = await _context.FillRequests.Where(s => _context.Shifts.Any(i => i.Id == s.ShiftId && i.EmployeeId == userId)).ToListAsync();
            List<FillRequestViewModel> fillRequestViewModels = new List<FillRequestViewModel>();

			foreach (FillRequest fillRequest in fillRequests)
            {
				// Getting Shift and Week
				var shift = _context.Shifts.FirstOrDefault(i => i.Id == fillRequest.ShiftId);
				var week = _context.Weeks.FirstOrDefault(i => i.Id == shift.WeekId);

				// Getting correct date and day
				var jan1 = new DateOnly(week.Year, 1, 1);
				DateOnly date = jan1.AddDays(DayOfWeek.Monday - jan1.DayOfWeek).AddDays((shift.Week.WeekNumber - 1) * 7).AddDays((int)shift.Weekday - (int)DayOfWeek.Monday);

				string[] days = ["Monday", "Tuesdday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

				// Creating FillReuestViewModel
				FillRequestViewModel fillRequestViewModel = new FillRequestViewModel()
				{
					Date = date,
					Day = days[shift.Weekday],
					StartTime = shift.StartTime,
					EndTime = shift.EndTime,

					Department = shift.Department,
					Status = fillRequest.Accepted == 0 ? "Open" : "Geaccepteerd"
				};

				fillRequestViewModels.Add(fillRequestViewModel);
			};

			return View(fillRequestViewModels);
        }

        // GET: ScheduleEmployeeController/IncomingFillRequests
        [HttpGet("Inkomende invalsverzoeken")]
        public async Task<IActionResult> IncomingFillRequests()
        {
			// Getting user id
			var user = await _userManager.GetUserAsync(User);
			int userId = user.Id;

			var fillRequests = await _context.FillRequests.Where(s => _context.Shifts.Any(i => i.Id == s.ShiftId && i.EmployeeId != userId)).ToListAsync();
			List<FillRequestViewModel> fillRequestViewModels = new List<FillRequestViewModel>();

			foreach (FillRequest fillRequest in fillRequests)
			{
				// Getting Shift and Week
				var shift = _context.Shifts.FirstOrDefault(i => i.Id == fillRequest.ShiftId);
				var week = _context.Weeks.FirstOrDefault(i => i.Id == shift.WeekId);

				// Getting correct date and day
				var jan1 = new DateOnly(week.Year, 1, 1);
				DateOnly date = jan1.AddDays(DayOfWeek.Monday - jan1.DayOfWeek).AddDays((shift.Week.WeekNumber - 1) * 7).AddDays((int)shift.Weekday - (int)DayOfWeek.Monday);

				string[] days = ["Monday", "Tuesdday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

				// Checking if this FillRequest does not overlap with an already existing shift
				var shifts = _context.Shifts.Where(s => s.EmployeeId == userId && s.WeekId == week.Id && s.Weekday == shift.Weekday).ToList();
				var validShift = true;
				foreach (Shift yourShift in shifts)
				{
					// shift ends during || shift starts earlier but ends later
					if (yourShift.StartTime <= shift.StartTime && yourShift.EndTime >= shift.StartTime) validShift = false;
					// shift starts during || shift starts later but ends earlier
					if (yourShift.StartTime >= shift.StartTime && yourShift.StartTime <= shift.EndTime) validShift = false;
				}

				// Checking if this shift does not break any CAO rules
				var userAge = DateTime.Today.Year - user.BirthDate.Year;
				var CLAs = _context.CLAEntries.Where(a => a.AgeStart <= userAge && a.AgeEnd >= userAge).ToList();

				foreach(CLAEntry CLA in CLAs)
				{
					// Shift duration
					if ((shift.EndTime - shift.StartTime).TotalMinutes > CLA.MaxShiftDuration) validShift = false;

					// Average works hours over a span of 4 weeks
					var lastFourWeeksShifts = _context.Shifts.Where(s => s.EmployeeId == userId && shift.Week.WeekNumber - s.Week.WeekNumber < 3 && s.Week.Year == shift.Week.Year).ToList();
					var lastFourWeeksTotalMinutes = (shift.EndTime - shift.StartTime).Minutes;
					foreach(Shift pastShift in lastFourWeeksShifts) lastFourWeeksTotalMinutes = lastFourWeeksTotalMinutes + (pastShift.EndTime - pastShift.StartTime).Minutes;
					if (lastFourWeeksTotalMinutes > CLA.MaxAvgWeeklyWorkDurationOverFourWeeks) validShift = false;

					// Latest work time
					if (shift.EndTime > CLA.LatestWorkTime) validShift = false;

					// Earliest work time
					if (shift.StartTime < CLA.EarliestWorkTime) validShift = false;

					// Max work duration per week
					var thisWeekShifts = _context.Shifts.Where(s => s.EmployeeId == userId && shift.Week.WeekNumber == s.Week.WeekNumber && s.Week.Year == shift.Week.Year).ToList();
					var thisWeekTotalMinutes = (shift.EndTime - shift.StartTime).Minutes;
					foreach (Shift pastShift in thisWeekShifts) thisWeekTotalMinutes = thisWeekTotalMinutes + (pastShift.EndTime - pastShift.StartTime).Minutes;
					if (thisWeekTotalMinutes > CLA.MaxWorkDurationPerWeek) validShift = false;

					// Max work days per week
					List<int> workDays = new List<int>();
					workDays.Add(shift.Weekday);
					foreach (Shift pastShift in thisWeekShifts) if (workDays.Contains(pastShift.Weekday) == false) workDays.Add(pastShift.Weekday);
					if (workDays.Count >= CLA.MaxWorkDaysPerWeek) validShift = false;

					// Max work duration per day
					var todayShifts = _context.Shifts.Where(s => s.EmployeeId == userId && shift.Weekday == s.Weekday && shift.Week.WeekNumber == s.Week.WeekNumber && s.Week.Year == shift.Week.Year).ToList();
					var todayTotalMinutes = (shift.EndTime - shift.StartTime).Minutes;
					foreach (Shift pastShift in todayShifts) todayTotalMinutes = todayTotalMinutes + (pastShift.EndTime - pastShift.StartTime).Minutes;
					if (todayTotalMinutes > CLA.MaxWorkDurationPerDay) validShift = false;
				}

				// Getting shift user (TODO external employee makes fill request might crash)
				//var shiftUser = _context.Users.Where(i => i.Id == shift.Id).FirstOrDefault();
				var shiftUser = _context.Users.Where(i => i.Id == shift.EmployeeId).FirstOrDefault();

				if (validShift == true)
				{
					// Creating FillReuestViewModel
					FillRequestViewModel fillRequestViewModel = new FillRequestViewModel()
					{
						Date = date,
						Day = days[shift.Weekday],
						StartTime = shift.StartTime,
						EndTime = shift.EndTime,

						Department = shift.Department,
						Name = shiftUser.FirstName + " " + shiftUser.LastName
					};

					fillRequestViewModels.Add(fillRequestViewModel);
				}
			};

			return View(fillRequestViewModels);
		}

		// GET: ScheduleEmployeeController/FillRequest/5
		[HttpGet("Invalsverzoek versturen")]
		public ActionResult FillRequest(int id)
		{
			var shift = _context.Shifts.FirstOrDefault(s => s.Id == id);
			if (shift == null) return NotFound();

			return View(shift);
		}

		// POST: ScheduleEmployeeController/FillRequest/5
		[ValidateAntiForgeryToken]
		[HttpPost("Invalsverzoek versturen")]
		public ActionResult FillRequestConfirmed(int id)
		{
			var shift = _context.Shifts.FirstOrDefault(s => s.Id == id);
			if (shift == null) return NotFound();

			// Check if there is not already an open FillRequest for this Shift
			var fillRequests = _context.FillRequests.Where(s => s.ShiftId == id).ToList();
			foreach (FillRequest request in fillRequests) if (request.Accepted == 0) return RedirectToAction(nameof(Schedule));

			FillRequest fillRequest = new FillRequest()
			{
				ShiftId = id,
				Accepted = 0
			};

			if (ModelState.IsValid)
			{
				_context.FillRequests.Add(fillRequest);
				_context.SaveChanges();
				return RedirectToAction(nameof(Schedule));
			}

			return RedirectToAction(nameof(Schedule));
		}

        // Get the date of the first day of the week
        DateOnly FirstDateOfWeek(int year, int week)
        {
            var jan1 = new DateOnly(year, 1, 1);
            var firstDayOfWeek = jan1.AddDays((week - 1) * 7 - (int)jan1.DayOfWeek + (int)DayOfWeek.Monday);

            if (firstDayOfWeek.Year < year) firstDayOfWeek = firstDayOfWeek.AddDays(7);

            return firstDayOfWeek;
        }

        // Get the weekday of the given year, week and day
        String Weekday(int year, int week, int day)
        {
            var jan1 = new DateOnly(year, 1, 1);
            var firstDayOfWeek = jan1.AddDays((week - 1) * 7 - (int)jan1.DayOfWeek + (int)DayOfWeek.Monday);

            if (firstDayOfWeek.Year < year) firstDayOfWeek = firstDayOfWeek.AddDays(7);

            return firstDayOfWeek.AddDays(day).DayOfWeek.ToString();
        }
    }
}
