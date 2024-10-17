using BumboSolid.Data;
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

				DateOnly firstDay = holidayDays.Min(x => x.Date);

				DateOnly lastDay = holidayDays.Max(x => x.Date);

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
		public ActionResult Bewerken(int id)
		{
			return View();
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
