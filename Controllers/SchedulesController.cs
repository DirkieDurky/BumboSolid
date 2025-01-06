﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data.Models;
using BumboSolid.Data;
using Microsoft.AspNetCore.Authorization;
using BumboSolid.Models;
using System.Globalization;

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
            var currentWeek = await _context.Weeks
                .Include(w => w.Shifts)
                .ThenInclude(s => s.Employee)
                .FirstOrDefaultAsync(w => w.Id == id);

            var culture = new CultureInfo("nl-NL");
            var today = DateTime.Now;
            var currentYear = (short)today.Year;
            var currentWeekNumber = (byte)culture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            if (currentWeek == null)
            {
                currentWeek = await _context.Weeks
                    .Include(w => w.Shifts)
                    .ThenInclude(s => s.Employee)
                    .FirstOrDefaultAsync(w => w.Year == currentYear && w.WeekNumber == currentWeekNumber && w.HasSchedule == 1);

                if (currentWeek == null) return RedirectToAction("Create");
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

            var viewModel = new SchedulesViewModel
            {
                Weeks = await _context.Weeks
                    .Include(w => w.Shifts)
                    .ThenInclude(s => s.Employee)
                    .OrderByDescending(w => w.Year)
                    .ThenByDescending(w => w.WeekNumber)
                    .ToListAsync(),
                WeekId = currentWeek.Id,
                PreviousWeekId = previousWeek?.Id,
                NextWeekId = nextWeek?.Id,
                CurrentWeekNumber = currentWeekNumber,
                IsCurrentWeek = (currentWeek.Year == currentYear && currentWeek.WeekNumber == currentWeekNumber)
            };
            return View(viewModel);
        }

        [HttpGet("{employeeId:int}/{id:int?}")]
        public async Task<IActionResult> EmployeeSchedule(int? id, int employeeId)
        {
            var currentWeek = await _context.Weeks
                .Include(w => w.Shifts)
                .ThenInclude(s => s.Employee)
                .FirstOrDefaultAsync(w => w.Id == id);

            var culture = new CultureInfo("nl-NL");
            var today = DateTime.Now;
            var currentYear = (short)today.Year;
            var currentWeekNumber = (byte)culture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            if (currentWeek == null)
            {
                currentWeek = await _context.Weeks
                    .Include(w => w.Shifts)
                    .ThenInclude(s => s.Employee)
                    .FirstOrDefaultAsync(w => w.Year == currentYear && w.WeekNumber == currentWeekNumber && w.HasSchedule == 1);

                if (currentWeek == null) return RedirectToAction("Create");
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
                .Where(s => s.EmployeeId == employeeId && s.WeekId == currentWeek.Id)
                .Include(s => s.Employee)
                .ToListAsync();

            var viewModel = new EmployeeScheduleDetailsViewModel
            {
                Weeks = await _context.Weeks
                    .Include(w => w.Shifts)
                    .ThenInclude(s => s.Employee)
                    .OrderByDescending(w => w.Year)
                    .ThenByDescending(w => w.WeekNumber)
                    .ToListAsync(),
                EmployeeId = employeeId,
                EmployeeName = _context.Employees
        .Where(e => e.Id == employeeId)
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
    }
}
