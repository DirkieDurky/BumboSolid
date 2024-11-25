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
					// Getting correct date and day
					var jan1 = new DateOnly(shift.Week.Year, 1, 1);
					DateOnly date = jan1.AddDays(DayOfWeek.Monday - jan1.DayOfWeek).AddDays((shift.Week.WeekNumber - 1) * 7).AddDays((int)shift.Weekday - (int)DayOfWeek.Monday);

					string[] days = ["Monday", "Tuesdday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

					// Creating FillReuestViewModel
					FillRequestViewModel fillRequestViewModel = new FillRequestViewModel()
                    {
                        Date = date,
                        Day = days[shift.Weekday],
						StartTime = shift.StartTime,
                        EndTime = shift.EndTime,

						Department = shift.Department,
                        Accepted = Convert.ToBoolean(fillRequest.Accepted)
					};

                    fillRequests.Add(fillRequestViewModel);
				}
			};

            return View(fillRequests);
        }

        // GET: ScheduleEmployeeController/IncomingFillRequests
        [HttpGet("Inkomende invalsverzoeken")]
        public ActionResult IncomingFillRequests()
        {
			var shifts = _context.Shifts.ToList(); //.Where(e => e.Employee != User.Id)

			List<FillRequestViewModel> fillRequests = new List<FillRequestViewModel>();

			foreach (Shift shift in shifts)
			{
				foreach (FillRequest fillRequest in shift.FillRequests)
				{
					// Getting correct date and day
					var jan1 = new DateOnly(shift.Week.Year, 1, 1);
					DateOnly date = jan1.AddDays(DayOfWeek.Monday - jan1.DayOfWeek).AddDays((shift.Week.WeekNumber - 1) * 7).AddDays((int)shift.Weekday - (int)DayOfWeek.Monday);

					string[] days = ["Monday", "Tuesdday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

					// Creating FillReuestViewModel
					FillRequestViewModel fillRequestViewModel = new FillRequestViewModel()
					{
						Date = date,
						Day = days[shift.Weekday],
						StartTime = shift.StartTime,
						EndTime = shift.EndTime,

						Department = shift.Department,
						Name = _context.Employees.FirstOrDefault(i => i.Id == shift.Employee).FirstName + " " + _context.Employees.FirstOrDefault(i => i.Id == shift.Employee).LastName
					};

					fillRequests.Add(fillRequestViewModel);
				}
			};

			return View(fillRequests);
		}
    }
}
