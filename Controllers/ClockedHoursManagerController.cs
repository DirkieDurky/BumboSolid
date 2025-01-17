using BumboSolid.Data.Models;
using BumboSolid.Data;
using BumboSolid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    public async Task<IActionResult> Overview(int employeeId, int? year, int? weekNumber)
    {
        if (year == null || weekNumber == null)
        {
            CultureInfo ci = new CultureInfo("nl-NL");
            Calendar calendar = ci.Calendar;

            year = (short)DateTime.Now.Year;
            weekNumber = (byte)calendar.GetWeekOfYear(DateTime.Now, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
        }

        var employee = _context.Users.FirstOrDefault(u => u.Id == employeeId);
        string? employeeName = employee == null ? null : employee.Name;

        var week = _context.Weeks.FirstOrDefault(w => w.Year == year && w.WeekNumber == weekNumber);
        DateOnly startDate;
        if (week == null)
        {
            startDate = FirstDateOfWeek((int)year, (int)weekNumber);
            ClockedHoursManagerOverviewViewModel emptyOverviewViewModel = new ClockedHoursManagerOverviewViewModel
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(6),
                ClockedHours = new(),
                WeekdayDictionary = new(),
                Year = (int)year,
                WeekNumber = (int)weekNumber,
                EmployeeId = employeeId,
                EmployeeName = employeeName,
            };

            return View(emptyOverviewViewModel);
        }

        var allClockedHours = await _context.ClockedHours
            .Where(ch => ch.EmployeeId == employeeId && ch.WeekId == week.Id)
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

        startDate = FirstDateOfWeek(week.Year, week.WeekNumber);
        ClockedHoursManagerOverviewViewModel overviewViewModel = new ClockedHoursManagerOverviewViewModel
        {
            StartDate = startDate,
            EndDate = startDate.AddDays(6),
            ClockedHours = allClockedHours,
            WeekdayDictionary = weekdayDictionary,
            Year = (int)year,
            WeekNumber = (int)weekNumber,
            EmployeeId = employeeId,
            EmployeeName = employeeName,
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
        if (id != clockedHours.Id)
        {
            return NotFound();
        }

        ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");
        ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

        if (ModelState.IsValid)
        {
            if (clockedHours.EndTime < clockedHours.StartTime)
            {
                ViewBag.Error = "De eindtijd moet later of gelijk zijn aan de starttijd.";
                return View(clockedHours);
            }

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

        return View(clockedHours);
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
}