using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public class CLANoConflictFields : ICLANoConflictFields
    {
        public bool NoConflicts(CLAEntry existingEntry, CLAManageViewModel model, ModelStateDictionary modelState, CLABreakEntry? breakEntry)
        {
            if (existingEntry == null) return true;
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

            if(breakEntry != null)
            {
                if (model.BreakWorkDuration.HasValue) // breakEntry.WorkDuration having a value is implied.
                    conflictFields.Add(nameof(model.BreakWorkDuration));
                if (breakEntry.MinBreakDuration.HasValue && model.BreakMinBreakDuration.HasValue)
                    conflictFields.Add(nameof(breakEntry.MinBreakDuration));
            }

            if (conflictFields.Count > 0)
            {
                AddFieldErrors(conflictFields, model, modelState);
                return false;
            }

            return true;
        }

        private void AddFieldErrors(List<string> conflictFields, CLAManageViewModel model, ModelStateDictionary modelState)
        {
            string errorMessage = "Voor deze leeftijden is hier al een CAO regel ingevoerd. Ga voor aanpassingen naar bewerken AUB.";
            foreach (var field in conflictFields)
            {
                switch (field)
                {
                    case nameof(model.MaxWorkDurationPerDay):
                        modelState.AddModelError(field, errorMessage);
                        break;
                    case nameof(model.MaxWorkDurationPerWeek):
                        modelState.AddModelError(field, errorMessage);
                        break;
                    case nameof(model.MaxWorkDaysPerWeek):
                        modelState.AddModelError(field, errorMessage);
                        break;
                    case nameof(model.MaxWorkDurationPerHolidayWeek):
                        modelState.AddModelError(field, errorMessage);
                        break;
                    case nameof(model.EarliestWorkTime):
                        modelState.AddModelError(field, errorMessage);
                        break;
                    case nameof(model.LatestWorkTime):
                        modelState.AddModelError(field, errorMessage);
                        break;
                    case nameof(model.MaxShiftDuration):
                        modelState.AddModelError(field, errorMessage);
                        break;
                    case nameof(model.MaxAvgWeeklyWorkDurationOverFourWeeks):
                        modelState.AddModelError(field, errorMessage);
                        break;
                    case nameof(model.BreakWorkDuration):
                        modelState.AddModelError(field, errorMessage);
                        break;
                    case nameof(model.BreakMinBreakDuration):
                        modelState.AddModelError(field, errorMessage);
                        break;
                }
            }
        }
    }
}
