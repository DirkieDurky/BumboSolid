using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BumboSolid.Web.Controllers
{
	public class WeerController : Controller
	{
		// GET: WeerController
		public ActionResult Index()
		{
			return View();
		}

		// GET: WeerController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: WeerController/Aanmaken
		public ActionResult Aanmaken()
		{
			return View();
		}

		// POST: WeerController/Aanmaken
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

		// GET: WeerController/Bewerken/5
		public ActionResult Bewerken(int id)
		{
			return View();
		}

		// POST: WeerController/Bewerken/5
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

		// GET: WeerController/Verwijderen/5
		public ActionResult Verwijderen(int id)
		{
			return View();
		}

		// POST: WeerController/Verwijderen/5
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
