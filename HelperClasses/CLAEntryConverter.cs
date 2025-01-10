using BumboSolid.Data.Models;
using BumboSolid.Models;

namespace BumboSolid.HelperClasses
{
    public class CLAEntryConverter : ICLAEntryConverter
    {
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
