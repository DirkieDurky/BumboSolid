using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public class CLAValidTimePerHolidayWeekLogic : ICLALogic
    {
        public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
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
    }
}
