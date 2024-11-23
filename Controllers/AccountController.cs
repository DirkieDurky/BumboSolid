using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BumboSolid.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Employee> _signInManager;
        private readonly UserManager<Employee> _userManager;

        public AccountController(
            UserManager<Employee> userManager,
            SignInManager<Employee> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = new Employee
                {
                    UserName = input.Email,
                    Email = input.Email,
                    FirstName = input.FirstName,
                    LastName = input.LastName
                };

                var result = await _userManager.CreateAsync(user, input.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect("/Prognoses/Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View();
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel input)
        {
            if (input == null || string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password))
            {
                ModelState.AddModelError(string.Empty, "Email and Password are required.");
                return View(input);
            }

            var user = await _userManager.FindByEmailAsync(input.Email);

            if (user == null)
            {
                // Log this for debugging
                Console.WriteLine($"User not found: {input.Email}");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(input);
            }

            var result = await _signInManager.PasswordSignInAsync(user, input.Password, true, false);

            // Log the result for debugging
            Console.WriteLine($"PasswordSignInAsync result: {result}");

            if (result.Succeeded)
            {
                return Redirect("/Prognoses/Index");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account has been locked due to multiple failed login attempts.");
                return View(input);
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "You are not allowed to login.");
                return View(input);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(input);
            }
        }


        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}
