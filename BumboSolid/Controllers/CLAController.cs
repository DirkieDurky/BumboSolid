using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using BumboSolid.HelperClasses;
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json.Linq;

namespace BumboSolid.Controllers;

[Authorize(Roles = "Manager")]
[Route("CAO")]
public class CLAController : Controller
{
	private readonly BumboDbContext _context;
	private List<ICLALogic> _logicRules;
	private ICLANoConflictFields _noOverwriteLogic;
	private ICLAEntryConverter _claEntryConverter;

	public CLAController(BumboDbContext context)
	{
		_context = context;
		_logicRules = MakeLogic();
		_noOverwriteLogic = new CLANoConflictFields();

		// Converters
		_claEntryConverter = new CLAEntryConverter();
	}

	// Put in the different validation rules to be active.
	private List<ICLALogic> MakeLogic()
	{
		List<ICLALogic> validateRules =
		[
			new CLAgeEndAfterAgeStartLogic(),
			new CLASevenWeekDaysLogic(),
			new CLATimeInDayLogic(),
			new CLAValidTimePerWeekLogic(),
			new CLAValidTimePerFourWeekAverageLogic(),
			new CLAViewModelNotEmptyLogic(),
			new CLANoMinuteDecimalsLogic(),
		];
		return validateRules;
	}

	public IActionResult EditDone()
	{
		return RedirectToAction(nameof(Index));
	}

	// GET: CLA
	[HttpGet("")]
	public async Task<IActionResult> Index()
	{
		var entries = await _context.CLAEntries
			.OrderBy(e => e.AgeStart)
			.ToListAsync();

		List<CLASurchargeEntry> surchargeEntries = await _context.CLASurchargeEntries
			.ToListAsync();

		int lastId = 0;

		List<CLACardViewModel> groupedCLACards = entries
			.GroupBy(e => new { e.AgeStart, e.AgeEnd })
			.Select(group =>
			{
				lastId = group.First().Id;
				return new CLACardViewModel
				{
					Id = group.First().Id,
					AgeStart = group.Key.AgeStart.HasValue ? group.Key.AgeStart.Value : null,
					AgeEnd = group.Key.AgeEnd.HasValue ? group.Key.AgeEnd.Value : null,
					Rules = group.SelectMany(e =>
					{
						var rules = new List<string>();

						if (e.MaxWorkDurationPerDay.HasValue)
							rules.Add($"Mag maximaal {e.MaxWorkDurationPerDay / 60.0} uur werken op een dag.");
						if (e.MaxShiftDuration.HasValue)
							rules.Add($"Eén shift mag maximaal {e.MaxShiftDuration / 60.0} uur duren.");
						if (e.MaxWorkDaysPerWeek.HasValue)
							rules.Add($"Mag maximaal {e.MaxWorkDaysPerWeek} dagen in een week werken.");
						if (e.MaxWorkDurationPerWeek.HasValue)
							rules.Add($"Mag maximaal {e.MaxWorkDurationPerWeek / 60.0} uur in een week werken.");
						if (e.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue)
							rules.Add($"Gemiddelde uren per week verspreid over vier weken mag maximaal " +
								$"{e.MaxAvgWeeklyWorkDurationOverFourWeeks / 60} uur zijn.");
						if (e.EarliestWorkTime.HasValue)
							rules.Add($"Mag op z'n vroegst om {e.EarliestWorkTime} beginnen met werken.");
						if (e.LatestWorkTime.HasValue)
							rules.Add($"Mag maximaal tot {e.LatestWorkTime} werken.");

						return rules;
					}).ToList()
				};
			}).ToList();

		groupedCLACards.AddRange(surchargeEntries
			.Select(entry =>
			{
				List<string> rules = [];

				string[] daysOfTheWeek = { "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrijdag", "Zaterdag", "Zondag" };

				if (entry.Weekday != null)
					rules.Add($"Toeslag geld alleen op {daysOfTheWeek[(int)entry.Weekday]}");
				if (entry.StartTime != null)
					rules.Add($"Toeslag geld vanaf {entry.StartTime}");
				if (entry.EndTime != null)
					rules.Add($"Toeslag geld tot {entry.EndTime}");

				return new CLACardViewModel
				{
					Id = entry.Id + lastId,
					Surcharge = entry.Surcharge,
					Rules = rules
				};
			}).ToList());

		return View(groupedCLACards);
	}

	// GET: CLA/Create
	[Route("Toevoegen")]
	public IActionResult Create()
	{
		return View(new CLAManageViewModel());
	}

