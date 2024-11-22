using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Controllers
{
	public class AvailabilityController : Controller
	{
		private readonly BumboDbContext _context;

		public AvailabilityController(BumboDbContext context)
		{
			_context = context;
		}

		// GET: AvailiabilityController/Index
		public async Task<IActionResult> Index()
		{
            List<AvailabilityRuleViewModel> availabilityViewModels = new List<AvailabilityRuleViewModel>();

            foreach (AvailabilityRule availabilityRule in _context.AvailabilityRules.ToList()) // Hier moet alleen de availabilityrules van de jusite medewerker en de geselceteerde week worden meegegeven
            {
				String Day = availabilityRule.Date.DayOfWeek.ToString();

                AvailabilityRuleViewModel availabilityViewModel = new AvailabilityRuleViewModel() { 
					Day = Day, 
					StartTime = availabilityRule.StartTime, 
					EndTime = availabilityRule.EndTime
				};

                availabilityViewModels.Add(availabilityViewModel);
            }

            return View(availabilityViewModels);
        }

        // GET: AvailiabilityController/Create
        public async Task<IActionResult> Create()
        {
            return View(new AvailabilityRuleViewModel());
        }

		// Post: AvailiabilityController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AvailabilityRuleViewModel availabilityRuleViewModel, int weekNr)
		{
			AvailabilityRule availabilityRule = new AvailabilityRule();
			
			// Convert String to DayOfWeek
			switch (availabilityRuleViewModel.Day)
			{
				case "Monday":
					availabilityRule.Date = ;
					break;
			}

			availabilityRule.StartTime = availabilityRuleViewModel.StartTime;
			availabilityRule.EndTime = availabilityRuleViewModel.EndTime;


			return View(availabilityRuleViewModel);
		}
	}
}
