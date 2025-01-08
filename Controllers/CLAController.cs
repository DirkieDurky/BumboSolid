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

        public CLAController(BumboDbContext context)
        {
            _context = context;
            _logicRules = MakeLogic();
        }

        private List<ICLALogic> MakeLogic()
        {
            List<ICLALogic> validateRules = new List<ICLALogic>();
            validateRules.Add(new CLAAgeWithinRangeLogic());
            validateRules.Add(new CLAgeEndAfterAgeStartLogic());
            validateRules.Add(new CLASevenWeekDaysLogic());
            validateRules.Add(new CLATimeInDayLogic());
            validateRules.Add(new CLATimeInWeekLogic());
            validateRules.Add(new CLAViewModelNotEmptyLogic());
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
            foreach(ICLALogic rule in _logicRules)
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
                var conflictFields = GetConflictFields(existingEntry, claViewModel);

                if (conflictFields.Count > 0) return View(claViewModel);

                existingEntry = ModelToEntry(claViewModel); // Can overwrite because there are no conflicts.

                var breakEntry = await _context.CLABreakEntries
                    .FirstOrDefaultAsync(e => e.CLAEntryId == existingEntry.Id);

                if (breakEntry == null && claViewModel.BreakWorkDuration.HasValue)
                {
                    breakEntry = ModelToBreakEntry(claViewModel, existingEntry.Id);
                    _context.CLAEntries.Update(existingEntry);
                    _context.CLABreakEntries.Add(breakEntry);
                    _context.SaveChanges();

                    TempData["Message"] = "CAO regels zijn geupdated!";
                    return RedirectToAction(nameof(Index));
                }

                //TODO: unsure about how to handle this in neat way rn.
                if (breakEntry != null && !breakEntry.MinBreakDuration.HasValue && claViewModel.BreakMinBreakDuration.HasValue)
                {
                    int minBreakTimeMulti = claViewModel.MaxUninterruptedShiftDurationHours ? 60 : 1;
                    breakEntry.MinBreakDuration = (int?)(claViewModel.BreakMinBreakDuration * minBreakTimeMulti);

                    _context.CLABreakEntries.Update(breakEntry);
                    _context.SaveChanges();
                }

                _context.CLAEntries.Update(existingEntry);
                _context.SaveChanges();
            }
                
                CLAEntry claEntry = ModelToEntry(claViewModel);
                _context.Add(claEntry);
                await _context.SaveChangesAsync();

                if (claViewModel.BreakWorkDuration.HasValue)
                {
                    CLABreakEntry breakEntry = ModelToBreakEntry(claViewModel, claEntry.Id);
                    _context.Add(breakEntry);
                    await _context.SaveChangesAsync();
                }

            TempData["Message"] = existingEntry != null ? "CAO regels zijn geupdated!" : "Nieuwe CAO regels succesvol toegevoegd!";

            return RedirectToAction(nameof(Index));
        }

        // Avoid conflict fields
        private List<string> GetConflictFields(CLAEntry existingEntry, CLAManageViewModel model)
        {
            var conflictFields = new List<string>();

            //Checks individual fields for having entries in filled in agerange
            if (existingEntry.MaxWorkDurationPerDay.HasValue && model.MaxWorkDurationPerDay.HasValue)
                conflictFields.Add(nameof(model.MaxWorkDurationPerDay));
            if (existingEntry.MaxWorkDaysPerWeek.HasValue && model.MaxWorkDaysPerWeek.HasValue)
                conflictFields.Add(nameof(model.MaxWorkDaysPerWeek));
            if (existingEntry.MaxWorkDurationPerWeek.HasValue && model.MaxWorkDurationPerWeek.HasValue)
                conflictFields.Add(nameof(model.MaxWorkDurationPerWeek));
            if (existingEntry.MaxWorkDurationPerHolidayWeek.HasValue && model.MaxWorkDurationPerHolidayWeek.HasValue)
                conflictFields.Add(nameof(model.MaxWorkDurationPerHolidayWeek));
            if (existingEntry.EarliestWorkTime.HasValue && model.EarliestWorkTime.HasValue)
                conflictFields.Add(nameof(model.EarliestWorkTime));
            if (existingEntry.LatestWorkTime.HasValue && model.LatestWorkTime.HasValue)
                conflictFields.Add(nameof(model.LatestWorkTime));
            if (existingEntry.MaxShiftDuration.HasValue && model.MaxShiftDuration.HasValue)
                conflictFields.Add(nameof(model.MaxShiftDuration));
            if (existingEntry.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue && model.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue)
                conflictFields.Add(nameof(model.MaxAvgWeeklyWorkDurationOverFourWeeks));

            if (conflictFields.Count > 0)
            {
                AddFieldErrors(conflictFields, model);
            }
            return conflictFields;
        }

        // Returns a CLAEntry with filled in values for what is filled in on the viewmodel
        private CLAEntry ModelToEntry(CLAManageViewModel viewmodel)
        {
            int arraylength = 6;

            var ints = new int[arraylength];

            int maxAvgMulti = viewmodel.MaxAvgDurationHours ? 60 : 1;
            int maxTotalShiftMulti = viewmodel.MaxTotalShiftDurationHours ? 60 : 1;
            int maxWorkDayMulti = viewmodel.MaxDayDurationHours ? 60 : 1;
            int maxHolidayMulti = viewmodel.MaxHolidayDurationHours ? 60 : 1;
            int maxWeekMulti = viewmodel.MaxWeekDurationHours ? 60 : 1;

            CLAEntry entry = new CLAEntry
            {
                AgeStart = viewmodel.AgeStart.HasValue ? viewmodel.AgeStart.Value : null,
                AgeEnd = viewmodel.AgeEnd.HasValue ? viewmodel.AgeEnd.Value : null,
                MaxAvgWeeklyWorkDurationOverFourWeeks = (int?)(viewmodel.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue ?
                    (viewmodel.MaxAvgWeeklyWorkDurationOverFourWeeks * maxAvgMulti) : null),
                MaxShiftDuration = viewmodel.MaxShiftDuration.HasValue ?
                    (int)(viewmodel.MaxShiftDuration.Value * maxTotalShiftMulti) : null,
                MaxWorkDaysPerWeek = viewmodel.MaxWorkDaysPerWeek.HasValue ? viewmodel.MaxWorkDaysPerWeek.Value : null,
                MaxWorkDurationPerDay = viewmodel.MaxWorkDurationPerDay.HasValue ?
                    (int)(viewmodel.MaxWorkDurationPerDay.Value * maxWorkDayMulti) : null,
                MaxWorkDurationPerHolidayWeek = viewmodel.MaxWorkDurationPerHolidayWeek.HasValue ?
                    (int)(viewmodel.MaxWorkDurationPerHolidayWeek.Value * maxHolidayMulti) : null,
                MaxWorkDurationPerWeek = viewmodel.MaxWorkDurationPerWeek.HasValue ?
                    (int)(viewmodel.MaxWorkDurationPerWeek.Value * maxWeekMulti) : null,
                LatestWorkTime = viewmodel.LatestWorkTime.HasValue ? viewmodel.LatestWorkTime.Value : null,
                EarliestWorkTime = viewmodel.EarliestWorkTime.HasValue ? viewmodel.EarliestWorkTime.Value : null
            };
            return entry;
        }

        // Same as ModelToEntry, but for optional break entries. Returns a CLABreakEntry ready to enter the database
        // Should only be entered when the viewmodel has a value for BreakWorkDuration, since breakentry is not allowed to exist without that.
        private CLABreakEntry ModelToBreakEntry(CLAManageViewModel model, int entryId)
        {
            int maxUninterruptedShiftMulti = model.MaxUninterruptedShiftDurationHours ? 60 : 1;
            int minBreakTimeMulti = model.MaxUninterruptedShiftDurationHours ? 60 : 1;

            CLABreakEntry entry = new CLABreakEntry
            {
                CLAEntryId = entryId,
                WorkDuration = (int)(model.BreakWorkDuration.Value * maxUninterruptedShiftMulti),
                MinBreakDuration = model.BreakMinBreakDuration.HasValue ?
                            (int)(model.BreakMinBreakDuration.Value * minBreakTimeMulti) : null
            };
            return entry;
        }

        private void NoMinuteDecimals(CLAManageViewModel claViewModel)
        {
            if (claViewModel == null) return;

            string regex = @"^\d+$";

            if (claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue && !claViewModel.MaxAvgDurationHours
                && !Regex.IsMatch(claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks.Value.ToString(), regex))
                ModelState.AddModelError(nameof(claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks), "Houd minuten alstublieft in hele getallen");
            if (claViewModel.MaxShiftDuration.HasValue && !claViewModel.MaxTotalShiftDurationHours
                && !Regex.IsMatch(claViewModel.MaxShiftDuration.Value.ToString(), regex))
                ModelState.AddModelError(nameof(claViewModel.MaxShiftDuration), "Houd minuten alstublieft in hele getallen");
            if (claViewModel.MaxWorkDurationPerDay.HasValue && !claViewModel.MaxDayDurationHours
                && !Regex.IsMatch(claViewModel.MaxWorkDurationPerDay.Value.ToString(), regex))
                ModelState.AddModelError(nameof(claViewModel.MaxWorkDurationPerDay), "Houd minuten alstublieft in hele getallen");
            if (claViewModel.MaxWorkDurationPerHolidayWeek.HasValue && !claViewModel.MaxHolidayDurationHours
                && !Regex.IsMatch(claViewModel.MaxWorkDurationPerHolidayWeek.Value.ToString(), regex))
                ModelState.AddModelError(nameof(claViewModel.MaxWorkDurationPerHolidayWeek), "Houd minuten alstublieft in hele getallen");
            if (claViewModel.MaxWorkDurationPerWeek.HasValue && !claViewModel.MaxWeekDurationHours
                && !Regex.IsMatch(claViewModel.MaxWorkDurationPerWeek.Value.ToString(), regex))
                ModelState.AddModelError(nameof(claViewModel.MaxWorkDurationPerWeek), "Houd minuten alstublieft in hele getallen");
            if (claViewModel.BreakWorkDuration.HasValue && !claViewModel.MaxUninterruptedShiftDurationHours
                && !Regex.IsMatch(claViewModel.BreakWorkDuration.Value.ToString(), regex))
                ModelState.AddModelError(nameof(claViewModel.BreakWorkDuration), "Houd minuten alstublieft in hele getallen");
            if (claViewModel.BreakMinBreakDuration.HasValue && !claViewModel.MinBreakTimeHours
                && !Regex.IsMatch(claViewModel.BreakMinBreakDuration.Value.ToString(), regex))
                ModelState.AddModelError(nameof(claViewModel.BreakMinBreakDuration), "Houd minuten alstublieft in hele getallen");
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

            decimal maxAvgMulti = 60;
            decimal maxTotalShiftMulti = 60;
            decimal maxWorkDayMulti = 60;
            decimal maxHolidayMulti = 60;
            decimal maxWeekMulti = 60;
            decimal maxUninterruptedMulti = 60;
            decimal minBreakTimeMulti = 60;

            int minTime = 120;

            if (claEntry.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue)
                maxAvgMulti = (claEntry.MaxAvgWeeklyWorkDurationOverFourWeeks >= minTime) ? 60 : 1;
            if (claEntry.MaxShiftDuration.HasValue)
                maxTotalShiftMulti = (claEntry.MaxShiftDuration.Value >= minTime) ? 60 : 1;
            if (claEntry.MaxWorkDurationPerDay.HasValue)
                maxWorkDayMulti = (claEntry.MaxWorkDurationPerDay >= minTime) ? 60 : 1;
            if (claEntry.MaxWorkDurationPerHolidayWeek.HasValue)
                maxHolidayMulti = (claEntry.MaxWorkDurationPerHolidayWeek >= minTime) ? 60 : 1;
            if (claEntry.MaxWorkDurationPerWeek.HasValue)
                maxWeekMulti = (claEntry.MaxWorkDurationPerWeek >= minTime) ? 60 : 1;

            if (breakEntry != null)
                maxUninterruptedMulti = breakEntry.WorkDuration >= minTime ? 60 : 1;

            if (breakEntry != null && breakEntry.MinBreakDuration.HasValue)
                minBreakTimeMulti = breakEntry.MinBreakDuration.Value >= minTime ? 60 : 1;
            

            CLAManageViewModel claViewModel = new()
            {
                Id = claEntry.Id,
                AgeStart = claEntry.AgeStart.HasValue ? claEntry.AgeStart : null,
                AgeEnd = claEntry.AgeEnd.HasValue ? claEntry.AgeEnd : null,
                MaxWorkDurationPerDay = claEntry.MaxWorkDurationPerDay.HasValue ?
                    claEntry.MaxWorkDurationPerDay / maxWorkDayMulti : null,
                MaxWorkDaysPerWeek = claEntry.MaxWorkDaysPerWeek.HasValue ?
                    claEntry.MaxWorkDaysPerWeek : null,
                MaxWorkDurationPerWeek = claEntry.MaxWorkDurationPerWeek.HasValue ?
                    claEntry.MaxWorkDurationPerWeek / maxWeekMulti : null,
                MaxWorkDurationPerHolidayWeek = claEntry.MaxWorkDurationPerHolidayWeek.HasValue ?
                    claEntry.MaxWorkDurationPerHolidayWeek / maxHolidayMulti : null,
                EarliestWorkTime = claEntry.EarliestWorkTime.HasValue ?
                    claEntry.EarliestWorkTime : null,
                LatestWorkTime = claEntry.LatestWorkTime.HasValue ?
                    claEntry.LatestWorkTime : null,
                MaxShiftDuration = claEntry.MaxShiftDuration.HasValue ?
                    claEntry.MaxShiftDuration / maxTotalShiftMulti : null,
                MaxAvgWeeklyWorkDurationOverFourWeeks = claEntry.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue ?
                    claEntry.MaxAvgWeeklyWorkDurationOverFourWeeks / maxAvgMulti : null,

                BreakWorkDuration = breakEntry != null ? breakEntry.WorkDuration / maxUninterruptedMulti : null,
                BreakMinBreakDuration = breakEntry != null && breakEntry.MinBreakDuration.HasValue ?
                    breakEntry.MinBreakDuration : null,

                MaxAvgDurationHours = maxAvgMulti == 60,
                MaxTotalShiftDurationHours = maxTotalShiftMulti == 60,
                MaxDayDurationHours = maxWorkDayMulti == 60,
                MaxHolidayDurationHours = maxHolidayMulti == 60,
                MaxWeekDurationHours = maxWeekMulti == 60,
                MaxUninterruptedShiftDurationHours = maxUninterruptedMulti == 60,
                MinBreakTimeHours = minBreakTimeMulti == 60
            };


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

            //Since all fields are nullable (and ID isn't chosen by user),
            //we have to check whether anything has been filled in anywhere...
            var notChecked = new List<String>
            {
                nameof(claViewModel.AgeStart), nameof(claViewModel.AgeEnd), nameof(claViewModel.MaxAvgDurationHours),
                nameof(claViewModel.MaxDayDurationHours), nameof(claViewModel.MaxHolidayDurationHours),
                nameof(claViewModel.MaxWeekDurationHours), nameof(claViewModel.MaxTotalShiftDurationHours),
                nameof(claViewModel.MaxUninterruptedShiftDurationHours), nameof(claViewModel.Id)
            }; //these on their own don't add any information

            bool hasValue = claViewModel.GetType()
                .GetProperties()
                .Where(p => !notChecked.Contains(p.Name))
                .Any(p => p.GetValue(claViewModel) != null);

            if (!hasValue)
            {
                ModelState.AddModelError("", "U mag niet hier alle waardes leeggooien, gebruik daarvoor A.U.B. het verwijderen");
            }

           foreach(ICLALogic rule in _logicRules)
            {
                rule.ValidateModel(claViewModel, ModelState);
            }

            NoMinuteDecimals(claViewModel);

            if (!ModelState.IsValid) return View(claViewModel);

            int maxAvgMulti = claViewModel.MaxAvgDurationHours ? 60 : 1;
            int maxTotalShiftMulti = claViewModel.MaxTotalShiftDurationHours ? 60 : 1;
            int maxWorkDayMulti = claViewModel.MaxDayDurationHours ? 60 : 1;
            int maxHolidayMulti = claViewModel.MaxHolidayDurationHours ? 60 : 1;
            int maxWeekMulti = claViewModel.MaxWeekDurationHours ? 60 : 1;
            int maxUninterruptedShiftMulti = claViewModel.MaxUninterruptedShiftDurationHours ? 60 : 1;
            int minBreakTimeMulti = claViewModel.MinBreakTimeHours ? 60 : 1;


            claEntry.MaxAvgWeeklyWorkDurationOverFourWeeks = claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue ?
                (int?)(claViewModel.MaxAvgWeeklyWorkDurationOverFourWeeks * maxAvgMulti) : null;
            claEntry.MaxShiftDuration = claViewModel.MaxShiftDuration.HasValue ?
                (int?)(claViewModel.MaxShiftDuration.Value * maxTotalShiftMulti) : null;
            claEntry.MaxWorkDaysPerWeek = claViewModel.MaxWorkDaysPerWeek.HasValue ?
                claViewModel.MaxWorkDaysPerWeek.Value : null;
            claEntry.MaxWorkDurationPerDay = claViewModel.MaxWorkDurationPerDay.HasValue ?
                (int?)(claViewModel.MaxWorkDurationPerDay.Value * maxWorkDayMulti) : null;
            claEntry.MaxWorkDurationPerHolidayWeek = claViewModel.MaxWorkDurationPerHolidayWeek.HasValue ?
                (int?)(claViewModel.MaxWorkDurationPerHolidayWeek.Value * maxHolidayMulti) : null;
            claEntry.MaxWorkDurationPerWeek = claViewModel.MaxWorkDurationPerWeek.HasValue ?
                (int?)(claViewModel.MaxWorkDurationPerWeek.Value * maxWeekMulti) : null;
            claEntry.LatestWorkTime = claViewModel.LatestWorkTime.HasValue ?
                claViewModel.LatestWorkTime.Value : null;
            claEntry.EarliestWorkTime = claViewModel.EarliestWorkTime.HasValue ?
                claViewModel.EarliestWorkTime.Value : null;


            if (breakEntry != null && claViewModel.BreakWorkDuration.HasValue)
            {
                _context.CLABreakEntries.Remove(breakEntry); //apparently needed
                var updatedBreakEntry = new CLABreakEntry
                {
                    CLAEntryId = breakEntry.CLAEntryId,
                    WorkDuration = (int)(claViewModel.BreakWorkDuration.Value * maxUninterruptedShiftMulti),
                    MinBreakDuration = claViewModel.BreakMinBreakDuration.HasValue ?
                    (int)claViewModel.BreakMinBreakDuration : null
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
