using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                Console.WriteLine(holiday.Name);
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
			return View(new HolidayViewModel());
		}

		// POST: FeestdagenController/Aanmaken
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Aanmaken(HolidayViewModel holidayViewModel)
		{
			Holiday holiday = new Holiday();
			holiday.Name = holidayViewModel.Name;

			// Making sure that LastDay is not before FirstDay
			if (holidayViewModel.LastDay.DayNumber < holidayViewModel.FirstDay.DayNumber)
			{
				ModelState.AddModelError("LastDay", "De laatste dag moet hetzelfde of later zijn dan de eerste dag");
				return View(holidayViewModel);
			}

			// Adding HolidayDay for every day the holiday is active
			for (int i = 0; i <= holidayViewModel.LastDay.DayNumber - holidayViewModel.FirstDay.DayNumber; i++)
			{
				HolidayDay holidayDay = new HolidayDay();
				holidayDay.HolidayName = holidayViewModel.Name;
				holidayDay.Date = holidayViewModel.FirstDay.AddDays(i);
				holidayDay.Impact = 0;
				holidayDay.HolidayNameNavigation = holiday;
				holiday.HolidayDays.Add(holidayDay);
			}

			// Check if the model state is still valid before saving to the database
			if (ModelState.IsValid)
			{
				_context.Holidays.Add(holiday);
				_context.SaveChanges();
				return RedirectToAction(nameof(Index));
			}

			return View(holidayViewModel);
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
