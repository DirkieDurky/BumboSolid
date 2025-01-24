﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data.Models;
using BumboSolid.Data;
using Microsoft.AspNetCore.Authorization;
using BumboSolid.Models;
using BumboSolid.HelperClasses.CLARules;
using Microsoft.AspNetCore.Identity;

namespace BumboSolid.Controllers;

[Authorize(Roles = "Manager")]
[Route("Shifts")]
public class ShiftsController : Controller
{
    private readonly BumboDbContext _context;
    private readonly UserManager<User> _userManager;

    public ShiftsController(BumboDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Shifts/Create
    [HttpGet("MedewerkerInplannen/{weekId:int}")]
    public async Task<IActionResult> Create(int weekId)
    {
        var week = _context.Weeks.First(w => w.Id == weekId);
        if (week == null) return NotFound();

        ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");

        // Only show the users with the employee role, because the manager may not work a shift.
        var employeeRole = await _context.Roles
            .Where(r => r.Name == "Employee")
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        if (employeeRole == 0)
        {
            throw new Exception("No employee role found");
        }

        var employeeRoleConnections = await _context.UserRoles
            .Where(ur => ur.RoleId == employeeRole)
            .Select(ur => ur.UserId)
            .ToListAsync();

        var employeeUsers = await _context.Users
            .Where(u => employeeRoleConnections.Contains(u.Id))
            .ToListAsync();

        List<Shift> shifts = _context.Shifts.ToList();
        //Exclude recursive columns to avoid infinite loop converting to JSON
        foreach (Shift shift in shifts)
        {
            shift.Employee = null;
            shift.Department = "";
            shift.DepartmentNavigation = null;
            shift.Week = null;
        }

        var viewModel = new ShiftCreateViewModel
        {
            Employees = employeeUsers,
            Shift = new Shift(),
            Week = week,
            Shifts = shifts,
            CLAEntries = _context.CLAEntries.ToList(),
        };

        return View(viewModel);
    }

    // POST: Shifts/Create
    [HttpPost("MedewerkerInplannen/{weekId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int weekId, ShiftCreateViewModel shiftCreateViewModel)
    {
        var week = await _context.Weeks.FirstOrDefaultAsync(w => w.Id == weekId);
        if (week == null) return NotFound();

        shiftCreateViewModel.Week = week;
        shiftCreateViewModel.Shift.Week = week;
        shiftCreateViewModel.Shift.WeekId = weekId;
        shiftCreateViewModel.Employees = await _context.Employees.ToListAsync();

        ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
        ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

        // Checking if this shift does not break any CAO rules
        bool validShift = true;
        var user = await _userManager.GetUserAsync(User);
        var userAge = (DateTime.Today - user.BirthDate.ToDateTime(new TimeOnly())).Days / 365;
        var CLAs = _context.CLAEntries.Where(a => (a.AgeStart <= userAge && a.AgeEnd >= userAge) || (a.AgeStart <= userAge && a.AgeEnd == null) || (a.AgeStart == null && a.AgeEnd >= userAge) || (a.AgeStart == null && a.AgeEnd == null)).ToList();
        var allShifts = _context.Shifts.Include(w => w.Week).ToList();
        validShift = new CLAApplyRules().ApplyCLARules(shiftCreateViewModel.Shift, CLAs, allShifts);

        if (!validShift)
        {
            ViewBag.Error = "Er worden CAO regels overtreden";

            return View(shiftCreateViewModel);
        }
        if (!ModelState.IsValid)
        {
            return View(shiftCreateViewModel);
        }
        if (shiftCreateViewModel.Shift.EndTime <= shiftCreateViewModel.Shift.StartTime)
        {
            ViewBag.Error = "De eindtijd moet later zijn dan de starttijd.";

            return View(shiftCreateViewModel);
        }

        if (shiftCreateViewModel.Shift.EmployeeId == -1)
        {
            shiftCreateViewModel.Shift.EmployeeId = null;
        }

        _context.Add(shiftCreateViewModel.Shift);
        await _context.SaveChangesAsync();

        return RedirectToAction("ManagerSchedule", "ScheduleManager");
    }


    // GET: Shifts/Edit/5
    [HttpGet("Bewerken/{id:int}")]
    public async Task<IActionResult> Edit(int? id)
    {
        var shift = await _context.Shifts
            .Include(s => s.Week)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (shift == null) return NotFound();

        List<Shift> shifts = new List<Shift>();
        //Only include columns that I need to avoid infinite loop converting to JSON
        foreach (Shift s in _context.Shifts.ToList())
        {
            Shift newShift = new Shift()
            {
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                WeekId = s.WeekId,
                EmployeeId = s.EmployeeId,
                Weekday = s.Weekday,
            };
            shifts.Add(newShift);
        }
        
        // Only show the users with the employee role, because the manager may not work a shift.
        var employeeRole = await _context.Roles
            .Where(r => r.Name == "Employee")
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        if (employeeRole == 0)
        {
            throw new Exception("No employee role found");
        }

        var employeeRoleConnections = await _context.UserRoles
            .Where(ur => ur.RoleId == employeeRole)
            .Select(ur => ur.UserId)
            .ToListAsync();

        var employeeUsers = await _context.Users
            .Where(u => employeeRoleConnections.Contains(u.Id))
            .ToListAsync();

        List<User> employees = new List<User>();
        //Only include columns that I need to avoid infinite loop converting to JSON
        foreach (User user in employeeUsers)
        {
            User newUser = new User()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
            };
            employees.Add(newUser);
        }

        var viewModel = new ShiftCreateViewModel
        {
            Shift = shift,
            Employees = employees,
            Week = shift.Week,
            Shifts = shifts,
            CLAEntries = _context.CLAEntries.ToList(),
        };

        ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");

        return View(viewModel);
    }

    // POST: Shifts/Edit/5
    [HttpPost("Bewerken/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ShiftCreateViewModel shiftCreateViewModel)
    {
        var week = await _context.Weeks.FirstOrDefaultAsync(w => w.Id == shiftCreateViewModel.Week.Id);
        if (week == null) return NotFound();

        shiftCreateViewModel.Week = week;
        shiftCreateViewModel.Shift.Week = week;
        shiftCreateViewModel.Shift.WeekId = week.Id;
        shiftCreateViewModel.Employees = await _context.Employees.ToListAsync();

        ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
        ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

        // Checking if this shift does not break any CAO rules
        bool validShift = true;
        var user = await _userManager.GetUserAsync(User);
        var userAge = (DateTime.Today - user.BirthDate.ToDateTime(new TimeOnly())).Days / 365;
        var CLAs = _context.CLAEntries.Where(a => (a.AgeStart <= userAge && a.AgeEnd >= userAge) || (a.AgeStart <= userAge && a.AgeEnd == null) || (a.AgeStart == null && a.AgeEnd >= userAge) || (a.AgeStart == null && a.AgeEnd == null)).ToList();
        var allShifts = _context.Shifts.Include(w => w.Week).ToList();
        validShift = new CLAApplyRules().ApplyCLARules(shiftCreateViewModel.Shift, CLAs, allShifts);

        if (!validShift)
        {
            ViewBag.Error = "Er worden CAO regels overtreden";

            return View(shiftCreateViewModel);
        }
        if (!ModelState.IsValid) return RedirectToAction(nameof(Edit), new { id });
        try
        {
            if (shiftCreateViewModel.Shift.EmployeeId == -1)
            {
                shiftCreateViewModel.Shift.EmployeeId = null;

                if (string.IsNullOrEmpty(shiftCreateViewModel.Shift.ExternalEmployeeName))
                {
                    ModelState.AddModelError("Shift.ExternalEmployeeName", "Externe medewerker naam is verplicht.");
                    return RedirectToAction(nameof(Edit), new { id });
                }
            }
            else
            {
                shiftCreateViewModel.Shift.ExternalEmployeeName = null;
            }

            if (shiftCreateViewModel.Shift.EndTime <= shiftCreateViewModel.Shift.StartTime)
            {
                ViewBag.Error = "De eindtijd moet later zijn dan de starttijd.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            var existingShift = await _context.Shifts
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingShift == null)
            {
                return NotFound();
            }

            existingShift.EmployeeId = shiftCreateViewModel.Shift.EmployeeId;
            existingShift.ExternalEmployeeName = shiftCreateViewModel.Shift.ExternalEmployeeName;
            existingShift.StartTime = shiftCreateViewModel.Shift.StartTime;
            existingShift.EndTime = shiftCreateViewModel.Shift.EndTime;
            existingShift.Department = shiftCreateViewModel.Shift.Department;
            existingShift.Weekday = shiftCreateViewModel.Shift.Weekday;

            _context.Entry(existingShift).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return RedirectToAction("ManagerSchedule", "ScheduleManager");
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
            var fillrequest = await _context.FillRequests.FirstOrDefaultAsync(f => f.ShiftId == id);
            if (fillrequest != null)
                _context.FillRequests.Remove(fillrequest);
            _context.Shifts.Remove(shift);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("ManagerSchedule", "ScheduleManager");
    }

    private bool ShiftExists(int id)
    {
        return _context.Shifts.Any(e => e.Id == id);
    }
}