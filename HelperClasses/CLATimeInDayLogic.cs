using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public class CLATimeInDayLogic : ICLALogic
    {
        public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
        {
            bool valid = true;
            if (model.MaxWorkDurationPerDay.HasValue)
                if ((model.MaxWorkDurationPerDay.Value > 1440 && !model.MaxDayDurationHours) ||
                    (model.MaxWorkDurationPerDay.Value > 24 && model.MaxDayDurationHours))
                {
                    modelState.AddModelError(nameof(model.MaxWorkDurationPerDay), "Er zit slechts 24 uur in een dag.");
                    valid = false;
                }
            return valid;
        }
    }
}
