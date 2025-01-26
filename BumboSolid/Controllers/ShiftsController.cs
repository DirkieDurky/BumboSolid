using Microsoft.AspNetCore.Mvc;
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
        // Exclude recursive columns to avoid infinite loop converting to JSON
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
        shiftCreateViewModel.Shift.WeekId = weekId;// Only show the users with the employee role, because the manager may not work a shift.

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

        List<Shift> shifts = new();
        // Exclude recursive columns to avoid infinite loop converting to JSON
        foreach (Shift shift in _context.Shifts.ToList())
        {
            Shift newShift = new Shift()
            {
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                WeekId = shift.WeekId,
                EmployeeId = shift.EmployeeId,
                Weekday = shift.Weekday,
            };
            shifts.Add(newShift);
        }

        shiftCreateViewModel.Shifts = shifts;

        List<User> employees = new List<User>();
        // Only include columns that I need to avoid infinite loop converting to JSON
        foreach (User u in employeeUsers)
        {
            User newUser = new User()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                BirthDate = u.BirthDate,
            };
            employees.Add(newUser);
        }
        shiftCreateViewModel.Employees = employees;

        ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
        ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

		// Checking if this shift doens not overlap with another shift
		var employee = _context.Employees.Where(e => e.Id == shiftCreateViewModel.Shift.EmployeeId).FirstOrDefault();
		var employeeAge = (DateTime.Today - employee.BirthDate.ToDateTime(new TimeOnly())).Days / 365;
		Shift employeeShift = shiftCreateViewModel.Shift;
		List<Shift> employeeShifts = _context.Shifts.Where(s => s.EmployeeId == employee.Id && s.Week == employeeShift.Week && s.Weekday == employeeShift.Weekday).ToList();
		foreach (var shift in employeeShifts) 
            if ((employeeShift.StartTime <= shift.StartTime && employeeShift.EndTime >= shift.EndTime) || (employeeShift.StartTime <= shift.EndTime && employeeShift.EndTime >= shift.EndTime) || (employeeShift.EndTime >= shift.StartTime && employeeShift.StartTime <= shift.StartTime) || (employeeShift.StartTime >= shift.StartTime && employeeShift.EndTime <= shift.EndTime)) 
                ModelState.AddModelError("", "Deze shift overlapt met een andere shift");
		// Checking if this shift does not break any CAO rules
		var CLAs = _context.CLAEntries.Where(a => (a.AgeStart <= employeeAge && a.AgeEnd >= employeeAge) || (a.AgeStart <= employeeAge && a.AgeEnd == null) || (a.AgeStart == null && a.AgeEnd >= employeeAge) || (a.AgeStart == null && a.AgeEnd == null)).ToList();
		var allShifts = _context.Shifts.Include(w => w.Week).ToList();
		if (new CLAApplyRules().ApplyCLARules(shiftCreateViewModel.Shift, CLAs, allShifts) == false) ModelState.AddModelError("", "Er worden CLA regels overtreden");
		if (!ModelState.IsValid) return View(shiftCreateViewModel);

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
        // Only include columns that I need to avoid infinite loop converting to JSON
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
        // Only include columns that I need to avoid infinite loop converting to JSON
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

        List<Shift> shifts = new List<Shift>();
        // Only include columns that I need to avoid infinite loop converting to JSON
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
        // Only include columns that I need to avoid infinite loop converting to JSON
        foreach (User employeeUser in employeeUsers)
        {
            User newUser = new User()
            {
                Id = employeeUser.Id,
                FirstName = employeeUser.FirstName,
                LastName = employeeUser.LastName,
                BirthDate = employeeUser.BirthDate,
            };
            employees.Add(newUser);
        }

        shiftCreateViewModel.Shifts = shifts;
        shiftCreateViewModel.CLAEntries = _context.CLAEntries.ToList();
        shiftCreateViewModel.Employees = employees;

        ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
        ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

		// Checking if this shift doens not overlap with another shift
		var employee = _context.Employees.Where(e => e.Id == shiftCreateViewModel.Shift.EmployeeId).FirstOrDefault();
		var employeeAge = (DateTime.Today - employee.BirthDate.ToDateTime(new TimeOnly())).Days / 365;
		Shift employeeShift = shiftCreateViewModel.Shift;
		List<Shift> employeeShifts = _context.Shifts.Where(s => s.EmployeeId == employee.Id && s.Week == employeeShift.Week && s.Weekday == employeeShift.Weekday).ToList();
		foreach (var shift in employeeShifts)
			if ((employeeShift.StartTime <= shift.StartTime && employeeShift.EndTime >= shift.EndTime) || (employeeShift.StartTime <= shift.EndTime && employeeShift.EndTime >= shift.EndTime) || (employeeShift.EndTime >= shift.StartTime && employeeShift.StartTime <= shift.StartTime) || (employeeShift.StartTime >= shift.StartTime && employeeShift.EndTime <= shift.EndTime))
				ModelState.AddModelError("", "Deze shift overlapt met een andere shift");
		// Checking if this shift does not break any CAO rules
		var CLAs = _context.CLAEntries.Where(a => (a.AgeStart <= employeeAge && a.AgeEnd >= employeeAge) || (a.AgeStart <= employeeAge && a.AgeEnd == null) || (a.AgeStart == null && a.AgeEnd >= employeeAge) || (a.AgeStart == null && a.AgeEnd == null)).ToList();
        var allShifts = _context.Shifts.Include(w => w.Week).ToList();
		if (new CLAApplyRules().ApplyCLARules(shiftCreateViewModel.Shift, CLAs, allShifts) == false) ModelState.AddModelError("", "Er worden CLA regels overtreden");
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