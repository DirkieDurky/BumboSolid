using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BumboSolid.Web.Controllers
{
	public class NormeringenController : Controller
	{
		// GET: Normeringen
		public ActionResult Index()
		{
			return View();
		}

		// GET: Normeringen/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: Normeringen/Aanmaken
		public ActionResult Aanmaken()
		{
			return View();
		}

		// POST: Normeringen/Aanmaken
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

		// GET: Normeringen/Bewerken/5
		public ActionResult Bewerken(int id)
		{
			return View();
		}

		// POST: Normeringen/Bewerken/5
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

		// GET: Normeringen/Verwijderen/5
		public ActionResult Verwijderen(int id)
		{
			return View();
		}

		// POST: Normeringen/Verwijderen/5
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
