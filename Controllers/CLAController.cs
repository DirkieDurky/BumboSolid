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

namespace BumboSolid.Controllers
{
    [Authorize(Roles = "Manager")]
    [Route("CAO")]
    public class CLAController : Controller
    {
        private readonly BumboDbContext _context;

        public CLAController(BumboDbContext context)
        {
            _context = context;
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
                            rules.Add($"Gemiddelde uren per week verspreid over vier weken mag maximaal {e.MaxAvgWeeklyWorkDurationOverFourWeeks / 60} uur zijn.");
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
                        } else
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
            if (!ModelState.IsValid) return View(claViewModel);

            //Since all fields are nullable (and ID isn't chosen by user),
            //we have to check whether anything has been filled in anywhere...
            var notChecked = new List<String>
            {
                nameof(claViewModel.AgeStart), nameof(claViewModel.AgeEnd), nameof(claViewModel.MaxAvgDurationHours),
                nameof(claViewModel.MaxDayDurationHours), nameof(claViewModel.MaxHolidayDurationHours),
                nameof(claViewModel.MaxWeekDurationHours), nameof(claViewModel.MaxTotalShiftDurationHours),
                nameof(claViewModel.MaxUninterruptedShiftDurationHours)
            }; //these on their own don't add any information

            bool hasValue = claViewModel.GetType()
                .GetProperties()
                .Where(p => !notChecked.Contains(p.Name))
                .Any(p => p.GetValue(claViewModel) != null);

            bool noErrors = true;
            if (!hasValue)
            {
                ModelState.AddModelError("", "Vergeet niet iets van een regel in te voeren.");
                noErrors = false;
            }

            if (claViewModel.AgeStart.HasValue && claViewModel.AgeEnd.HasValue && (claViewModel.AgeStart > claViewModel.AgeEnd))
            {
                ModelState.AddModelError("AgeEnd", "De eind leeftijd moet hoger zijn dan de begin leeftijd");
                noErrors = false;
            }

            if (claViewModel.MaxWorkDaysPerWeek.HasValue && claViewModel.MaxWorkDaysPerWeek.Value > 7)
            {
                ModelState.AddModelError("MaxWorkDaysPerWeek", "Er zijn slechts zeven dagen in een week.");
                noErrors = false;
            }

            if (!noErrors) return View(claViewModel);

            int maxAvgMulti = claViewModel.MaxAvgDurationHours ? 60 : 1;
            int maxTotalShiftMulti = claViewModel.MaxTotalShiftDurationHours ? 60 : 1;
            int maxWorkDayMulti = claViewModel.MaxDayDurationHours ? 60 : 1;
            int maxHolidayMulti = claViewModel.MaxHolidayDurationHours ? 60 : 1;
            int maxWeekMulti = claViewModel.MaxWeekDurationHours ? 60 : 1;
            int maxUninterruptedShiftMulti = claViewModel.MaxUninterruptedShiftDurationHours ? 60 : 1;

            var existingEntry = await _context.CLAEntries
                .FirstOrDefaultAsync(e =>
                    (e.AgeStart == claViewModel.AgeStart && e.AgeEnd == claViewModel.AgeEnd) ||
                    (e.AgeStart == null && claViewModel.AgeStart == null && e.AgeEnd == claViewModel.AgeEnd) ||
                    (e.AgeStart == claViewModel.AgeStart && e.AgeEnd == null && claViewModel.AgeEnd == null) ||
                    (e.AgeStart == null && e.AgeEnd == null && claViewModel.AgeStart == null && claViewModel.AgeEnd == null));

