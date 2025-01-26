using BumboSolid.Data;
using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BumboSolid.Controllers;

[Authorize(Roles = "Employee,Manager")]
[Route("Beschikbaarheid")]
public class AvailabilityController : Controller
{
    private readonly BumboDbContext _context;
    private readonly UserManager<User> _userManager;

    public AvailabilityController(BumboDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;

    }

    // GET: AvailiabilityController/Index
    [HttpGet("")]
    public async Task<IActionResult> Index(DateOnly weekStart)
    {
        // Getting user
        var user = await _userManager.GetUserAsync(User);
        int userId = user.Id;

        DateOnly startDate = weekStart;

        var availabilityRules = _context.AvailabilityRules
                             .Where(r => r.Employee == userId).OrderByDescending(r => r.Date).ThenBy(r => r.StartTime)
                             .ToList();

        return View(availabilityRules);
    }

    // GET: AvailiabilityController/Create
    [HttpGet("Aanmaken")]
    public async Task<IActionResult> Create(int Year, int WeekNr)
    {
        ViewBag.year = Year;
        ViewBag.weekNr = WeekNr;

        return View(new AvailabilityRule());
    }

    // Post: AvailiabilityController/Create
    [ValidateAntiForgeryToken]
    [HttpPost("Aanmaken")]
    public async Task<IActionResult> Create(AvailabilityRule availabilityRule, int Year, int WeekNr, string Availability)
    {
        // Getting user
        var user = await _userManager.GetUserAsync(User);
        int userId = user.Id;

        ViewBag.year = Year;
        ViewBag.weekNr = WeekNr;

        // Making sure that EndTime is not before StartTime
        if (availabilityRule.EndTime < availabilityRule.StartTime)
        {
            ModelState.AddModelError("EndTime", "De eind tijd moet hetzelfde of later zijn dan de start tijd");
            return View(availabilityRule);
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

        availabilityRule.Employee = userId;
        availabilityRule.StartTime = availabilityRule.StartTime;
        availabilityRule.EndTime = availabilityRule.EndTime;

        // Check if the model state is still valid before saving to the database
        if (ModelState.IsValid)
        {
            _context.AvailabilityRules.Add(availabilityRule);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(availabilityRule);
    }

    // GET: AvailiabilityController/Edit/5
    [HttpGet("Bewerken")]
    public async Task<IActionResult> Edit(int AvailabilityId, int Year, int WeekNr)
    {
        var availabilityRule = await _context.AvailabilityRules.FindAsync(AvailabilityId);
        if (availabilityRule == null) return NotFound();

        ViewBag.year = Year;
        ViewBag.weekNr = WeekNr;

        // Convert Available or School to Availability
        ViewBag.availability = "Unavailable";
        if (availabilityRule.Available == 1) ViewBag.availability = "Available";
        if (availabilityRule.School == 1) ViewBag.availability = "School";

        return View(availabilityRule);
    }

    // Post: AvailiabilityController/Edit/5
    [ValidateAntiForgeryToken]
    [HttpPost("Bewerken")]
    public async Task<IActionResult> Edit(AvailabilityRule availabilityRule, int Year, int WeekNr, string Availability)
    {
        // Getting user
        var user = await _userManager.GetUserAsync(User);
        int userId = user.Id;

        ViewBag.year = Year;
        ViewBag.weekNr = WeekNr;
        ViewBag.availability = Availability;

        if (availabilityRule == null) return NotFound();

        // Making sure that EndTime is not before StartTime
        if (availabilityRule.EndTime < availabilityRule.StartTime)
        {
            ModelState.AddModelError("EndTime", "De eind tijd moet hetzelfde of later zijn dan de start tijd");
            return View(availabilityRule);
        }

        // Convert Availability to Available or School
        switch (Availability)
        {
            case "Available":
                availabilityRule.Available = 1;
                availabilityRule.School = 0;
                break;
            case "School":
                availabilityRule.Available = 0;
                availabilityRule.School = 1;
                break;
            default:
                availabilityRule.Available = 0;
                availabilityRule.School = 0;
                break;
        }

        availabilityRule.Employee = userId;

        // Check if the model state is still valid before saving to the database
        if (ModelState.IsValid)
        {
            _context.AvailabilityRules.Update(availabilityRule);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(availabilityRule);
    }

    // GET: AvailiabilityController/Delete/5
    [HttpGet("Verwijderen/{AvailabilityId:int}")]
    public async Task<IActionResult> Delete(int AvailabilityId, int Year, int WeekNr)
    {
        var availabilityRule = await _context.AvailabilityRules.FindAsync(AvailabilityId);
        if (availabilityRule == null) return NotFound();

        ViewBag.year = Year;
        ViewBag.weekNr = WeekNr;

        return View(availabilityRule);
    }

    // POST: AvailiabilityController/Delete/5
    [ActionName("Verwijderen")]
    [ValidateAntiForgeryToken]
    [HttpPost("Verwijderen/{AvailabilityId:int}")]
    public async Task<IActionResult> DeleteConfirmed(int AvailabilityId, int Year, int WeekNr)
    {
        var availabilityRule = await _context.AvailabilityRules.FindAsync(AvailabilityId);

        if (availabilityRule != null)
        {
            _context.AvailabilityRules.Remove(availabilityRule);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // Get the date of the first day of the week
    DateOnly FirstDateOfWeek(int year, int week)
    {
        var jan1 = new DateOnly(year, 1, 1);
        var firstDayOfWeek = jan1.AddDays((week - 1) * 7 - (int)jan1.DayOfWeek + (int)DayOfWeek.Monday);

        if (firstDayOfWeek.Year < year) firstDayOfWeek = firstDayOfWeek.AddDays(7);

        return firstDayOfWeek;
    }
}
