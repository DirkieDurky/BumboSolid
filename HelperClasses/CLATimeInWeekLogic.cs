using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses;

public class CLATimeInWeekLogic : ICLALogic
{
    // Three different rules use week time, decided to keep them in one class though.
    public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
    {
        return ValidPerWeek(model, modelState)
               && ValidPerHolidayWeek(model, modelState)
               && ValidPerFourWeekAverage(model, modelState);
    }

    private bool ValidPerWeek(CLAManageViewModel model, ModelStateDictionary modelState)
    {
        bool validPerWeek = true;
        if (model.MaxWorkDurationPerWeek.HasValue)
            if ((model.MaxWorkDurationPerWeek.Value > 10080 && !model.MaxWeekDurationHours) ||
                (model.MaxWorkDurationPerWeek.Value > 168 && model.MaxWeekDurationHours))
            {
                modelState.AddModelError(nameof(model.MaxWorkDurationPerWeek), "Er zit slechts 168 uur in een week");
                validPerWeek = false;
            }
        return validPerWeek;
    }

    private bool ValidPerHolidayWeek(CLAManageViewModel model, ModelStateDictionary modelState)
    {
        bool validPerHolidayWeek = true;
        if (model.MaxWorkDurationPerHolidayWeek.HasValue)
            if ((model.MaxWorkDurationPerHolidayWeek.Value > 10080 && !model.MaxHolidayDurationHours) ||
                (model.MaxWorkDurationPerHolidayWeek.Value > 168 && model.MaxHolidayDurationHours))
            {
                modelState.AddModelError(nameof(model.MaxWorkDurationPerHolidayWeek), "Er zit slechts 168 uur in een week");
                validPerHolidayWeek = false;
            }
        return validPerHolidayWeek;
    }

    private bool ValidPerFourWeekAverage(CLAManageViewModel model, ModelStateDictionary modelState)
    {
        bool validPerFourWeekAverage = true;
        if (model.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue)
            if ((model.MaxAvgWeeklyWorkDurationOverFourWeeks.Value > 10080 && !model.MaxAvgDurationHours) ||
                (model.MaxAvgWeeklyWorkDurationOverFourWeeks.Value > 168 && model.MaxAvgDurationHours))
            {
                modelState.AddModelError(nameof(model.MaxAvgWeeklyWorkDurationOverFourWeeks), "Er zit slechts 168 uur in een week");
                validPerFourWeekAverage = false;
            }
        return validPerFourWeekAverage;
    }
}
