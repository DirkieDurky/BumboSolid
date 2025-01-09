using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using BumboSolid.Migrations;

namespace BumboSolid.Controllers
{
    [Authorize(Roles = "Manager")]
    [Route("Medewerkers")]
    public class EmployeesController : Controller
    {
        private readonly BumboDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private const int MinAge = 13;
        private const int MaxAge = 128;

        private const int MinEmployedYears = 0;
        private const int MaxEmployedYears = 128;

        public EmployeesController(BumboDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // Only show the users with the employee role, because the manager may not be deleted.
            var employeeRole = await _context.Roles
                .Where(r => r.Name == "Employee")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (employeeRole == null)
            {
                ViewBag.NoEmployees = "No employees found.";
                return View(new List<User>());
            }

            var employees = await _context.UserRoles
                .Where(ur => ur.RoleId == employeeRole)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var employeeUsers = await _context.Users
                .Where(u => employees.Contains(u.Id))
                .Include(u => u.Departments)
                .ToListAsync();

            return View(employeeUsers);
        }

        [HttpGet("Aanmaken")]
        public async Task<IActionResult> Create()
        {
            var departments = await _context.Departments.ToListAsync();

            EmployeesCreateViewModel employeesCreateViewModel = new EmployeesCreateViewModel();
            employeesCreateViewModel.Departments = departments;

            return View(employeesCreateViewModel);
        }

        [HttpPost("Aanmaken")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeesCreateViewModel input)
        {
            if (await _userManager.FindByEmailAsync(input.Email) != null)
            {
                ModelState.AddModelError(nameof(input.Email), $"De email '{input.Email}' is al in gebruik.");
            }

            // Validate Age
            if (!IsValidAge(input.BirthDate, out var ageErrorMessage))
            {
                ModelState.AddModelError(nameof(input.BirthDate), ageErrorMessage);
            }

            // Validate Employment Duration
            if (!IsValidEmploymentDuration(input.EmployedSince, input.BirthDate, out var employmentErrorMessage))
            {
                ModelState.AddModelError(nameof(input.EmployedSince), employmentErrorMessage);
            }

            if (!PasswordIsStrongEnough(input.Password))
            {
                ModelState.AddModelError(nameof(input.Password), "Wachtwoord is niet sterk genoeg. Zorg dat uw wachtwoord voldoet aan de aangegeven regels. Het speciale karakter moet één van de volgende karakters zijn (!@#$%^&*()_+-=[]{}|`~)");
            }

            if (input.Password != input.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(input.Password), "De wachtwoorden komen niet overeen.");
            }

            if (!input.SelectedDepartments.Any())
            {
                ModelState.AddModelError("SelectedDepartments", "Kies minstens één afdeling.");
            }

            if (ModelState.IsValid)
            {
                var user = new User
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
                    Departments = await _context.Departments
                    .Where(d => input.SelectedDepartments.Contains(d.Name))
                    .ToListAsync(),
                };

                var result = await _userManager.CreateAsync(user, input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Employee");
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

            input.Departments = await _context.Departments.ToListAsync();
            return View(input);
        }


        [HttpGet("Bewerken/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Users
                .Include(u => u.Departments)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var departments = await _context.Departments.ToListAsync();

            var model = new EmployeesEditViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PlaceOfResidence = employee.PlaceOfResidence,
                StreetName = employee.StreetName,
                StreetNumber = employee.StreetNumber,
                BirthDate = employee.BirthDate,
                EmployedSince = employee.EmployedSince,
                Departments = departments,
                SelectedDepartments = employee.Departments.Select(d => d.Name).ToList()
            };

            return View(model);
        }

