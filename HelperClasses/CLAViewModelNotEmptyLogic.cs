using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses;

public class CLAViewModelNotEmptyLogic : ICLALogic
{
    // Since all fields are nullable (and ID isn't chosen by user),
    // we have to check whether anything has been filled in anywhere...
    public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
    {
        // these on their own don't add any information
        var valueExemptFields = new List<string>
        {
            nameof(model.AgeStart), nameof(model.AgeEnd), nameof(model.MaxAvgDurationHours),
            nameof(model.MaxDayDurationHours), nameof(model.MaxHolidayDurationHours),
            nameof(model.MaxWeekDurationHours), nameof(model.MaxTotalShiftDurationHours),
            nameof(model.MaxUninterruptedShiftDurationHours), nameof(model.MinBreakTimeHours), nameof(model.Id)
        };

        bool hasValue = model.GetType()
            .GetProperties()
            .Where(p => !valueExemptFields.Contains(p.Name))
            .Any(p => p.GetValue(model) != null);

        if (!hasValue)
        {
            modelState.AddModelError("", "U mag geen CAO regels toevoegen met niks van restricties ingevuld.");
        }

        return hasValue;
    }

}
