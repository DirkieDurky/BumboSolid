using Microsoft.AspNetCore.Mvc;

namespace BumboSolid.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            if (User.IsInRole("Manager"))
            {
                return RedirectToAction("Index", "Prognoses");
            }
            if (User.IsInRole("Employee"))
            {
                return RedirectToAction("EmployeeSchedule", "ScheduleEmployee");
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
