﻿using System;
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
using Microsoft.IdentityModel.Tokens;

namespace BumboSolid.Controllers
{
    [Authorize(Roles = "Manager")]
    [Route("Shifts")]
    public class ShiftsController : Controller
    {
        private readonly BumboDbContext _context;

        public ShiftsController(BumboDbContext context)
        {
            _context = context;
        }

        // GET: Shifts
        [HttpGet("")]
        [HttpGet("{id:int?}")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                CultureInfo ci = new CultureInfo("nl-NL");
                Calendar calendar = ci.Calendar;

                //Add week entry for next week
                DateTime nextWeek = DateTime.Now.AddDays(7);
                short year = (short)nextWeek.Year;
                byte week = (byte)calendar.GetWeekOfYear(nextWeek, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);

                var currentWeek = _context.Weeks.FirstOrDefault(w => w.Year == year && w.WeekNumber == week);
                if (currentWeek == null)
                {
                    currentWeek = new Week()
                    {
                        Year = year,
                        WeekNumber = week,
                    };
                    _context.Add(currentWeek);
                    _context.SaveChanges();
                }
                id = currentWeek.Id;
            }

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

        // GET: Shifts/Details/5
        [HttpGet("Rooster Details")]
        public IActionResult Details(short year, short week, int day, int startTime, int endTime)
        {
            TimeOnly startTimeTime = new(startTime, 0);
            TimeOnly endTimeTime = new(endTime, 0);

            var shifts = _context.Shifts
                .Include(s => s.DepartmentNavigation)
                .Include(s => s.Week)
                .Include(s => s.Employee)
                .Where(s => s.Week!.Year == year && s.Week.WeekNumber == week && s.Weekday == day && s.StartTime <= endTimeTime && s.EndTime >= startTimeTime)
                .ToList();

            if (shifts == null)
            {
                return NotFound();
            }

            string dayName;

            switch (day)
            {
                case 0:
                    dayName = "Maandag";
                    break;
                case 1:
                    dayName = "Dinsdag";
                    break;
                case 2:
                    dayName = "Woensdag";
                    break;
                case 3:
                    dayName = "Donderdag";
                    break;
                case 4:
                    dayName = "Vrijdag";
                    break;
                case 5:
                    dayName = "Zaterdag";
                    break;
                case 6:
                    dayName = "Zondag";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(day), "Invalid day of the week");
            }

            ScheduleViewDetailsViewModel scheduleViewDetailsViewModel = new()
            {
                Shifts = shifts,
                Day = dayName,
                StartTime = startTimeTime,
                EndTime = endTimeTime,
            };

            return View(scheduleViewDetailsViewModel);
        }

        // GET: Shifts/Create
        [HttpGet("MedewerkerInplannen/{weekId:int}")]
        public async Task<IActionResult> Create(int weekId)
        {
            var week = _context.Weeks.First(w => w.Id == weekId);

            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");
            ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

            var employeeRole = await _context.Roles
                .Where(r => r.Name == "Employee")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var employees = await _context.UserRoles
                .Where(ur => ur.RoleId == employeeRole)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var employeeUsers = _context.Users
                .Where(u => employees.Contains(u.Id))
                .Include(u => u.AvailabilityRules).ToList();

            var viewModel = new ShiftCreateViewModel
            {
                Employees = employeeUsers,
                Shift = new Shift(),
                Week = week,
            };

            return View(viewModel);
        }

        // POST: Shifts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("MedewerkerInplannen/{weekId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int weekId, ShiftCreateViewModel shiftCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                if (shiftCreateViewModel.Shift.Employee == null && shiftCreateViewModel.Shift.ExternalEmployeeName.IsNullOrEmpty())
                {
                    ViewBag.Error = "Vul een medewerker in";
                }
                else if (shiftCreateViewModel.Shift.EndTime <= shiftCreateViewModel.Shift.StartTime)
                {
                    ViewBag.Error = "De eindtijd moet later zijn dan de starttijd.";
                }
                else
                {
                    if (shiftCreateViewModel.Shift.EmployeeId == -1) shiftCreateViewModel.Shift.EmployeeId = null;
                    _context.Add(shiftCreateViewModel.Shift);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Schedules");
                }
            }

            var week = _context.Weeks.First(w => w.Id == weekId);

            var employeeRole = await _context.Roles
                .Where(r => r.Name == "Employee")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var employees = await _context.UserRoles
                .Where(ur => ur.RoleId == employeeRole)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var employeeUsers = _context.Users
                .Where(u => employees.Contains(u.Id))
                .Include(u => u.AvailabilityRules).ToList();

            shiftCreateViewModel.Week = week;
            shiftCreateViewModel.Employees = employeeUsers;

            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
            ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

            return View(shiftCreateViewModel);
        }


        // GET: Shifts/Edit/5
        [HttpGet("Bewerken/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shifts.FirstOrDefault(s => s.Id == id) == null)
            {
                return NotFound();
            }

            Shift shift = _context.Shifts.Include(s => s.Week).First(s => s.Id == id);

            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");
            ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

            var employeeRole = await _context.Roles
                .Where(r => r.Name == "Employee")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var employees = await _context.UserRoles
                .Where(ur => ur.RoleId == employeeRole)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var employeeUsers = _context.Users
                .Where(u => employees.Contains(u.Id))
                .Include(u => u.AvailabilityRules).ToList();

            var viewModel = new ShiftCreateViewModel
            {
                Employees = employeeUsers,
                Shift = shift,
                Week = shift.Week,
            };

            if (shift.EmployeeId == null) shift.EmployeeId = -1;

            return View(viewModel);
        }

        // POST: Shifts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost("Bewerken/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShiftCreateViewModel shiftCreateViewModel)
        {
            if (id != shiftCreateViewModel.Shift.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (shiftCreateViewModel.Shift.EmployeeId == -1) shiftCreateViewModel.Shift.EmployeeId = null;
                    _context.Update(shiftCreateViewModel.Shift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftExists(shiftCreateViewModel.Shift.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Schedules");
            }
            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
            ViewBag.WeekDays = new SelectList(_context.Weeks, "Id", "Id", shiftCreateViewModel.Shift.WeekId);
            return View(shiftCreateViewModel);
        }

        // GET: Shifts/Delete/5
        [HttpGet("Verwijderen/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shifts
                .Include(s => s.DepartmentNavigation)
                .Include(s => s.Week)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shift == null)
            {
                return NotFound();
            }

            return View(shift);
        }

        // POST: Shifts/Delete/5
        [ActionName("Delete")]
        [HttpPost("Verwijderen/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift != null)
            {
                _context.Shifts.Remove(shift);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Schedules");
        }

        private bool ShiftExists(int id)
        {
            return _context.Shifts.Any(e => e.Id == id);
        }
    }
}