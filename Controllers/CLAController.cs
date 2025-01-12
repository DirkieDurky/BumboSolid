using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net.Cache;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Configuration;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Humanizer;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using BumboSolid.HelperClasses;

namespace BumboSolid.Controllers
{
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
                new CLAAgeWithinRangeLogic(),
                new CLAgeEndAfterAgeStartLogic(),
                new CLASevenWeekDaysLogic(),
                new CLATimeInDayLogic(),
                new CLATimeInWeekLogic(),
                new CLAViewModelNotEmptyLogic(),
                new CLANoBreakWithoutWorkLimitLogic(),
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
                .Include(e => e.CLABreakEntries)
                .OrderBy(e => e.AgeStart)
                .ToListAsync();

            var groupedCLACards = entries
                .GroupBy(e => new { e.AgeStart, e.AgeEnd })
                .Select(group => new CLACardViewModel
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
                        if (e.MaxWorkDurationPerHolidayWeek.HasValue)
                            rules.Add($"Mag maximaal {e.MaxWorkDurationPerHolidayWeek / 60.0} uur werken als het een vakantieweek is.");
                        if (e.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue)
                            rules.Add($"Gemiddelde uren per week verspreid over vier weken mag maximaal " +
                                $"{e.MaxAvgWeeklyWorkDurationOverFourWeeks / 60} uur zijn.");
                        if (e.EarliestWorkTime.HasValue)
                            rules.Add($"Mag op z'n vroegst om {e.EarliestWorkTime} beginnen met werken.");
                        if (e.LatestWorkTime.HasValue)
                            rules.Add($"Mag maximaal tot {e.LatestWorkTime} werken.");

                        if (e.CLABreakEntries.IsNullOrEmpty())
                        {
                            return rules;
                        }
                        if (e.CLABreakEntries[0].MinBreakDuration.HasValue)
                        {
                            rules.Add($"Wanneer iemand langer dan {e.CLABreakEntries[0].WorkDuration / 60.0} uur werkt, " +
                                $"is er recht op een pauze van {e.CLABreakEntries[0].MinBreakDuration} minuten.");
                        }
                        else
                        {
                            rules.Add($"Moet pauze krijgen na {e.CLABreakEntries[0].WorkDuration / 60.0} uur werken.");
                        }
                        return rules;
                    }).ToList()
                }).ToList();

            return View(groupedCLACards);
        }

        [Route("Toevoegen")]
        // GET: CLA/Create
        public IActionResult Create()
        {
            return View(new CLAManageViewModel());
        }

        // POST: CLA/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                var breakEntry = await _context.CLABreakEntries
                    .FirstOrDefaultAsync(e => e.CLAEntryId == existingEntry.Id);

                if (!_noOverwriteLogic.NoConflicts(existingEntry, claViewModel, ModelState, breakEntry)) return View(claViewModel);

                existingEntry = _claEntryConverter.ModelToEntry(claViewModel, existingEntry); // Can overwrite because there are no conflicts.

                if (breakEntry == null && claViewModel.BreakWorkDuration.HasValue)
                {
                    breakEntry = new CLABreakEntry();
                    breakEntry = _claEntryConverter.ModelToBreakEntry(claViewModel, existingEntry.Id, breakEntry);
                    _context.CLAEntries.Update(existingEntry);
                    _context.CLABreakEntries.Add(breakEntry);
                    _context.SaveChanges();

                    TempData["Message"] = "CAO regels zijn geupdated!";
                    return RedirectToAction(nameof(Index));
                }


                if (breakEntry != null && !breakEntry.MinBreakDuration.HasValue && claViewModel.BreakMinBreakDuration.HasValue)
                {
                    int minBreakTimeMulti = claViewModel.MaxUninterruptedShiftDurationHours ? 60 : 1;
                    breakEntry.MinBreakDuration = (int?)(claViewModel.BreakMinBreakDuration * minBreakTimeMulti);

                    _context.CLABreakEntries.Update(breakEntry);
                    _context.SaveChanges();
                }

                _context.CLAEntries.Update(existingEntry);
                _context.SaveChanges();

                TempData["Message"] = "CAO regels zijn geupdated!";
                return RedirectToAction(nameof(Index));
            }


            CLAEntry claEntry = new CLAEntry();
            claEntry = _claEntryConverter.ModelToEntry(claViewModel, claEntry);
            _context.Add(claEntry);
            await _context.SaveChangesAsync();

            if (claViewModel.BreakWorkDuration.HasValue)
            {
                CLABreakEntry breakEntry = new CLABreakEntry();
                _claEntryConverter.ModelToBreakEntry(claViewModel, claEntry.Id, breakEntry);
                _context.Add(breakEntry);
                await _context.SaveChangesAsync();
            }

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

            var breakEntry = await _context.CLABreakEntries
                .FirstOrDefaultAsync(e => e.CLAEntryId == claEntry.Id);

            CLAManageViewModel claViewModel = _claEntryConverter.EntryToModel(claEntry, breakEntry);

            return View(claViewModel);
        }

        // POST: CLA/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            var breakEntry = await _context.CLABreakEntries.
                FirstOrDefaultAsync(e => e.CLAEntryId == claViewModel.Id);

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


            int maxUninterruptedShiftMulti = claViewModel.MaxUninterruptedShiftDurationHours ? 60 : 1;
            int minBreakTimeMulti = claViewModel.MinBreakTimeHours ? 60 : 1;



            _claEntryConverter.ModelToEntry(claViewModel, claEntry);


            if (breakEntry != null && claViewModel.BreakWorkDuration.HasValue)
            {
                _context.CLABreakEntries.Remove(breakEntry); //apparently needed
                var updatedBreakEntry = new CLABreakEntry
                {
                    CLAEntryId = breakEntry.CLAEntryId,
                    WorkDuration = (int)(claViewModel.BreakWorkDuration.Value * maxUninterruptedShiftMulti),
                    MinBreakDuration = claViewModel.BreakMinBreakDuration.HasValue ?
                    (int)claViewModel.BreakMinBreakDuration * minBreakTimeMulti : null
                };

                _context.CLABreakEntries.Add(updatedBreakEntry);
            }

            if (breakEntry != null && !claViewModel.BreakWorkDuration.HasValue)
                _context.CLABreakEntries.Remove(breakEntry);

            if (breakEntry == null && claViewModel.BreakWorkDuration.HasValue)
            {
                breakEntry = new CLABreakEntry
                {
                    CLAEntryId = claEntry.Id,
                    WorkDuration = (int)(claViewModel.BreakWorkDuration.Value * maxUninterruptedShiftMulti),
                    MinBreakDuration = claViewModel.BreakMinBreakDuration.HasValue ?
                        (int)(claViewModel.BreakMinBreakDuration * minBreakTimeMulti) : null
                };
                _context.CLABreakEntries.Add(breakEntry);
            }

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cLAEntry = await _context.CLAEntries.FindAsync(id);
            if (cLAEntry == null)
            {
                TempData["Message"] = "CAO regel niet gevonden. Niks verwijderd.";
                return RedirectToAction(nameof(Index));
            }

            var breakEntry = await _context.CLABreakEntries.
                FirstOrDefaultAsync(e => e.CLAEntryId == id);
            if (breakEntry != null) _context.CLABreakEntries.Remove(breakEntry);
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
    }
}
