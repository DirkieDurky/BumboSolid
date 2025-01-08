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
        public async Task<IActionResult> Create(int weekId, string returnUrl)
        {
            var week = _context.Weeks.First(w => w.Id == weekId);

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");

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
                .Include(u => u.AvailabilityRules).ToList()
                .ToDictionary(u => u.Id, u => u.FirstName);

            var viewModel = new ShiftCreateViewModel
            {
                Employees = employeeUsers,
                Shift = new Shift(),
                Week = week,
            };

            employeeUsers.Add(-1, "Extern filiaal");

            return View(viewModel);
        }

        // POST: Shifts/Create
        [HttpPost("MedewerkerInplannen/{weekId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int weekId, ShiftCreateViewModel shiftCreateViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (shiftCreateViewModel.Shift.EndTime <= shiftCreateViewModel.Shift.StartTime)
                {
                    ViewBag.Error = "De eindtijd moet later zijn dan de starttijd.";
                }
                else
                {
                    if (shiftCreateViewModel.Shift.EmployeeId == -1) shiftCreateViewModel.Shift.EmployeeId = null;
                    _context.Add(shiftCreateViewModel.Shift);
                    await _context.SaveChangesAsync();
                    return Redirect(returnUrl);
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

            Dictionary<Int32, String> employeeUsers = _context.Users
                .Where(u => employees.Contains(u.Id))
                .Include(u => u.AvailabilityRules).ToList()
                .ToDictionary(u => u.Id, u => u.FirstName);

            shiftCreateViewModel.Week = week;
            shiftCreateViewModel.Employees = employeeUsers;

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
            ViewBag.WeekDays = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

            return View(shiftCreateViewModel);
        }


        // GET: Shifts/Edit/5
        [HttpGet("Bewerken/{id:int}")]
        public async Task<IActionResult> Edit(int? id, string returnUrl)
        {
            if (id == null || _context.Shifts.FirstOrDefault(s => s.Id == id) == null)
            {
                return NotFound();
            }

            var shift = _context.Shifts.Include(s => s.Week).First(s => s.Id == id);

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name");

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
                .Include(u => u.AvailabilityRules).ToList()
                .ToDictionary(u => u.Id, u => u.FirstName);

            var viewModel = new ShiftCreateViewModel
            {
                Employees = employeeUsers,
                Shift = shift,
                Week = shift.Week
            };

            employeeUsers.Add(-1, "Extern filiaal");
            shift.EmployeeId ??= -1;

            return View(viewModel);
        }

        // POST: Shifts/Edit/5
        [HttpPost("Bewerken/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShiftCreateViewModel shiftCreateViewModel, string returnUrl)
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

                    if (shiftCreateViewModel.Shift.EndTime <= shiftCreateViewModel.Shift.StartTime)
                    {
                        ViewBag.Error = "De eindtijd moet later zijn dan de starttijd.";
                        return View(shiftCreateViewModel);
                    }

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
                return Redirect(returnUrl);
            }
            ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
            ViewBag.WeekDays = new SelectList(_context.Weeks, "Id", "Id", shiftCreateViewModel.Shift.WeekId);
            ViewBag.ReturnUrl = returnUrl;
            return View(shiftCreateViewModel);
        }

        // GET: Shifts/Delete/5
        [HttpGet("Verwijderen/{id:int}")]
        public async Task<IActionResult> Delete(int? id, string returnUrl)
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
            ViewBag.ReturnUrl = returnUrl;

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