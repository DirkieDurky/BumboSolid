using System.Globalization;
using System;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

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
					// Getting correct date
					var jan1 = new DateOnly(shift.Week.Year, 1, 1);
					DateOnly date = jan1.AddDays(DayOfWeek.Monday - jan1.DayOfWeek).AddDays((shift.Week.WeekNumber - 1) * 7).AddDays((int)shift.Weekday - (int)DayOfWeek.Monday);

                    // Creating FillReuestViewModel
                    FillRequestViewModel fillRequestViewModel = new FillRequestViewModel()
                    {
                        Date = date,
                        Day = shift.Weekday,
						StartTime = shift.StartTime,
                        EndTime = shift.EndTime,

						Department = shift.Department,
                        Accepted = fillRequest.Accepted

                    };
				}
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
