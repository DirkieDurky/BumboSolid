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
		public async Task<IActionResult> Index()
		{
			var bumboDbContext = _context.Weeks.Include(w => w.Shifts);
			return View(await bumboDbContext.ToListAsync());
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
		[HttpGet("Medewerker Inplannen")]
		public async Task<IActionResult> Create(Int16 year, Int16 week)
		{
			ViewData["Department"] = new SelectList(_context.Departments, "Name", "Name");
			ViewData["WeekId"] = new SelectList(new List<string> { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" });

			var employeeRole = await _context.Roles
				.Where(r => r.Name == "Employee")
				.Select(r => r.Id)
				.FirstOrDefaultAsync();

			var employees = await _context.UserRoles
				.Where(ur => ur.RoleId == employeeRole)
				.Select(ur => ur.UserId)
				.ToListAsync();

			var employeeUsers = await _context.Users
				.Where(u => employees.Contains(u.Id))
				.Include(u => u.AvailabilityRules)
				.ToListAsync();

			var viewModel = new ShiftCreateViewModel
			{
				Employees = employeeUsers,
				Shift = new Shift(),
				Year = year,
				Week = week
			};

			employeeUsers.Insert(0, new User
			{
				FirstName = "Extern",
				LastName = "filiaal"
			});

			// Create a SelectList from the employees list
			ViewData["Employees"] = new SelectList(employeeUsers, "Id", "Name");
			return View(viewModel);
		}

		// POST: Shifts/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,WeekId,Weekday,Department,StartTime,EndTime,Employee,ExternalEmployeeName")] ShiftCreateViewModel shift)
		{
			if (ModelState.IsValid)
			{
				_context.Add(shift);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["Department"] = new SelectList(_context.Departments, "Name", "Name", shift.Shift.Department);
			ViewData["WeekId"] = new SelectList(_context.Weeks, "Id", "Id", shift.Shift.WeekId);
			return View(shift);
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
