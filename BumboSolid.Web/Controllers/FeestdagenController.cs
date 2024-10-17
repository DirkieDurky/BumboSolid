using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BumboSolid.Web.Controllers
{
	public class FeestdagenController : Controller
	{
		// GET: FeestdagenController
		public ActionResult Index()
		{
			return View();
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
