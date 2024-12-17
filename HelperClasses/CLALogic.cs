using BumboSolid.Data.Models;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public class CLALogic : ICLALogic
    {
        bool ICLALogic.ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
        {
            //Since all fields are nullable (and ID isn't chosen by user),
            //we have to check whether anything has been filled in anywhere...
            var valueExemptFields = new List<string>
            {
                nameof(model.AgeStart), nameof(model.AgeEnd), nameof(model.MaxAvgDurationHours),
                nameof(model.MaxDayDurationHours), nameof(model.MaxHolidayDurationHours),
                nameof(model.MaxWeekDurationHours), nameof(model.MaxTotalShiftDurationHours),
                nameof(model.MaxUninterruptedShiftDurationHours), nameof(model.Id)
            }; //these on their own don't add any information

            bool hasValue = model.GetType()
                .GetProperties()
                .Where(p => !valueExemptFields.Contains(p.Name))
                .Any(p => p.GetValue(model) != null);

            if (!hasValue)
            {
                modelState.AddModelError("", "Vergeet niet iets van een regel in te voeren.");
            }

            if (model.AgeStart.HasValue && model.AgeEnd.HasValue && (model.AgeStart > model.AgeEnd))
            {
                modelState.AddModelError("AgeEnd", "De eind leeftijd moet hoger zijn dan de begin leeftijd");
            }

            if (model.AgeStart.HasValue && model.AgeStart > 128)
            {
                modelState.AddModelError(nameof(model.AgeStart), "Houdt de leeftijd wel een beetje realistisch A.U.B.");
            }

            if (model.AgeEnd.HasValue && model.AgeEnd > 128)
            {
                modelState.AddModelError(nameof(model.AgeEnd), "Wanneer iemand zo oud is mogen ze al lang met pension, " +
                    "houdt de leeftijd A.U.B. realistisch");
            }

            if (model.MaxWorkDaysPerWeek.HasValue && model.MaxWorkDaysPerWeek.Value > 7)
            {
                modelState.AddModelError("MaxWorkDaysPerWeek", "Er zijn slechts zeven dagen in een week.");
            }

            if (model.MaxWorkDurationPerDay.HasValue)
                if ((model.MaxWorkDurationPerDay.Value > 1440 && !model.MaxDayDurationHours) ||
                    (model.MaxWorkDurationPerDay.Value > 24 && model.MaxDayDurationHours))
                {
                    modelState.AddModelError(nameof(model.MaxWorkDurationPerDay), "Er zit slechts 24 uur in een dag.");
                }

            if (model.MaxWorkDurationPerWeek.HasValue)
                if ((model.MaxWorkDurationPerWeek.Value > 10080 && !model.MaxWeekDurationHours) ||
                    (model.MaxWorkDurationPerWeek.Value > 168 && model.MaxWeekDurationHours))
                {
                    modelState.AddModelError(nameof(model.MaxWorkDurationPerWeek), "Er zit slechts 168 uur in een week");
                }

            if (model.MaxWorkDurationPerHolidayWeek.HasValue)
                if ((model.MaxWorkDurationPerHolidayWeek.Value > 10080 && !model.MaxHolidayDurationHours) ||
                    (model.MaxWorkDurationPerHolidayWeek.Value > 168 && model.MaxHolidayDurationHours))
                {
                    modelState.AddModelError(nameof(model.MaxWorkDurationPerHolidayWeek), "Er zit slechts 168 uur in een week");
                }

            if (model.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue)
                if ((model.MaxAvgWeeklyWorkDurationOverFourWeeks.Value > 10080 && !model.MaxAvgDurationHours) ||
                    (model.MaxAvgWeeklyWorkDurationOverFourWeeks.Value > 168 && model.MaxAvgDurationHours))
                {
                    modelState.AddModelError(nameof(model.MaxAvgWeeklyWorkDurationOverFourWeeks), "Er zit slechts 168 uur in een week");
                }

            if (!model.BreakWorkDuration.HasValue && model.BreakMinBreakDuration.HasValue)
            {
                modelState.AddModelError(nameof(model.BreakMinBreakDuration),
                    "Minimale pauzetijd mag niet worden ingevuld wanneer maximale werkduur zonder pauzes leeg is");
            }

            if (!modelState.IsValid) return false;
            return true;
        }

        CLAEntry ICLALogic.ViewModelToEntry(CLAManageViewModel model)
        {
            throw new NotImplementedException();
        }

        // Grab the modifier booleans for whether values need to be multiplied/divided to minutes or hours
        public int[] GetModifiers(CLAManageViewModel model)
        {
            int arraylength = 6;

            var ints = new int[arraylength];

            ints[0] = model.MaxAvgDurationHours ? 60 : 1;
            ints[1] = model.MaxTotalShiftDurationHours ? 60 : 1;
            ints[2] = model.MaxDayDurationHours ? 60 : 1;
            ints[3] = model.MaxHolidayDurationHours ? 60 : 1;
            ints[4] = model.MaxWeekDurationHours ? 60 : 1;
            ints[5] = model.MaxUninterruptedShiftDurationHours ? 60 : 1;

            return ints;
        }

        // Returns a list of which entries 
        public List<string> ExistingConflictFields(CLAManageViewModel claViewModel, CLAEntry existingEntry)
        {
            var conflictFields = new List<string>();

            // Checks individual fields for having entries in filled in agerange
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


        }

    }
}
