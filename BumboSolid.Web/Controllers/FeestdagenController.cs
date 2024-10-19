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

				HolidayViewModel holidayViewModel = new HolidayViewModel() { Holiday = holiday, FirstDay = firstDay, LastDay = lastDay };

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
			HolidayViewModel holiday = new HolidayViewModel();

            foreach(Holiday h in _context.Holidays.Include(x => x.HolidayDays).ToList())
			{
				if (id.Equals(h.Name))
				{
					List<HolidayDay> holidayDays = h.HolidayDays.ToList();

					holiday.Holiday = h;
					holiday.FirstDay = holidayDays[0].Date;
					holiday.LastDay = holidayDays[holidayDays.Count() - 1].Date;

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
		public ActionResult Verwijderen(int id)
		{
			return View();
		}

		// POST: FeestdagenController/Verwijderen/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Verwijderen(int id, IFormCollection collection)
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
	}
}
