using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BumboSolid.Controllers;

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
                return Redirect("/RoosterMedewerker");
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

    [HttpGet]
    public async Task<IActionResult> Details()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound();
        }

        if (await _userManager.IsInRoleAsync(user, "Manager"))
        {
            TempData["ErrorMessage"] = "Managers mogen hun wachtwoord niet wijzigen.";
            return RedirectToAction("Details");
        }

        if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
        {
            ModelState.AddModelError(string.Empty, "Alle velden zijn verplicht.");
            return View("Details", user);
        }

        if (NewPassword != ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "De nieuwe wachtwoorden komen niet overeen.");
            return View("Details", user);
        }

        if (!PasswordIsStrongEnough(NewPassword))
        {
            ModelState.AddModelError(string.Empty, "Wachtwoord voldoet niet aan de eisen: minimaal 8 tekens, 1 hoofdletter, 1 kleine letter, 1 cijfer en 1 speciaal karakter.");
            return View("Details", user);
        }

        var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                var translatedMessage = error.Code switch
                {
                    "PasswordMismatch" => "Het huidige wachtwoord is onjuist.",
                    _ => error.Description
                };
                ModelState.AddModelError(string.Empty, translatedMessage);
            }
            return View("Details", user);
        }

        TempData["SuccessMessage"] = "Wachtwoord succesvol gewijzigd.";
        return RedirectToAction("Details");
    }

    public bool PasswordIsStrongEnough(string password)
    {
        string passwordRegex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+\\-=\\[\\]{}\\|`~;:'\",.<>])[A-Za-z\\d!@#$%^&*()_+\\-=\\[\\]{}\\|`~;:'\",.<>]{8,}$";
        return Regex.IsMatch(password, passwordRegex);
    }
}