            if (existingEntry != null)
            {
                var conflictFields = new List<string>();

                //Checks individual fields for having entries in filled in agerange
                if (existingEntry.MaxWorkDurationPerDay.HasValue && claViewModel.MaxWorkDurationPerDay.HasValue)
                    conflictFields.Add(nameof(claViewModel.MaxWorkDurationPerDay));
                if (existingEntry.MaxWorkDaysPerWeek.HasValue && claViewModel.MaxWorkDaysPerWeek.HasValue)
                    conflictFields.Add(nameof(claViewModel.MaxWorkDaysPerWeek));
                if (existingEntry.MaxWorkDurationPerWeek.HasValue && claViewModel.MaxWorkDurationPerWeek.HasValue)
                    conflictFields.Add(nameof(claViewModel.MaxWorkDurationPerWeek));
                if (existingEntry.MaxWorkDurationPerHolidayWeek.HasValue && claViewModel.MaxWorkDurationPerHolidayWeek.HasValue)
                    conflictFields.Add(nameof(claViewModel.MaxWorkDurationPerHolidayWeek));
                if (existingEntry.EarliestWorkTime.HasValue && claViewModel.EarliestWorkTime.HasValue)
                    conflictFields.Add(nameof(claViewModel.EarliestWorkTime));
                if (existingEntry.LatestWorkTime.HasValue && claViewModel.LatestWorkTime.HasValue)
                    conflictFields.Add(nameof(claViewModel.LatestWorkTime));
                if (existingEntry.MaxShiftDuration.HasValue && claViewModel.MaxShiftDuration.HasValue)
                    conflictFields.Add(nameof(claViewModel.MaxShiftDuration));
                if (existingEntry.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue && claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue)
                    conflictFields.Add(nameof(claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks));

                if (conflictFields.Any())
                {
                    AddFieldErrors(conflictFields, claViewModel);
                    return View(claViewModel);
                }


                if (claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue)
                    existingEntry.MaxAvgWeeklyWorkDurationOverFourWeeks = claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks * maxAvgMulti;
                if (claViewModel.MaxShiftDuration.HasValue)
                    existingEntry.MaxShiftDuration = claViewModel.MaxShiftDuration * maxTotalShiftMulti;
                if (claViewModel.MaxWorkDurationPerDay.HasValue)
                    existingEntry.MaxWorkDurationPerDay = claViewModel.MaxWorkDurationPerDay * maxWorkDayMulti;
                if (claViewModel.MaxWorkDurationPerWeek.HasValue)
                    existingEntry.MaxWorkDurationPerWeek = claViewModel.MaxWorkDurationPerWeek * maxWeekMulti;
                if (claViewModel.MaxWorkDurationPerHolidayWeek.HasValue)
                    existingEntry.MaxWorkDurationPerHolidayWeek = claViewModel.MaxWorkDurationPerHolidayWeek * maxHolidayMulti;
                if (claViewModel.MaxWorkDaysPerWeek.HasValue)
                    existingEntry.MaxWorkDaysPerWeek = claViewModel.MaxWorkDaysPerWeek;
                if (claViewModel.LatestWorkTime.HasValue)
                    existingEntry.LatestWorkTime = claViewModel.LatestWorkTime;
                if (claViewModel.EarliestWorkTime.HasValue)
                    existingEntry.EarliestWorkTime = claViewModel.EarliestWorkTime;

                var breakEntry = await _context.CLABreakEntries
                    .FirstOrDefaultAsync(e => e.CLAEntryId == existingEntry.Id);

                if (breakEntry == null && claViewModel.BreakWorkDuration.HasValue)
                {
                    breakEntry = new CLABreakEntry();
                    breakEntry.CLAEntryId = existingEntry.Id;
                    breakEntry.WorkDuration = claViewModel.BreakWorkDuration.Value * maxUninterruptedShiftMulti;
                    breakEntry.MinBreakDuration = claViewModel.BreakMinBreakDuration.HasValue ?
                        claViewModel.BreakMinBreakDuration.Value : null;

                    _context.CLAEntries.Update(existingEntry);
                    _context.CLABreakEntries.Add(breakEntry);
                    _context.SaveChanges();

                    TempData["Message"] = "CAO regels zijn geupdated!";
                    return RedirectToAction(nameof(Index));
                }

                if (breakEntry != null && !breakEntry.MinBreakDuration.HasValue && claViewModel.BreakMinBreakDuration.HasValue)
                {
                    breakEntry.MinBreakDuration = claViewModel.BreakMinBreakDuration;

                    _context.CLABreakEntries.Update(breakEntry);
                    _context.SaveChanges();
                }

                _context.CLAEntries.Update(existingEntry);
                _context.SaveChanges();
            }
            else
            {
                CLAEntry claEntry = new CLAEntry
                {
                    AgeStart = claViewModel.AgeStart.HasValue ? claViewModel.AgeStart.Value : null,
                    AgeEnd = claViewModel.AgeEnd.HasValue ? claViewModel.AgeEnd.Value : null,
                    MaxAvgWeeklyWorkDurationOverFourWeeks = claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue ?
                    (claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks * maxAvgMulti) : null,
                    MaxShiftDuration = claViewModel.MaxShiftDuration.HasValue ?
                    (claViewModel.MaxShiftDuration.Value * maxTotalShiftMulti) : null,
                    MaxWorkDaysPerWeek = claViewModel.MaxWorkDaysPerWeek.HasValue ? claViewModel.MaxWorkDaysPerWeek.Value : null,
                    MaxWorkDurationPerDay = claViewModel.MaxWorkDurationPerDay.HasValue ?
                    (claViewModel.MaxWorkDurationPerDay.Value * maxWorkDayMulti) : null,
                    MaxWorkDurationPerHolidayWeek = claViewModel.MaxWorkDurationPerHolidayWeek.HasValue ?
                    (claViewModel.MaxWorkDurationPerHolidayWeek.Value * maxHolidayMulti) : null,
                    MaxWorkDurationPerWeek = claViewModel.MaxWorkDurationPerWeek.HasValue ?
                    (claViewModel.MaxWorkDurationPerWeek.Value * maxWeekMulti) : null,
                    LatestWorkTime = claViewModel.LatestWorkTime.HasValue ? claViewModel.LatestWorkTime.Value : null,
                    EarliestWorkTime = claViewModel.EarliestWorkTime.HasValue ? claViewModel.EarliestWorkTime.Value : null
                };
                _context.Add(claEntry);
                await _context.SaveChangesAsync();

                if (claViewModel.BreakWorkDuration.HasValue)
                {
                    CLABreakEntry breakEntry = new CLABreakEntry
                    {
                        CLAEntryId = claEntry.Id,
                        WorkDuration = claViewModel.BreakWorkDuration.Value * maxUninterruptedShiftMulti,
                        MinBreakDuration = claViewModel.BreakMinBreakDuration.HasValue ? claViewModel.BreakMinBreakDuration.Value : null
                    };
                    _context.Add(breakEntry);
                    await _context.SaveChangesAsync();
                }
            }


