using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses;

public class CLAgeEndAfterAgeStartLogic : ICLALogic
{
    // Have to ensure start age of a range is not a higher number than end age is.
    public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
    {
        bool valid = false;
        if (!model.AgeStart.HasValue || !model.AgeEnd.HasValue) valid = true; // If either age is null, its not a higher start age than end age.
        if (model.AgeStart.HasValue && model.AgeEnd.HasValue)
        {
            if (model.AgeStart <= model.AgeEnd.Value) valid = true;
        }
        if (!valid) modelState.AddModelError(nameof(model.AgeEnd), "De eind leeftijd moet hoger zijn dan de begin leeftijd");
        return valid;
    }
}
