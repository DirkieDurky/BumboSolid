﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;

namespace BumboSolid.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly BumboDbContext _context;

        public EmployeesController(BumboDbContext context)
        {
            _context = context;
        }

        // Displays the list of all employees with related data.
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.AvailabilityDays)
                .Include(e => e.FillRequests)
                .ToListAsync();

            return View(employees);
        }

        // Provides the view to create a new employee.
        public IActionResult Create()
        {
            return View();
        }

        // Processes the data submitted for creating a new employee.
        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // Retrieves the details of an employee for editing.
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                // Change to index screen?
                return NotFound();
            }
            return View(employee);
        }

        // Processes the data submitted to update an employee's information.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.PlaceOfResidence = employee.PlaceOfResidence;
            existingEmployee.StreetName = employee.StreetName;
            existingEmployee.StreetNumber = employee.StreetNumber;
            existingEmployee.BirthDate = employee.BirthDate;
            existingEmployee.EmployedSince = employee.EmployedSince;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Checks if an employee exists in the database by ID.
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.AspNetUserId == id);
        }
    }
}
