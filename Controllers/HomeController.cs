using BumboSolid.Data;
using BumboSolid.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BumboSolid.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly BumboDbContext _context;

		public HomeController(ILogger<HomeController> logger, BumboDbContext context)
		{
			_logger = logger;
			_context = context;
		}

		public IActionResult Index()
		{
			var prognosisList = _context.Prognoses
				.OrderByDescending(p => p.Year)
				.ThenByDescending(p => p.Week)
				.ToList();

			return View(prognosisList);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