            TempData["Message"] = existingEntry != null ? "CAO regels zijn geupdated!" : "Nieuwe CAO regels succesvol toegevoegd!";

            return RedirectToAction(nameof(Index));
        }

        private void AddFieldErrors(List<string> conflictFields, CLAManageViewModel claViewModel)
        {
            foreach (var field in conflictFields)
            {
                switch (field)
                {
                    case nameof(claViewModel.MaxWorkDurationPerDay):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                    case nameof(claViewModel.MaxWorkDurationPerWeek):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                    case nameof(claViewModel.MaxWorkDaysPerWeek):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                    case nameof(claViewModel.MaxWorkDurationPerHolidayWeek):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                    case nameof(claViewModel.EarliestWorkTime):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                    case nameof(claViewModel.LatestWorkTime):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                    case nameof(claViewModel.MaxShiftDuration):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                    case nameof(claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                    case nameof(claViewModel.BreakWorkDuration):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                    case nameof(claViewModel.BreakMinBreakDuration):
                        ModelState.AddModelError(field, "Voor deze leeftijden is hier al een CAO regel ingevoerd");
                        break;
                }
            }
        }

        // GET: CLA/Edit/5
        [HttpGet("Bewerken/{id:int?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLAEntry = await _context.CLAEntries.FindAsync(id);
            if (cLAEntry == null)
            {
                return NotFound();
            }
            return View(cLAEntry);
        }

        // POST: CLA/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CLAManageViewModel claViewModel)
        {
            //throw new NotImplementedException();
            //if (id != claViewModel.Id)
            //{
            //    return NotFound();
            //}

            return RedirectToAction(nameof(Index));

        }

        // GET: CLA/Delete/5
        [HttpGet("Verwijderen/{id:int?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cLAEntry = await _context.CLAEntries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cLAEntry == null)
            {
                return NotFound();
            }

            return View(cLAEntry);
        }

        // POST: CLA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cLAEntry = await _context.CLAEntries.FindAsync(id);
            if (cLAEntry != null)
            {
                _context.CLAEntries.Remove(cLAEntry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CLAEntryExists(int id)
        {
            return _context.CLAEntries.Any(e => e.Id == id);
        }
    }
}
