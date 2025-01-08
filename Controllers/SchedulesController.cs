using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data.Models;
using BumboSolid.Data;
using Microsoft.AspNetCore.Authorization;
using BumboSolid.Models;
using System.Globalization;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Humanizer;

namespace BumboSolid.Controllers
{
	[Authorize(Roles = "Manager")]
	[Route("Rooster")]
	public class SchedulesController : Controller
	{
		private readonly BumboDbContext _context;
		private static TimeOnly openingTime = new TimeOnly(7, 0);
		private static TimeOnly closingTime = new TimeOnly(20, 0);

		public SchedulesController(BumboDbContext context)
		{
			_context = context;
		}

		// GET: Shifts
		[HttpGet("")]
		[HttpGet("{id:int?}")]
		public async Task<IActionResult> Index(int? id)
		{
			CultureInfo ci = new CultureInfo("nl-NL");
			Calendar calendar = ci.Calendar;

			DateTime nextWeek = DateTime.Now.AddDays(7);
			short year = (short)nextWeek.Year;
			byte week = (byte)calendar.GetWeekOfYear(nextWeek, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);

			var currentWeek = _context.Weeks.FirstOrDefault(w => w.Year == year && w.WeekNumber == week);

			if (id == null)
			{
				if (currentWeek != null)
				{
					if (currentWeek.HasSchedule == 1)
					{
						id = currentWeek.Id;
					}
					else
					{
						return RedirectToAction("Create");
					}
				}
				else
				{
					return RedirectToAction("Create");
				}
			}

			ViewBag.FillRequests = _context.FillRequests.Where(f => f.Accepted == 0 && f.SubstituteEmployee != null).ToList().Count;

			var viewModel = new SchedulesViewModel
			{
				Weeks = await _context.Weeks
					.Include(w => w.Shifts)
						.ThenInclude(s => s.Employee)
					.OrderByDescending(p => p.Year)
						.ThenByDescending(p => p.WeekNumber)
						.ToListAsync(),
				WeekId = (int)id,
			};

			return View(viewModel);
		}

		// GET: Shifts/Create
		[HttpGet("Aanmaken")]
		public IActionResult Create()
		{
			return View();
		}

		// POST: Shifts/Create
		[HttpPost("Aanmaken")]
		public IActionResult Create(int? id)
		{
			CultureInfo ci = new CultureInfo("nl-NL");
			Calendar calendar = ci.Calendar;

			DateTime nextWeek = DateTime.Now.AddDays(7);
			short year = (short)nextWeek.Year;
			byte week = (byte)calendar.GetWeekOfYear(nextWeek, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);

			var currentWeek = _context.Weeks
				.Include(w => w.PrognosisDays)
					.ThenInclude(pd => pd.PrognosisDepartments)
						.ThenInclude(pd => pd.DepartmentNavigation)
				.FirstOrDefault(w => w.Year == year && w.WeekNumber == week);

			if (currentWeek != null)
			{
				id = currentWeek.Id;

				if (currentWeek.PrognosisDays.Count == 7)
				{
					foreach (PrognosisDay day in currentWeek.PrognosisDays)
					{
						DateTime startOfYear = new DateTime(year, 1, 1);
						DateTime currentDay = calendar.AddWeeks(startOfYear, week - 1).AddDays(day.Weekday - (int)startOfYear.DayOfWeek + 1);
						DateOnly date = DateOnly.FromDateTime(currentDay);
						foreach (PrognosisDepartment department in day.PrognosisDepartments)
						{
							int remainingWorkHours = department.WorkHours;

							var availabilityRules = _context.AvailabilityRules
								.Include(ar => ar.EmployeeNavigation)
									.ThenInclude(en => en!.Departments).ToList();

							availabilityRules = availabilityRules.Where(ar => ar.Available == 1 && ar.Date == date && ar.EmployeeNavigation!.Departments.Contains(department.DepartmentNavigation))
								.OrderBy(ar => ar.StartTime)
									.ThenBy(ar => ar.EndTime - ar.StartTime).ToList();

							foreach (AvailabilityRule rule in availabilityRules)
							{
								if (remainingWorkHours <= 0) break;
								User employee = _context.Employees.First(e => e.Id == rule.Employee);

								_context.Shifts.Add(new Shift()
								{
									WeekId = currentWeek.Id,
									Weekday = day.Weekday,
									Department = department.Department,
									StartTime = openingTime.CompareTo(rule.StartTime) > 0 ? openingTime : rule.StartTime,
									EndTime = closingTime.CompareTo(rule.EndTime) < 0 ? closingTime : rule.EndTime,
									EmployeeId = rule.Employee,
									IsBreak = 0,
								});
								remainingWorkHours -= (rule.EndTime - rule.StartTime).Hours;
							}
						}
					}
				}

				currentWeek.HasSchedule = 1;
				_context.SaveChangesAsync();
			}
			else
			{
				currentWeek = new Week()
				{
					Year = year,
					WeekNumber = week,
					HasSchedule = 1,
				};
				_context.Add(currentWeek);
				_context.SaveChanges();

				id = currentWeek.Id;
			}

			return RedirectToAction("Index", new { id });
		}

