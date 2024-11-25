using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Controllers
{
    [Authorize(Roles = "Manager")]
    [Route("Schedule")]
	public class ScheduleEmployeeController : Controller
	{
		private readonly BumboDbContext _context;

		public ScheduleEmployeeController(BumboDbContext context)
		{
			_context = context;
		}

        // GET: ScheduleEmployeeController
        [HttpGet("")]
		public ActionResult Schedule()
		{
            return View();
		}

        // GET: ScheduleEmployeeController/OutgoingFillRequests
        [HttpGet("Uitgaande invalsverzoeken")]
        public ActionResult OutgoingFillRequests()
        {
            var shifts = _context.Shifts.ToList(); //.Where(e => e.Employee == User.Id)

            List<FillRequestViewModel> fillRequests = new List<FillRequestViewModel>();

			foreach (Shift shift in shifts)
            {
                foreach (FillRequest fillRequest in shift.FillRequests)
                {
					FillRequestViewModel fillRequestViewModel = new FillRequestViewModel()
					{


				     };
				}
			}
			}

            return View(shifts);
        }

        // GET: ScheduleEmployeeController/IncomingFillRequests
        [HttpGet("Inkomende invalsverzoeken")]
        public ActionResult IncomingFillRequests()
        {
            return View();
        }
    }
}
