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

        // GET: Shifts/Create
        [HttpGet("MedewerkerInplannen/{weekId:int}")]
        public async Task<IActionResult> Create(int weekId)
        {
            var week = _context.Weeks.First(w => w.Id == weekId);
            if (week == null) return NotFound();

            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");

            var viewModel = new ShiftCreateViewModel
            {
                Employees = _context.Employees.ToList(),
                Shift = new Shift(),
                Week = week,
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
            shiftCreateViewModel.Employees = await _context.Employees.ToListAsync();

            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
            ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });
            
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

            var viewModel = new ShiftCreateViewModel
            {
                Shift = shift,
                Employees = await _context.Employees.ToListAsync(),
                Week = shift.Week
            };

            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");

            return View(viewModel);
        }

        // POST: Shifts/Edit/5
        [HttpPost("Bewerken/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShiftCreateViewModel shiftCreateViewModel)
        {
            if (id != shiftCreateViewModel.Shift.Id)
            {
                return NotFound();
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
}