using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Controllers
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
            return View(new HolidayViewModel());
        }

		// POST: FeestdagenController/Aanmaken
		[HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult Aanmaken(HolidayViewModel holidayViewModel)
        {
            Holiday holiday = new Holiday();
            holiday.Name = holidayViewModel.Name;

			// Making sure that the Holiday does not already exist
			foreach (Holiday h in _context.Holidays)
			{
				if (h.Name.Equals(holiday.Name))
				{
					ModelState.AddModelError("Name", "Er bestaat al een feestdag met deze naam");
					return View(holidayViewModel);
				}
			}

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
					List<HolidayDay> holidayDays = h.HolidayDays;

					holiday.Holiday = h;
					holiday.FirstDay = holidayDays[0].Date;
					holiday.LastDay = holidayDays[holidayDays.Count() - 1].Date;

					holiday = CreateGraph(holiday);

					break;
				}
			}
			return View(holiday);
		}

		// POST: FeestdagenController/Bewerken/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Bewerken(HolidayManageViewModel HolidayManageViewModel, DateOnly FirstDay, DateOnly LastDay)
		{
			var Holiday = HolidayManageViewModel.Holiday;
			bool changedDates = false;

			// Making sure that FirstDay and LastDay are still in the same year
			if (FirstDay.Year != Holiday.HolidayDays[0].Date.Year)
			{
				HolidayManageViewModel = CreateGraph(HolidayManageViewModel);
				ModelState.AddModelError("FirstDay", "Het gegeven jaar moet hetzelfde zijn als het jaar waarin het feest is gemaakt (" + Holiday.HolidayDays[0].Date.Year + ")");
				return View(HolidayManageViewModel);
			}
			if (LastDay.Year != Holiday.HolidayDays[0].Date.Year)
			{
				HolidayManageViewModel = CreateGraph(HolidayManageViewModel);
				ModelState.AddModelError("LastDay", "Het gegeven jaar moet hetzelfde zijn als het jaar waarin het feest is gemaakt (" + Holiday.HolidayDays[0].Date.Year + ")");
				return View(HolidayManageViewModel);
			}

			// Making sure that LastDay is not before FirstDay
			if (LastDay.DayNumber < FirstDay.DayNumber)
			{
				HolidayManageViewModel = CreateGraph(HolidayManageViewModel);
				ModelState.AddModelError("LastDay", "De laatste dag moet hetzelfde of later zijn dan de eerste dag");
				return View(HolidayManageViewModel);
			}

			_context.Holidays.Update(Holiday);

			// Add or Remove HolidayDays if neccesary
			int firstDayDifference = FirstDay.DayNumber - Holiday.HolidayDays[0].Date.DayNumber;
			int LastDayDifference = LastDay.DayNumber - Holiday.HolidayDays[Holiday.HolidayDays.Count() - 1].Date.DayNumber;

			// Make sure not both dates are changed at the same time
			if (firstDayDifference != 0 && LastDayDifference != 0)
			{
				HolidayManageViewModel = CreateGraph(HolidayManageViewModel);
				ModelState.AddModelError("LastDay", "Je kan niet beide dagen tergelijkertijd aanpassen");
				return View(HolidayManageViewModel);
			}

			// Adding days before
			if (firstDayDifference < 0)
			{
				for (int i = 0; i < Math.Abs(firstDayDifference); i++)
				{
					HolidayDay holidayDay = new HolidayDay();
					holidayDay.Date = Holiday.HolidayDays[0].Date.AddDays(-i - 1);
					holidayDay.Impact = 0;
					holidayDay.HolidayName = Holiday.Name;

					_context.HolidayDays.Add(holidayDay);
				}

				HolidayManageViewModel.LastDay = LastDay;
				changedDates = true;
			}

			// Removing days before
			if (firstDayDifference > 0)
			{
				for (int i = 0; i < firstDayDifference; i++)
				{
					HolidayDay holidayDay = Holiday.HolidayDays[i];

					_context.HolidayDays.Remove(holidayDay);
				}

				HolidayManageViewModel.LastDay = LastDay;
				changedDates = true;
			}

			// Adding days after
			if (LastDayDifference > 0)
			{
				int holidayDays = Holiday.HolidayDays.Count() - 1;

				for (int i = 0; i < Math.Abs(LastDayDifference); i++)
				{
					HolidayDay holidayDay = new HolidayDay();
					holidayDay.Date = Holiday.HolidayDays[holidayDays].Date.AddDays(i + 1);
					holidayDay.Impact = 0;
					holidayDay.HolidayName = Holiday.Name;

					_context.HolidayDays.Add(holidayDay);
				}

				HolidayManageViewModel.LastDay = LastDay;
				changedDates = true;
			}

			// Removing days after
			if (LastDayDifference < 0)
			{
				int holidayDays = Holiday.HolidayDays.Count();

				for (int i = 0; i < Math.Abs(LastDayDifference); i++)
				{
					HolidayDay holidayDay = Holiday.HolidayDays[holidayDays - 1 - i];

					_context.HolidayDays.Remove(holidayDay);
				}

				HolidayManageViewModel.LastDay = LastDay;
				changedDates = true;
			}

			await _context.SaveChangesAsync();

			// If dates have been changed the user goes back to the index to make sure the new HolidayDays are handled correctly
			if (changedDates == true) return RedirectToAction(nameof(Index));
			else
			{
				HolidayManageViewModel = CreateGraph(HolidayManageViewModel);
				return View(HolidayManageViewModel);
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

		// Create the graph requried for Bewerken
		public HolidayManageViewModel CreateGraph(HolidayManageViewModel HolidayManageViewModel)
		{
			Holiday Holiday = HolidayManageViewModel.Holiday;

			if (HolidayManageViewModel != null)
			{
				if (Holiday.HolidayDays.Count > 1)
				{
					foreach (HolidayDay holidayDay in Holiday.HolidayDays)
					{
						HolidayManageViewModel.xValues.Add(holidayDay.Date.Day + "-" + holidayDay.Date.Month);
						HolidayManageViewModel.yValues.Add(holidayDay.Impact);

						if (HolidayManageViewModel.HighestImpact < holidayDay.Impact) HolidayManageViewModel.HighestImpact = holidayDay.Impact;
						else if (HolidayManageViewModel.LowestImpact > holidayDay.Impact) HolidayManageViewModel.LowestImpact = holidayDay.Impact;
					}
				}
				else
				{
					HolidayManageViewModel.HighestImpact = 0;
					HolidayManageViewModel.LowestImpact = 0;
				}
			}

			return HolidayManageViewModel;
		}
	}
}