        // GET: Shifts/FillRequests
        [HttpGet("Invalsverzoeken")]
        public IActionResult FillRequests()
        {
			List<FillRequestViewModel> fillRequestViewModels = new List<FillRequestViewModel>();
			foreach(var fillRequest in _context.FillRequests.Where(f => f.Accepted == 0 && f.SubstituteEmployee != null).Include(f => f.Shift).Include(f => f.Shift.Week).Include(f => f.Shift.Employee).Include(f => f.SubstituteEmployee))
			{
				// Getting Shift and Week
				var shift = fillRequest.Shift;
				var week = shift.Week;

                // Getting correct date and day
                var jan1 = new DateOnly(week.Year, 1, 1);
                DateOnly date = jan1.AddDays(DayOfWeek.Monday - jan1.DayOfWeek).AddDays((week.WeekNumber - 1) * 7).AddDays((int)shift.Weekday - (int)DayOfWeek.Monday);

                string[] days = ["Monday", "Tuesdday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

                // Creating FillReuestViewModel
                fillRequestViewModels.Add(new FillRequestViewModel()
                {
                    Date = date,
                    Day = days[shift.Weekday],
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    Department = shift.Department,
                    Shift = shift,
					SubstituteEmployee = fillRequest.SubstituteEmployee,
					Id = fillRequest.Id,
                });
            }

            return View(fillRequestViewModels);
        }

        //GET: Shifts/AnswerFillRequest
        [HttpGet("Invalsverzoek Antwoorden")]
        public IActionResult AnswerFillRequest(int id, string status)
        {
			FillRequest fillRequest = _context.FillRequests.Where(f => f.Id == id).Include(f => f.Shift).Include(f => f.Shift.Week).Include(f => f.Shift.Employee).Include(f => f.SubstituteEmployee).FirstOrDefault();

            if (fillRequest == null) return NotFound();

            // Getting Shift and Week
            var shift = fillRequest.Shift;
            var week = shift.Week;

            // Getting correct date and day
            var jan1 = new DateOnly(week.Year, 1, 1);
            DateOnly date = jan1.AddDays(DayOfWeek.Monday - jan1.DayOfWeek).AddDays((week.WeekNumber - 1) * 7).AddDays((int)shift.Weekday - (int)DayOfWeek.Monday);

            string[] days = ["Monday", "Tuesdday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

            FillRequestViewModel fillRequestViewModel = new()
			{
                Date = date,
                Day = days[shift.Weekday],
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Department = shift.Department,
                Shift = shift,
                SubstituteEmployee = fillRequest.SubstituteEmployee,
                Id = fillRequest.Id,
            };

			ViewBag.Answer = status;

            return View(fillRequestViewModel);
        }

        //POST: Shifts/AnswerFillRequest
        [HttpPost("Invalsverzoek Antwoorden")]
        public IActionResult AnswerFillRequestConfirm(int id, string Status)
        {
            FillRequest fillRequest = _context.FillRequests.Where(f => f.Id == id).Include(f => f.Shift).Include(f => f.Shift.Employee).Include(f => f.SubstituteEmployee).FirstOrDefault();

            if (fillRequest == null) return NotFound();

			// Accept FillRequest
			if (Status.Equals("accepteren"))
			{
                Shift shift = fillRequest.Shift;
                shift.Employee = fillRequest.SubstituteEmployee;
				fillRequest.Accepted = 1;

                _context.Shifts.Update(shift);
                _context.FillRequests.Update(fillRequest);
            }
            else _context.FillRequests.Remove(fillRequest);

            _context.SaveChanges();

            return RedirectToAction("FillRequests");
        }
    }
}
