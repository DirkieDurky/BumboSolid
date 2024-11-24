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

namespace BumboSolid.Controllers
{
    [Route("CAO")]
    public class CLAController : Controller
    {
        private readonly BumboDbContext _context;

        public CLAController(BumboDbContext context)
        {
            _context = context;
        }

        // GET: CLA
        [Route("CAO")]
        public async Task<IActionResult> Index()
        {
            var entries = await _context.CLAEntries
                .Include(e => e.CLABreakEntries)
                .OrderBy(e => e.AgeStart)
                .ToListAsync();

            var groupedCLACards = entries
                .Where(e => e.AgeStart.HasValue && e.AgeEnd.HasValue)
                .GroupBy(e => new { e.AgeStart, e.AgeEnd })
                .Select(group => new CLACardViewModel
                {
                    AgeStart = group.Key.AgeStart.Value,
                    AgeEnd = group.Key.AgeEnd.Value,
                    Rules = group.SelectMany(e =>
                    {
                        var rules = new List<string>();

                        if (e.MaxWorkDurationPerDay.HasValue)
                            rules.Add($"Mag maximaal {e.MaxWorkDurationPerDay/60.0} uur werken op een dag.");
                        if (e.MaxShiftDuration.HasValue)
                            rules.Add($"Eén shift mag maximaal {e.MaxShiftDuration/60.0} uur duren.");
                        if (e.MaxWorkDaysPerWeek.HasValue)
                            rules.Add($"Mag maximaal {e.MaxWorkDaysPerWeek} dagen in een week werken.");
                        if (e.MaxWorkDurationPerWeek.HasValue)
                            rules.Add($"Mag maximaal {e.MaxWorkDurationPerWeek / 60.0} uur in een week werken.");
                        if (e.MaxWorkDurationPerHolidayWeek.HasValue)
                            rules.Add($"Mag maximaal {e.MaxWorkDurationPerHolidayWeek/60.0} uur werken als het een vakantieweek is.");
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
                            rules.Add($"Wanneer iemand langer dan {e.CLABreakEntries[0].WorkDuration/60.0} uur werkt, " +
                                $"is er recht op een pauze van {e.CLABreakEntries[0].MinBreakDuration} minuten.");
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
            return View();
        }

        // POST: CLA/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AgeStart,AgeEnd,MaxWorkDurationPerDay,MaxWorkDaysPerWeek,MaxWorkDurationPerWeek,MaxWorkDurationPerHolidayWeek,EarliestWorkTime,LatestWorkTime,MaxAvgWeeklyWorkDurationOverFourWeeks,MaxShiftDuration")] CLAEntry claEntry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(claEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(claEntry);
        }

        // GET: CLA/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,AgeStart,AgeEnd,MaxWorkDurationPerDay,MaxWorkDaysPerWeek,MaxWorkDurationPerWeek,MaxWorkDurationPerHolidayWeek,EarliestWorkTime,LatestWorkTime,MaxAvgWeeklyWorkDurationOverFourWeeks,MaxShiftDuration")] CLAEntry cLAEntry)
        {
            if (id != cLAEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cLAEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CLAEntryExists(cLAEntry.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cLAEntry);
        }

        // GET: CLA/Delete/5
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
