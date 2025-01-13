using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses;

public class CLANoBreakWithoutWorkLimitLogic : ICLALogic
{
    public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
    {
        if (!model.BreakWorkDuration.HasValue && model.BreakMinBreakDuration.HasValue)
        {
            modelState.AddModelError(nameof(model.BreakWorkDuration), "U mag geen minimale pauzetijd invullen zonder een maximale ononderbroken werktijd.");
            return false;
        }
        return true;
    }
}
