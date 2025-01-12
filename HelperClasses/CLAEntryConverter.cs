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

    }
}
