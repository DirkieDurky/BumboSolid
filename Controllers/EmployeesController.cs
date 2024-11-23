using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Identity;

namespace BumboSolid.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly BumboDbContext _context;
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;

        public EmployeesController(BumboDbContext context, UserManager<Employee> userManager, SignInManager<Employee> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Displays the list of all employees with related data.
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.AvailabilityRules)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeesCreateViewModel input)
        {
            var existingUser = await _userManager.FindByEmailAsync(input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already in use.");
            }

            if (ModelState.IsValid)
            {
                var user = new Employee
                {
                    UserName = input.Email,
                    Email = input.Email,
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    PlaceOfResidence = input.PlaceOfResidence,
                    StreetName = input.StreetName,
                    StreetNumber = input.StreetNumber,
                    BirthDate = input.BirthDate,
                    EmployedSince = input.EmployedSince,
                    NormalizedEmail = input.Email.ToUpper(),
                    NormalizedUserName = input.Email.ToUpper()
                };

                var result = await _userManager.CreateAsync(user, input.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(input);
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
            return _context.Employees.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Delete(int id)
		{
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

			return View(employee);
		}

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
