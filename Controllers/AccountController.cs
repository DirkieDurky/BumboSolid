using BumboSolid.Data.Models;
using BumboSolid.Migrations;
using BumboSolid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel Input)
        {
            if (!ModelState.IsValid)
            {
                return View(Input);
            }

            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, true, false);

            if (result.Succeeded)
            {
                if (User.IsInRole("Manager"))
                {
                    return Redirect("/Prognoses");
                }
                if (User.IsInRole("Employee"))
                {
                    return Redirect("/Roosters");
                }
                ViewBag.Error = "Geen rol";
                return View(Input);
            }
            else
            {
                ViewBag.Error = "Het ingevoerde wachtwoord of e-mailadres is onjuist.";
                return View(Input);
            }
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
