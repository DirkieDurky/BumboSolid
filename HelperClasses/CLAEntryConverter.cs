using BumboSolid.Data.Models;
using BumboSolid.Models;

namespace BumboSolid.HelperClasses
{
    public class CLAEntryConverter : ICLAEntryConverter
    {
        public CLAManageViewModel EntryToModel(CLAEntry entry, CLABreakEntry? breakEntry)
        {
            decimal maxAvgMulti = 60;
            decimal maxTotalShiftMulti = 60;
            decimal maxWorkDayMulti = 60;
            decimal maxHolidayMulti = 60;
            decimal maxWeekMulti = 60;
            decimal maxUninterruptedMulti = 60;
            decimal minBreakTimeMulti = 60;

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
                    CalculateAndRound(entry.MaxWorkDurationPerDay, maxWorkDayMulti) : null,
                MaxWorkDaysPerWeek = entry.MaxWorkDaysPerWeek.HasValue ?
                    entry.MaxWorkDaysPerWeek : null,
                MaxWorkDurationPerWeek = entry.MaxWorkDurationPerWeek.HasValue ?
                    CalculateAndRound(entry.MaxWorkDurationPerWeek, maxWeekMulti) : null,
                MaxWorkDurationPerHolidayWeek = entry.MaxWorkDurationPerHolidayWeek.HasValue ?
                    CalculateAndRound(entry.MaxWorkDurationPerHolidayWeek, maxHolidayMulti) : null,
                EarliestWorkTime = entry.EarliestWorkTime.HasValue ?
                    entry.EarliestWorkTime : null,
                LatestWorkTime = entry.LatestWorkTime.HasValue ?
                    entry.LatestWorkTime : null,
                MaxShiftDuration = entry.MaxShiftDuration.HasValue ?
                    CalculateAndRound(entry.MaxShiftDuration, maxTotalShiftMulti) : null,
                MaxAvgWeeklyWorkDurationOverFourWeeks = entry.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue ?
                    CalculateAndRound(entry.MaxAvgWeeklyWorkDurationOverFourWeeks, maxAvgMulti) : null,

                BreakWorkDuration = breakEntry != null ? breakEntry.WorkDuration / maxUninterruptedMulti : null, //TODO
                BreakMinBreakDuration = breakEntry != null && breakEntry.MinBreakDuration.HasValue ?
                    breakEntry.MinBreakDuration / minBreakTimeMulti : null,

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

        public decimal CalculateAndRound(int? field, decimal factor)
        {
            if (field == null) return 0;
            decimal result;
            result = (int)field / factor;
            Math.Round(result, 2);
            return result;
        }

        // Returns a CLAEntry with filled in values for what is filled in on the viewmodel
        public CLAEntry ModelToEntry(CLAManageViewModel model)
        {
            int arraylength = 6;

            var ints = new int[arraylength];

            int maxAvgMulti = model.MaxAvgDurationHours ? 60 : 1;
            int maxTotalShiftMulti = model.MaxTotalShiftDurationHours ? 60 : 1;
            int maxWorkDayMulti = model.MaxDayDurationHours ? 60 : 1;
            int maxHolidayMulti = model.MaxHolidayDurationHours ? 60 : 1;
            int maxWeekMulti = model.MaxWeekDurationHours ? 60 : 1;

            CLAEntry entry = new CLAEntry
            {
                AgeStart = model.AgeStart.HasValue ? model.AgeStart.Value : null,
                AgeEnd = model.AgeEnd.HasValue ? model.AgeEnd.Value : null,
                MaxAvgWeeklyWorkDurationOverFourWeeks = (int?)(model.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue ?
                    (model.MaxAvgWeeklyWorkDurationOverFourWeeks * maxAvgMulti) : null),
                MaxShiftDuration = model.MaxShiftDuration.HasValue ?
                    (int)(model.MaxShiftDuration.Value * maxTotalShiftMulti) : null,
                MaxWorkDaysPerWeek = model.MaxWorkDaysPerWeek.HasValue ? model.MaxWorkDaysPerWeek.Value : null,
                MaxWorkDurationPerDay = model.MaxWorkDurationPerDay.HasValue ?
                    (int)(model.MaxWorkDurationPerDay.Value * maxWorkDayMulti) : null,
                MaxWorkDurationPerHolidayWeek = model.MaxWorkDurationPerHolidayWeek.HasValue ?
                    (int)(model.MaxWorkDurationPerHolidayWeek.Value * maxHolidayMulti) : null,
                MaxWorkDurationPerWeek = model.MaxWorkDurationPerWeek.HasValue ?
                    (int)(model.MaxWorkDurationPerWeek.Value * maxWeekMulti) : null,
                LatestWorkTime = model.LatestWorkTime.HasValue ? model.LatestWorkTime.Value : null,
                EarliestWorkTime = model.EarliestWorkTime.HasValue ? model.EarliestWorkTime.Value : null
            };
            return entry;
        }
    }
}
