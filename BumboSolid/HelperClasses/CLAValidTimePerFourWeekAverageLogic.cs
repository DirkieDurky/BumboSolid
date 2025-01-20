using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public class CLAValidTimePerFourWeekAverageLogic : ICLALogic
    {
        public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
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
}
