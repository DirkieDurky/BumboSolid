﻿using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Web.Controllers
{
	public class FeestdagenController : Controller
	{
		private readonly BumboDbContext _context;

		public FeestdagenController(BumboDbContext context)
		{
			_context = context;
		}

		// GET: FeestdagenController
		public ActionResult Index()
		{
			List<HolidayViewModel> holidayViewModels = new List<HolidayViewModel>();

			foreach (Holiday holiday in _context.Holidays.Include(x => x.HolidayDays).ToList())
			{
				List<HolidayDay> holidayDays = holiday.HolidayDays.ToList();

				DateOnly firstDay = holidayDays[0].Date;
				DateOnly lastDay = holidayDays[holidayDays.Count()-1].Date;

				HolidayViewModel holidayViewModel = new HolidayViewModel() { Name = holiday.Name, FirstDay = firstDay, LastDay = lastDay };

				holidayViewModels.Add(holidayViewModel);
			}

			return View(holidayViewModels);
		}

		// GET: FeestdagenController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: FeestdagenController/Aanmaken
		public ActionResult Aanmaken()
		{
			return View();
		}

		// POST: FeestdagenController/Aanmaken
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Aanmaken(IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: FeestdagenController/Bewerken/5
		public ActionResult Bewerken(String id)
		{
			HolidayManageViewModel holiday = new HolidayManageViewModel();

            foreach(Holiday h in _context.Holidays.Include(x => x.HolidayDays).ToList())
			{
				if (id.Equals(h.Name))
				{
					List<HolidayDay> holidayDays = h.HolidayDays.ToList();

					holiday.Holiday = h;
					holiday.FirstDay = holidayDays[0].Date;
					holiday.LastDay = holidayDays[holidayDays.Count() - 1].Date;

					if (holidayDays.Count > 1)
					{
						foreach (HolidayDay holidayDay in holidayDays)
						{
							holiday.xValues.Add(holidayDay.Date.Day + "-" + holidayDay.Date.Month);
							holiday.yValues.Add(holidayDay.Impact);

							if (holiday.HighestImpact < holidayDay.Impact) holiday.HighestImpact = holidayDay.Impact;
							else if (holiday.LowestImpact > holidayDay.Impact) holiday.LowestImpact = holidayDay.Impact;
						}
					} else
					{
						holiday.HighestImpact = 0;
						holiday.LowestImpact = 0;
					}

					break;
				}
			}
			return View(holiday);
		}

		// POST: FeestdagenController/Bewerken/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Bewerken(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: FeestdagenController/Verwijderen/5
		public async Task<IActionResult> Verwijderen(String Name)
		{
			if (Name == null)
			{
				return NotFound();
			}

			var holiday = await _context.Holidays.FirstOrDefaultAsync(h => h.Name == Name);
			if (holiday == null)
			{
				return NotFound();
			}

			return View(holiday);
		}

        // POST: FeestdagenController/Verwijderen/5
        [HttpPost, ActionName("Verwijderen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerwijderenConfirmed(String Name)
        {
            var holiday = await _context.Holidays.Include(h => h.HolidayDays).FirstOrDefaultAsync(h => h.Name == Name);

            if (holiday != null)
            {
                foreach (HolidayDay holidayDay in holiday.HolidayDays)
                {
                    _context.HolidayDays.Remove(holidayDay);
                }

                _context.Holidays.Remove(holiday);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));
        }
	}
}
