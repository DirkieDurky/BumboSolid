using System.Globalization;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BumboSolid.HelperClasses.CLARules;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.Controllers;

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
    public async Task<IActionResult> EmployeeSchedule(int? id)
    {
        var user = await _userManager.GetUserAsync(User);
        int userId = user.Id;

        var currentWeek = await _context.Weeks
            .Include(w => w.Shifts)
            .ThenInclude(s => s.Employee)
            .FirstOrDefaultAsync(w => w.Id == id);

        var culture = CultureInfo.CurrentCulture;
        var today = DateTime.Now;
        var currentYear = (short)today.Year;
        var currentWeekNumber = (byte)culture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        if (currentWeek == null)
        {
            currentWeek = await _context.Weeks
                .Include(w => w.Shifts)
                .ThenInclude(s => s.Employee)
                .FirstOrDefaultAsync(w => w.Year == currentYear && w.WeekNumber == currentWeekNumber);

            if (currentWeek == null) return NotFound();
        }

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

        var employeeShifts = await _context.Shifts
            .Where(s => s.EmployeeId == userId && s.WeekId == currentWeek.Id)
            .Include(s => s.FillRequests)
            .Include(s => s.Employee)
            .ToListAsync();

        var viewModel = new EmployeeScheduleViewModel
        {
            Weeks = await _context.Weeks
                .Include(w => w.Shifts)
                .ThenInclude(s => s.Employee)
                .ThenInclude(f => f.FillRequests)
                .OrderByDescending(w => w.Year)
                .ThenByDescending(w => w.WeekNumber)
                .ToListAsync(),
            EmployeeId = userId,
            EmployeeName = _context.Employees
            .Where(e => e.Id == userId)
            .Select(e => e.Name)
            .FirstOrDefault() ?? "Unknown",
            WeekId = currentWeek.Id,
            PreviousWeekId = previousWeek?.Id,
            NextWeekId = nextWeek?.Id,
            CurrentWeekNumber = currentWeekNumber,
            IsCurrentWeek = (currentWeek.Year == currentYear && currentWeek.WeekNumber == currentWeekNumber)
        };
        return View(viewModel);
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
                Department = shift.Department
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

            // Checking if this FillRequest has not already been taken
            if (fillRequest.SubstituteEmployee != null) validShift = false;

			// Checking if this shift does not break any CAO rules
			var userAge = (DateTime.Today - user.BirthDate.ToDateTime(new TimeOnly())).Days/365;
			var CLAs = _context.CLAEntries.Where(a => (a.AgeStart <= userAge && a.AgeEnd >= userAge) || (a.AgeStart <= userAge && a.AgeEnd == null) || (a.AgeStart == null && a.AgeEnd >= userAge) || (a.AgeStart == null && a.AgeEnd == null)).ToList();
            var allShifts = _context.Shifts.Include(w => w.Week).ToList();
			validShift = new CLAApplyRules().ApplyCLARules(shift, CLAs, allShifts);

			// Getting shift user (TODO external employee makes fill request might crash)
            var shiftUser = _context.Users.Where(i => i.Id == shift.EmployeeId).FirstOrDefault();

            if (validShift == true)
            {
                // Creating FillReuestViewModel
                FillRequestViewModel fillRequestViewModel = new FillRequestViewModel()
                {
                    Id = fillRequest.Id,
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

    // GET: ScheduleEmployeeController/SendFillRequest/5
    [HttpGet("Invalsverzoek versturen")]
    public ActionResult SendFillRequest(int id)
    {
        var shift = _context.Shifts.FirstOrDefault(s => s.Id == id);
        if (shift == null) return NotFound();

        return View(shift);
    }

    // POST: ScheduleEmployeeController/SendFillRequest/5
    [ValidateAntiForgeryToken]
    [HttpPost("Invalsverzoek versturen")]
    public ActionResult SendFillRequestConfirmed(int id)
    {
        var shift = _context.Shifts.FirstOrDefault(s => s.Id == id);
        if (shift == null) return NotFound();

        // Check if there is not already an open FillRequest for this Shift
        if (_context.FillRequests.Where(s => s.ShiftId == id).FirstOrDefault() != null) return RedirectToAction(nameof(EmployeeSchedule));

        FillRequest fillRequest = new()
        {
            ShiftId = id,
        };

        if (ModelState.IsValid)
        {
            _context.FillRequests.Add(fillRequest);
            _context.SaveChanges();
            return RedirectToAction(nameof(EmployeeSchedule));
        }

        return RedirectToAction(nameof(EmployeeSchedule));
    }

    // GET: ScheduleEmployeeController/AcceptFillRequest/5
    [HttpGet("Invalsverzoek accepteren")]
    public ActionResult AcceptFillRequest(int id)
    {
        var fillRequest = _context.FillRequests.Include(f => f.Shift).FirstOrDefault(f => f.Id == id);
        if (fillRequest == null) return NotFound();

        return View(fillRequest);
    }

    // POST: ScheduleEmployeeController/AcceptFillRequest/5
    [ValidateAntiForgeryToken]
    [HttpPost("Invalsverzoek accepteren")]
    public async Task<IActionResult> AcceptFillRequestConfirmed(int id)
    {
        var fillRequest = _context.FillRequests.FirstOrDefault(s => s.Id == id);
        if (fillRequest == null) return NotFound();

        // Check if this FillRequest has not already been accepted
        if (fillRequest.SubstituteEmployee != null) return RedirectToAction(nameof(EmployeeSchedule));

        // Checking if this shift does not break any CAO rules
        bool validShift = true;
		var user = await _userManager.GetUserAsync(User);
		var userAge = (DateTime.Today - user.BirthDate.ToDateTime(new TimeOnly())).Days / 365;
		var CLAs = _context.CLAEntries.Where(a => (a.AgeStart <= userAge && a.AgeEnd >= userAge) || (a.AgeStart <= userAge && a.AgeEnd == null) || (a.AgeStart == null && a.AgeEnd >= userAge) || (a.AgeStart == null && a.AgeEnd == null)).ToList();
		var allShifts = _context.Shifts.Include(w => w.Week).ToList();
		validShift = new CLAApplyRules().ApplyCLARules(fillRequest.Shift, CLAs, allShifts);

		fillRequest.SubstituteEmployee = await _userManager.GetUserAsync(User);
        if (ModelState.IsValid && validShift)
        {
            _context.FillRequests.Update(fillRequest);
            _context.SaveChanges();
            return RedirectToAction(nameof(EmployeeSchedule));
        }

        return RedirectToAction(nameof(EmployeeSchedule));
    }

    // GET: ScheduleEmployeeController/Absent/5
    [HttpGet("Afmelden")]
    public async Task<IActionResult> Absent(int id)
    {
        // Getting Shift and Week
        var shift = _context.Shifts.FirstOrDefault(s => s.Id == id);
        if (shift == null) return NotFound();
        shift.Week = _context.Weeks.FirstOrDefault(i => i.Id == shift.WeekId);

        AbsenceViewModel absenceViewModel = new AbsenceViewModel()
        {
            ShiftId = id,
            Weekday = Weekday(shift.Week.Year, shift.Week.WeekNumber, shift.Weekday),
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            Department = shift.Department
        };

        return View(absenceViewModel);
    }

    // Post: ScheduleEmployeeController/Absent/5
    [ValidateAntiForgeryToken]
    [HttpPost("Afmelden")]
    public async Task<IActionResult> Absent(AbsenceViewModel absenceViewModel)
    {
        var shift = _context.Shifts.FirstOrDefault(s => s.Id == absenceViewModel.ShiftId);
        if (shift == null) return NotFound();

		// Check if the start or endtime is within the allowed range
		if (absenceViewModel.StartTime < shift.StartTime || absenceViewModel.EndTime > shift.EndTime) ModelState.AddModelError("", "De starttijd of eindtijd is buiten de shiftduur");
		if (!ModelState.IsValid) return View(absenceViewModel);

        // Check if the whole shift has to go or just a bit
        if (absenceViewModel.StartTime <= shift.StartTime && absenceViewModel.EndTime >= shift.EndTime)
        {
            var fillrequest = await _context.FillRequests.FirstOrDefaultAsync(f => f.ShiftId == shift.Id);
            if (fillrequest != null)
                _context.FillRequests.Remove(fillrequest);
            _context.Shifts.Remove(shift);
        }

        // Check if the shift has to be split
        else if (absenceViewModel.StartTime > shift.StartTime && absenceViewModel.EndTime < shift.EndTime)
        {
            Shift newShift = new Shift()
            {
                StartTime = absenceViewModel.EndTime,
                EndTime = shift.EndTime,
                WeekId = shift.WeekId,
                Week = shift.Week,
                Weekday = shift.Weekday,
                Department = shift.Department,
                EmployeeId = shift.EmployeeId,
                Employee = shift.Employee,
                DepartmentNavigation = shift.DepartmentNavigation
            };

            shift.EndTime = absenceViewModel.StartTime;

            _context.Shifts.Add(newShift);
            _context.Shifts.Update(shift);
        }
        else
        {
            if (shift.StartTime < absenceViewModel.StartTime) shift.EndTime = absenceViewModel.StartTime;
            else shift.StartTime = absenceViewModel.EndTime;
            _context.Shifts.Update(shift);
        }

        // Creating absent
        Absence absent = new Absence()
        {
            StartTime = absenceViewModel.StartTime,
            EndTime = absenceViewModel.EndTime,
            AbsentDescription = absenceViewModel.Description,
            Employee = await _userManager.GetUserAsync(User),
            WeekId = shift.WeekId,
            Week = shift.Week,
            Weekday = shift.Weekday
        };

        if (ModelState.IsValid)
        {
            _context.Absences.Add(absent);
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(EmployeeSchedule));
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
