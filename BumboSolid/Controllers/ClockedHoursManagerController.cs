﻿using BumboSolid.Data.Models;
using BumboSolid.Data;
using BumboSolid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace BumboSolid.Controllers;

[Authorize(Roles = "Manager")]
[Route("Geklokte uren")]
public class ClockedHoursManagerController : Controller
{
	private readonly BumboDbContext _context;
	private readonly UserManager<User> _userManager;

	public ClockedHoursManagerController(BumboDbContext context, UserManager<User> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

    [HttpGet("Overzicht/{employeeId:int}/{year:int?}/{weekNumber:int?}")]
    public async Task<IActionResult> Overview(int? weekId, int employeeId)
    {
        var employee = _context.Users.FirstOrDefault(u => u.Id == employeeId);
        string? employeeName = employee?.Name;

        var currentWeek = await GetCurrentWeek(weekId);

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

        var startDate = FirstDateOfWeek(currentWeek.Year, currentWeek.WeekNumber);

        var allClockedHours = await _context.ClockedHours
            .Where(ch => ch.EmployeeId == employeeId && ch.WeekId == weekId)
            .OrderByDescending(ch => ch.WeekId)
            .ThenByDescending(ch => ch.Weekday)
            .ThenByDescending(ch => ch.StartTime)
            .ToListAsync();

		var weekdayDictionary = new Dictionary<byte, string>
		{
			{ 0, "Maandag" },
			{ 1, "Dinsdag" },
			{ 2, "Woensdag" },
			{ 3, "Donderdag" },
			{ 4, "Vrijdag" },
			{ 5, "Zaterdag" },
			{ 6, "Zondag" }
		};

        ClockedHoursManagerOverviewViewModel overviewViewModel = new ClockedHoursManagerOverviewViewModel
        {
            StartDate = startDate,
            EndDate = startDate.AddDays(6),
            ClockedHours = allClockedHours,
            WeekdayDictionary = weekdayDictionary,
            EmployeeId = employeeId,
            EmployeeName = employeeName,
            WeekId = currentWeek.Id,
            PreviousWeekId = previousWeek?.Id,
            NextWeekId = nextWeek?.Id,
            IsCurrentWeek = (currentWeek.Year == currentYear && currentWeek.WeekNumber == currentWeekNumber),
        };

		return View(overviewViewModel);
	}

	[HttpGet("Bewerken/{id:int?}")]
	public async Task<IActionResult> Edit(int? id, int employeeId)
	{
		if (id == null)
		{
			return NotFound();
		}

		var clockedHours = await _context.ClockedHours.FindAsync(id);
		if (clockedHours == null)
		{
			return NotFound();
		}

		ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");
		ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });
		ViewBag.EmployeeId = employeeId;
		return View(clockedHours);
	}

	// POST: ClockedHours/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost("Bewerken/{id:int}")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, int employeeId, ClockedHours clockedHours)
	{
		if (id != clockedHours.Id) return NotFound();

		ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");
		ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

		// Check if the employee is allowed to work the given department
		User employee = await _context.Employees.Where(e => e.Id == employeeId).Include(e => e.Departments).FirstOrDefaultAsync();
		bool validDepartment = false;
		foreach (var department in employee.Departments) if (department.Name.Equals(clockedHours.Department)) validDepartment = true;
		if (validDepartment == false) ModelState.AddModelError("", "De medewerker mag niet werken bij deze afdeling");
		if (!ModelState.IsValid) return View(clockedHours);

		try
		{
			_context.Update(clockedHours);
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!ClockedHoursExists(clockedHours.Id))
			{
				return NotFound();
			}
			else
			{
				throw;
			}
		}
		return RedirectToAction(nameof(Overview), new { employeeId = clockedHours.EmployeeId });
	}

	// GET: ClockedHours/Delete/5
	[HttpGet("Verwijderen/{id:int?}")]
	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var clockedHours = await _context.ClockedHours
			.Include(c => c.Week)
			.FirstOrDefaultAsync(m => m.Id == id);
		if (clockedHours == null)
		{
			return NotFound();
		}

		return View(clockedHours);
	}

	// POST: ClockedHours/Delete/5
	[HttpPost("Verwijderen/{id:int?}")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var clockedHours = await _context.ClockedHours.FindAsync(id);
		int? employeeId = null;
		if (clockedHours != null)
		{
			employeeId = clockedHours.EmployeeId;
			_context.ClockedHours.Remove(clockedHours);
		}

		await _context.SaveChangesAsync();
		if (employeeId == null)
		{
			return RedirectToAction("Index", "Employees");
		}
		else
		{
			return RedirectToAction(nameof(Overview), new { employeeId });
		}
	}

	private bool ClockedHoursExists(int id)
	{
		return _context.ClockedHours.Any(e => e.Id == id);
	}

    DateOnly FirstDateOfWeek(int year, int week)
    {
        var jan1 = new DateOnly(year, 1, 1);
        var firstDayOfWeek = jan1.AddDays((week - 1) * 7 - (int)jan1.DayOfWeek + (int)DayOfWeek.Monday);

        return firstDayOfWeek;
    }

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
}