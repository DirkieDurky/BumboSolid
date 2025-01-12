using BumboSolid.Data.Models;
using BumboSolid.Models;

namespace BumboSolid.HelperClasses
{
    public class CLAEntryConverter : ICLAEntryConverter
    {

        public void EnsureAgeRange(CLAEntry entry, CLAManageViewModel model)
        {
            if (entry == null) return;
            if (model == null) return;

            if (model.AgeStart.HasValue || model.AgeEnd.HasValue) return;

            var ageStart = entry.AgeStart;
            var ageEnd = entry.AgeEnd;
            model.AgeStart = ageStart;
            model.AgeEnd = ageEnd;
        }

        // Returns a CLAEntry with filled in values for what is filled in on the viewmodel
        public CLAEntry ModelToEntry(CLAManageViewModel model, CLAEntry entry)
        {
            int arraylength = 6;

            var ints = new int[arraylength];

            int maxAvgMulti = model.MaxAvgDurationHours ? 60 : 1;
            int maxTotalShiftMulti = model.MaxTotalShiftDurationHours ? 60 : 1;
            int maxWorkDayMulti = model.MaxDayDurationHours ? 60 : 1;
            int maxHolidayMulti = model.MaxHolidayDurationHours ? 60 : 1;
            int maxWeekMulti = model.MaxWeekDurationHours ? 60 : 1;
            
            entry.AgeStart = model.AgeStart.HasValue ? model.AgeStart.Value : entry.AgeStart;
            entry.AgeEnd = model.AgeEnd.HasValue ? model.AgeEnd.Value : entry.AgeEnd;
            entry.MaxAvgWeeklyWorkDurationOverFourWeeks = (int?)(model.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue ?
                (model.MaxAvgWeeklyWorkDurationOverFourWeeks * maxAvgMulti) : entry.MaxAvgWeeklyWorkDurationOverFourWeeks);
            entry.MaxShiftDuration = model.MaxShiftDuration.HasValue ?
                (int)(model.MaxShiftDuration.Value * maxTotalShiftMulti) : entry.MaxShiftDuration;
            entry.MaxWorkDaysPerWeek = model.MaxWorkDaysPerWeek.HasValue ? model.MaxWorkDaysPerWeek.Value : entry.MaxWorkDaysPerWeek;
            entry.MaxWorkDurationPerDay = model.MaxWorkDurationPerDay.HasValue ?
                (int)(model.MaxWorkDurationPerDay.Value * maxWorkDayMulti) : entry.MaxWorkDurationPerDay;
            entry.MaxWorkDurationPerHolidayWeek = model.MaxWorkDurationPerHolidayWeek.HasValue ?
                (int)(model.MaxWorkDurationPerHolidayWeek.Value * maxHolidayMulti) : entry.MaxWorkDurationPerHolidayWeek;
            entry.MaxWorkDurationPerWeek = model.MaxWorkDurationPerWeek.HasValue ?
                (int)(model.MaxWorkDurationPerWeek.Value * maxWeekMulti) : entry.MaxWorkDurationPerWeek;
            entry.LatestWorkTime = model.LatestWorkTime.HasValue ? model.LatestWorkTime.Value : entry.LatestWorkTime;
            entry.EarliestWorkTime = model.EarliestWorkTime.HasValue ? model.EarliestWorkTime.Value : entry.EarliestWorkTime;
            
            return entry;
        }

        // Same as ModelToEntry, but for optional break entries. Returns a CLABreakEntry ready to enter the database
        // Should only be entered when the viewmodel has a value for BreakWorkDuration, since breakentry is not allowed to exist without that.
        public CLABreakEntry ModelToBreakEntry(CLAManageViewModel model, int entryId, CLABreakEntry breakEntry)
        {
            int maxUninterruptedShiftMulti = model.MaxUninterruptedShiftDurationHours ? 60 : 1;
            int minBreakTimeMulti = model.MinBreakTimeHours ? 60 : 1;

            breakEntry.CLAEntryId = entryId;
            breakEntry.WorkDuration = (int)(model.BreakWorkDuration.Value * maxUninterruptedShiftMulti);
            breakEntry.MinBreakDuration = model.BreakMinBreakDuration.HasValue ?
                (int)(model.BreakMinBreakDuration.Value * minBreakTimeMulti) : null;

            return breakEntry;
        }

        public CLAManageViewModel EntryToModel(CLAEntry entry, CLABreakEntry? breakEntry)
        {
            decimal maxAvgMulti = 60;
            decimal maxTotalShiftMulti = 60;
            decimal maxWorkDayMulti = 60;
            decimal maxHolidayMulti = 60;
            decimal maxWeekMulti = 60;
            decimal maxUninterruptedMulti = 60;
            decimal minBreakTimeMulti = 1; // Since its more likely to be filled in in minutes

            int minTime = 120;

            if (entry.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue)
                maxAvgMulti = (entry.MaxAvgWeeklyWorkDurationOverFourWeeks >= minTime) ? 60 : 1;
            if (entry.MaxShiftDuration.HasValue)
                maxTotalShiftMulti = (entry.MaxShiftDuration.Value >= minTime) ? 60 : 1;
            if (entry.MaxWorkDurationPerDay.HasValue)
                maxWorkDayMulti = (entry.MaxWorkDurationPerDay >= minTime) ? 60 : 1;
            if (entry.MaxWorkDurationPerHolidayWeek.HasValue)
                maxHolidayMulti = (entry.MaxWorkDurationPerHolidayWeek >= minTime) ? 60 : 1;
            if (entry.MaxWorkDurationPerWeek.HasValue)
                maxWeekMulti = (entry.MaxWorkDurationPerWeek >= minTime) ? 60 : 1;

            if (breakEntry != null)
                maxUninterruptedMulti = breakEntry.WorkDuration >= minTime ? 60 : 1;

            if (breakEntry != null && breakEntry.MinBreakDuration.HasValue)
                minBreakTimeMulti = breakEntry.MinBreakDuration.Value >= minTime ? 60 : 1;


            CLAManageViewModel claViewModel = new()
            {
                Id = entry.Id,
                AgeStart = entry.AgeStart.HasValue ? entry.AgeStart : null,
                AgeEnd = entry.AgeEnd.HasValue ? entry.AgeEnd : null,
                MaxWorkDurationPerDay = entry.MaxWorkDurationPerDay.HasValue ?
                    entry.MaxWorkDurationPerDay / maxWorkDayMulti : null,
                MaxWorkDaysPerWeek = entry.MaxWorkDaysPerWeek.HasValue ?
                    entry.MaxWorkDaysPerWeek : null,
                MaxWorkDurationPerWeek = entry.MaxWorkDurationPerWeek.HasValue ?
                    entry.MaxWorkDurationPerWeek / maxWeekMulti : null,
                MaxWorkDurationPerHolidayWeek = entry.MaxWorkDurationPerHolidayWeek.HasValue ?
                    entry.MaxWorkDurationPerHolidayWeek / maxHolidayMulti : null,
                EarliestWorkTime = entry.EarliestWorkTime.HasValue ?
                    entry.EarliestWorkTime : null,
                LatestWorkTime = entry.LatestWorkTime.HasValue ?
                    entry.LatestWorkTime : null,
                MaxShiftDuration = entry.MaxShiftDuration.HasValue ?
                    entry.MaxShiftDuration / maxTotalShiftMulti : null,
                MaxAvgWeeklyWorkDurationOverFourWeeks = entry.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue ?
                    entry.MaxAvgWeeklyWorkDurationOverFourWeeks / maxAvgMulti : null,

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

            return claViewModel;
        }
    }
}