        [HttpPost("Bewerken/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeesEditViewModel model)
        {
            if (!model.SelectedDepartments.Any())
            {
                ModelState.AddModelError("SelectedDepartments", "Kies minstens één afdeling.");
            }

            // Validate Age
            if (!IsValidAge(model.BirthDate, out var ageErrorMessage))
            {
                ModelState.AddModelError(nameof(model.BirthDate), ageErrorMessage);
            }

            // Validate Employment Duration
            if (!IsValidEmploymentDuration(model.EmployedSince, model.BirthDate, out var employmentErrorMessage))
            {
                ModelState.AddModelError(nameof(model.EmployedSince), employmentErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                model.Departments = await _context.Departments.ToListAsync();
                return View(model);
            }

            var existingEmployee = await _context.Users
                .Include(u => u.Departments)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.FirstName = model.FirstName;
            existingEmployee.LastName = model.LastName;
            existingEmployee.PlaceOfResidence = model.PlaceOfResidence;
            existingEmployee.StreetName = model.StreetName;
            existingEmployee.StreetNumber = model.StreetNumber;
            existingEmployee.BirthDate = model.BirthDate;
            existingEmployee.EmployedSince = model.EmployedSince;

            // Check if the email has been changed and if it's already taken by another user
            if (existingEmployee.Email != model.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != id))
                {
                    ModelState.AddModelError(nameof(model.Email), $"De email '{model.Email}' is al in gebruik.");
                    model.Departments = await _context.Departments.ToListAsync();
                    return View(model);
                }

                existingEmployee.Email = model.Email;
            }

            // Password logic
            if (!string.IsNullOrEmpty(model.Password))
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError(nameof(model.Password), "De wachtwoorden komen niet overeen.");
                    model.Departments = await _context.Departments.ToListAsync();
                    return View(model);
                }

                if (!PasswordIsStrongEnough(model.Password))
                {
                    ModelState.AddModelError(nameof(model.Password), "Wachtwoord is niet niet sterk genoeg. Zorg dat uw wachtwoord voldoet aan de volgende regels:\n" +
                        "Minimaal 8 karakters.\n" +
                        "Minimaal 1 cijfer.\n" +
                        "Minimaal 1 kleine letter.\n" +
                        "Minimaal 1 hoofdletter.\n" +
                        "Minimaal 1 speciaal karakter. (Één van de volgende: !@#$%^&*()_+-=[]{}|`~)");
                }

                if (ModelState.IsValid)
                {
                    var passwordHasher = new PasswordHasher<User>();
                    existingEmployee.PasswordHash = passwordHasher.HashPassword(existingEmployee, model.Password);
                }
                else
                {
                    model.Departments = await _context.Departments.ToListAsync();
                    return View(model);
                }
            }

            // Update employee departments
            var selectedDepartments = await _context.Departments
                .Where(d => model.SelectedDepartments.Contains(d.Name))
                .ToListAsync();

            existingEmployee.Departments = selectedDepartments;

            _context.Users.Update(existingEmployee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        [HttpGet("Verwijderen/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees
                .Include(u => u.Departments)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost("Verwijderen/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee != null)
            {
                // Delete refrences to employee
                foreach (Shift shift in _context.Shifts.Where(e => e.Employee == employee).ToList())
                {
                    foreach (FillRequest fillRequest in _context.FillRequests.Where(s => s.Shift == shift).ToList()) _context.FillRequests.Remove(fillRequest);
                    _context.Shifts.Remove(shift);
                }
                foreach (AvailabilityRule availabilityRule in _context.AvailabilityRules.Where(e => e.Employee == employee.Id).ToList()) _context.AvailabilityRules.Remove(availabilityRule);

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool IsValidAge(DateOnly birthDate, out string errorMessage)
        {
            errorMessage = null;

            var age = DateTime.Today.Year - birthDate.Year;
            if (birthDate > DateOnly.FromDateTime(DateTime.Today.AddYears(-age)))
                age--;

            if (age < MinAge || age > MaxAge)
            {
                errorMessage = $"Leeftijd moet tussen {MinAge} en {MaxAge} jaar zijn. Huidige leeftijd: {age}.";
                return false;
            }

            return true;
        }

        private bool IsValidEmploymentDuration(DateOnly employedSince, DateOnly birthDate, out string errorMessage)
        {
            errorMessage = null;

            if (employedSince < birthDate)
            {
                errorMessage = "De datum 'In dienst sinds' kan niet eerder zijn dan de geboortedatum.";
                return false;
            }

            var yearsEmployed = DateTime.Today.Year - employedSince.Year;
            if (employedSince > DateOnly.FromDateTime(DateTime.Today.AddYears(-yearsEmployed)))
                yearsEmployed--;

            if (yearsEmployed < MinEmployedYears || yearsEmployed > MaxEmployedYears)
            {
                errorMessage = $"Dienstjaren moeten tussen {MinEmployedYears} en {MaxEmployedYears} jaar zijn. Huidige dienstjaren: {yearsEmployed}.";
                return false;
            }

            return true;
        }

        public bool PasswordIsStrongEnough(string password)
        {
            string passwordRegex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+\\-=\\[\\]{}\\|`~;:'\",.<>])[A-Za-z\\d!@#$%^&*()_+\\-=\\[\\]{}\\|`~;:'\",.<>]{8,}$";
            return Regex.IsMatch(password, passwordRegex);
        }

    }
}
