using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public class CLAValidTimePerWeekLogic : ICLALogic
    {
        public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
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
    }
}
