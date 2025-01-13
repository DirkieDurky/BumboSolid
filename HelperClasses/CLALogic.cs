using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BumboSolid.HelperClasses;

public class CLALogic : ICLALogic
{
    public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
    {
        return true;
    }
}
