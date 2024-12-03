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
	[Route("Rooster")]
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
		public async Task<IActionResult> Details(int? id)
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

			Dictionary<Int32, String> employeeUsers = _context.Users
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
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[ValidateAntiForgeryToken]
		[HttpPost("MedewerkerInplannen/{weekId:int}")]
		public async Task<IActionResult> Create(int weekId, ShiftCreateViewModel shiftCreateViewModel)
		{
			if (ModelState.IsValid)
			{
				if (shiftCreateViewModel.Shift.EmployeeId == -1) shiftCreateViewModel.Shift.EmployeeId = null;
				_context.Add(shiftCreateViewModel.Shift);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewBag.Departments = new SelectList(_context.Departments, "Name", "Name", shiftCreateViewModel.Shift.Department);
			ViewBag.WeekDays = new SelectList(_context.Weeks, "Id", "Id", shiftCreateViewModel.Shift.WeekId);
			return View(shiftCreateViewModel);
		}

		// GET: Shifts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var shift = await _context.Shifts.FindAsync(id);
			if (shift == null)
			{
				return NotFound();
			}
			ViewData["Department"] = new SelectList(_context.Departments, "Name", "Name", shift.Department);
			ViewData["WeekId"] = new SelectList(_context.Weeks, "Id", "Id", shift.WeekId);
			return View(shift);
		}

		// POST: Shifts/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,WeekId,Weekday,Department,StartTime,EndTime,Employee,ExternalEmployeeName")] ShiftCreateViewModel shift)
		{
			if (id != shift.Shift.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(shift);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ShiftExists(shift.Shift.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["Department"] = new SelectList(_context.Departments, "Name", "Name", shift.Shift.Department);
			ViewData["WeekId"] = new SelectList(_context.Weeks, "Id", "Id", shift.Shift.WeekId);
			return View(shift);
		}

		// GET: Shifts/Delete/5
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
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var shift = await _context.Shifts.FindAsync(id);
			if (shift != null)
			{
				_context.Shifts.Remove(shift);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ShiftExists(int id)
		{
			return _context.Shifts.Any(e => e.Id == id);
		}
	}
}
