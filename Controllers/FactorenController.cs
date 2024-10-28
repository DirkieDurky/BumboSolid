using Microsoft.AspNetCore.Mvc;

namespace BumboSolid.Controllers
{
	public class FactorenController : Controller
	{
		// GET: FactorenController
		public ActionResult Index()
		{
			return View();
		}

		// GET: FactorenController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: FactorenController/Aanmaken
		public ActionResult Aanmaken()
		{
			return View();
		}

		// POST: FactorenController/Aanmaken
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

		// GET: FactorenController/Bewerken/5
		public ActionResult Bewerken(int id)
		{
			return View();
		}

		// POST: FactorenController/Bewerken/5
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

		// GET: FactorenController/Verwijderen/5
		public ActionResult Verwijderen(int id)
		{
			return View();
		}

		// POST: FactorenController/Verwijderen/5
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