	// POST: CLA/Create
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Route("Toevoegen")]
	public async Task<IActionResult> Create(CLAManageViewModel claViewModel)
	{
		foreach (ICLALogic rule in _logicRules)
		{
			rule.ValidateModel(claViewModel, ModelState);
		}
		if (!ModelState.IsValid) return View(claViewModel);

		var existingEntry = await _context.CLAEntries
			.FirstOrDefaultAsync(e =>
				(e.AgeStart == claViewModel.AgeStart && e.AgeEnd == claViewModel.AgeEnd) ||
				(e.AgeStart == null && claViewModel.AgeStart == null && e.AgeEnd == claViewModel.AgeEnd) ||
				(e.AgeStart == claViewModel.AgeStart && e.AgeEnd == null && claViewModel.AgeEnd == null) ||
				(e.AgeStart == null && e.AgeEnd == null && claViewModel.AgeStart == null && claViewModel.AgeEnd == null));

		if (existingEntry != null)
		{
			if (!_noOverwriteLogic.NoConflicts(existingEntry, claViewModel, ModelState)) return View(claViewModel);

			existingEntry = _claEntryConverter.ModelToEntry(claViewModel, existingEntry); // Can overwrite because there are no conflicts.

			_context.CLAEntries.Update(existingEntry);
			_context.SaveChanges();

			TempData["Message"] = "CAO regels zijn geupdated!";
			return RedirectToAction(nameof(Index));
		}

		CLAEntry claEntry = new CLAEntry();
		claEntry = _claEntryConverter.ModelToEntry(claViewModel, claEntry);
		_context.Add(claEntry);
		await _context.SaveChangesAsync();

		TempData["Message"] = "Nieuwe CAO regels succesvol toegevoegd!";

		return RedirectToAction(nameof(Index));
	}

	// GET: CLA/Edit/5
	[HttpGet(template: "Bewerken")]
	public async Task<IActionResult> Edit(int? ageStart, int? ageEnd)
	{
		var claEntry = await _context.CLAEntries
			.FirstOrDefaultAsync(e =>
				(e.AgeStart == ageStart && e.AgeEnd == ageEnd) ||
				(e.AgeStart == null && ageStart == null && e.AgeEnd == ageEnd) ||
				(e.AgeStart == ageStart && e.AgeEnd == null && ageEnd == null) ||
				(e.AgeStart == null && e.AgeEnd == null && ageStart == null && ageEnd == null));
		if (claEntry == null)
		{
			TempData["Message"] = "CAO regel niet gevonden";
			return RedirectToAction(nameof(Index));
		}

		CLAManageViewModel claViewModel = _claEntryConverter.EntryToModel(claEntry);

		return View(claViewModel);
	}

	// POST: CLA/Edit/5
	[HttpPost]
	[ValidateAntiForgeryToken]
	[Route("Bewerken")]
	public async Task<IActionResult> Edit(CLAManageViewModel claViewModel)
	{
		if (!claViewModel.Id.HasValue)
		{
			TempData["Message"] = "Id niet gevonden, aanpassen geannuleerd.";
			return RedirectToAction(nameof(Index));
		}

		var claEntry = await _context.CLAEntries.
			FirstOrDefaultAsync(e => e.Id == claViewModel.Id);

		if (claEntry == null)
		{
			TempData["Message"] = "CAO regel niet gevonden, aanpassen geannuleerd.";
			return RedirectToAction(nameof(Index));
		}
		_claEntryConverter.EnsureAgeRange(claEntry, claViewModel);

		foreach (ICLALogic rule in _logicRules)
		{
			rule.ValidateModel(claViewModel, ModelState);
		}

		if (!ModelState.IsValid) return View(claViewModel);

		_claEntryConverter.ModelToEntry(claViewModel, claEntry);

		_context.CLAEntries.Update(claEntry);
		_context.SaveChanges();

		TempData["Message"] = "Succesvol deze regel geupdated";
		return RedirectToAction(nameof(Index));
	}

	// GET: CLA/Delete/5
	[HttpGet("Verwijderen")]
	public async Task<IActionResult> Delete(int? ageStart, int? ageEnd)
	{
		var claEntry = await _context.CLAEntries
			.FirstOrDefaultAsync(e =>
				(e.AgeStart == ageStart && e.AgeEnd == ageEnd) ||
				(e.AgeStart == null && ageStart == null && e.AgeEnd == ageEnd) ||
				(e.AgeStart == ageStart && e.AgeEnd == null && ageEnd == null) ||
				(e.AgeStart == null && e.AgeEnd == null && ageStart == null && ageEnd == null));
		if (claEntry == null)
		{
			TempData["Message"] = "CAO regel niet gevonden";
			return RedirectToAction(nameof(Index));
		}

		return View(claEntry);
	}

	// POST: CLA/Delete/5
	[HttpPost, ActionName("VerwijderdSucces")]
	[Route("Verwijderen")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var cLAEntry = await _context.CLAEntries.FindAsync(id);
		if (cLAEntry == null)
		{
			TempData["Message"] = "CAO regel niet gevonden. Niks verwijderd.";
			return RedirectToAction(nameof(Index));
		}

		_context.CLAEntries.Remove(cLAEntry);

		await _context.SaveChangesAsync();

		string message = "";

		if (cLAEntry.AgeStart.HasValue && cLAEntry.AgeEnd.HasValue)
		{
			message = $"Regels voor {cLAEntry.AgeStart} tot {cLAEntry.AgeEnd} verwijderd";
		}
		else if (!cLAEntry.AgeStart.HasValue && !cLAEntry.AgeEnd.HasValue)
		{
			message = $"Algemene regels verwijderd";
		}
		else if (!cLAEntry.AgeStart.HasValue && cLAEntry.AgeEnd.HasValue)
		{
			message = $"Regels tot {cLAEntry.AgeEnd} verwijderd";
		}
		else if (cLAEntry.AgeStart.HasValue && !cLAEntry.AgeEnd.HasValue)
		{
			message = $"Regels vanaf {cLAEntry.AgeStart} jaar verwijderd";
		}

		TempData["Message"] = message;
		return RedirectToAction(nameof(Index));
	}

	[Route("ToeslagToevoegen")]
	public IActionResult CreateSurchargeEntry()
	{
		return View(new CLASurchargeEntry());
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Route("ToeslagToevoegen")]
	public async Task<IActionResult> CreateSurchargeEntry([Bind("Id,Surcharge,Weekday,StartTime,EndTime")] CLASurchargeEntry claSurchargeEntry)
	{
		if (ModelState.IsValid)
		{
			_context.Add(claSurchargeEntry);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		return View(claSurchargeEntry);
	}

	[HttpGet("ToeslagVerwijderen")]
	public async Task<IActionResult> DeleteSurchargeEntry(int? id)
	{
		CLASurchargeEntry? claSurchargeEntry = await _context.CLASurchargeEntries
			.FirstOrDefaultAsync(e => e.Id == id);

		if (claSurchargeEntry == null)
		{
			TempData["Message"] = "CAO Toeslag Regel niet gevonden";
			return RedirectToAction(nameof(Index));
		}

		return View(claSurchargeEntry);
	}

	[HttpPost, ActionName("ToeslagVerwijderdSucces")]
	[Route("ToeslagVerwijderen")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteSurchargeEntryConfirmed(int id)
	{
		var claSurchargeEntry = await _context.CLASurchargeEntries.FindAsync(id);
		if (claSurchargeEntry == null)
		{
			TempData["Message"] = "CAO Toeslag Regel niet gevonden. Niks verwijderd.";
			return RedirectToAction(nameof(Index));
		}

		_context.CLASurchargeEntries.Remove(claSurchargeEntry);

		await _context.SaveChangesAsync();

		string message = "Successvol verwijderd";

		TempData["Message"] = message;
		return RedirectToAction(nameof(Index));
	}

	[HttpGet(template: "ToeslagBewerken")]
	public async Task<IActionResult> EditSurchargeEntry(int? id)
	{
		CLASurchargeEntry? claSurchargeEntry = await _context.CLASurchargeEntries
			.FirstOrDefaultAsync(entry => entry.Id == id);
		if (claSurchargeEntry == null)
		{
			TempData["Message"] = "CAO regel niet gevonden";
			return RedirectToAction(nameof(Index));
		}

		return View(claSurchargeEntry);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Route("ToeslagBewerken")]
	public async Task<IActionResult> EditSurchargeEntry(CLASurchargeEntry claSurchargeEntry)
	{
		if (claSurchargeEntry.Id == 0)
		{
			TempData["Message"] = "Id niet gevonden, aanpassen geannuleerd.";
			return RedirectToAction(nameof(Index));
		}

		CLASurchargeEntry? claSurchargeEntryDB = await _context.CLASurchargeEntries.
			FirstOrDefaultAsync(e => e.Id == claSurchargeEntry.Id);

		if (claSurchargeEntryDB == null)
		{
			TempData["Message"] = "CAO regel niet gevonden, aanpassen geannuleerd.";
			return RedirectToAction(nameof(Index));
		}

		if (!ModelState.IsValid) return View(claSurchargeEntry);

		claSurchargeEntryDB.Surcharge = claSurchargeEntry.Surcharge;
		claSurchargeEntryDB.Weekday = claSurchargeEntry.Weekday;
		claSurchargeEntryDB.StartTime = claSurchargeEntry.StartTime;
		claSurchargeEntryDB.EndTime = claSurchargeEntry.EndTime;

		_context.CLASurchargeEntries.Update(claSurchargeEntryDB);
		_context.SaveChanges();

		TempData["Message"] = "Succesvol deze toeslag regel geupdated";
		return RedirectToAction(nameof(Index));
	}
}
