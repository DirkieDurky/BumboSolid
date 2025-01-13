using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public class CLASevenWeekDaysLogic : ICLALogic
    {
        public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
        {
            bool valid = true;
            if (model.MaxWorkDaysPerWeek > 7)
            {
                modelState.AddModelError(nameof(model.MaxWorkDaysPerWeek), "Er zitten slechts zeven dagen in een week.");
                valid = false;
            }
            return valid;
        }
    }
}
