using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses
{
    public class CLAAgeWithinRangeLogic :ICLALogic
    {
        public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
        {
            bool valid_age = true;
            if (model.AgeStart.HasValue &&  model.AgeStart.Value > 128)
            {
                modelState.AddModelError(nameof(model.AgeStart), "Houdt AUB de leeftijd onder 128 jaar.");
                valid_age = false;
            }
            if (model.AgeEnd.HasValue && model.AgeEnd.Value > 128)
            {
                modelState.AddModelError(nameof(model.AgeEnd), "Houdt AUB de leeftijd onder 128 jaar.");
                valid_age = false;
            }
            return valid_age;
        }
    }
}
