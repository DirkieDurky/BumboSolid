using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;

namespace BumboSolid.HelperClasses
{
    public class CLANoMinuteDecimalsLogic : ICLALogic
    {
        public bool ValidateModel(CLAManageViewModel model, ModelStateDictionary modelState)
        {
            if (model == null) return false;

            string regex = @"^\d+$";

            if (model.MaxAvgWeeklyWorkDurationOverFourWeeks.HasValue && !model.MaxAvgDurationHours
                && !Regex.IsMatch(model.MaxAvgWeeklyWorkDurationOverFourWeeks.Value.ToString(), regex))
                modelState.AddModelError(nameof(model.MaxAvgWeeklyWorkDurationOverFourWeeks), "Houd minuten alstublieft in hele getallen");
            if (model.MaxShiftDuration.HasValue && !model.MaxTotalShiftDurationHours
                && !Regex.IsMatch(model.MaxShiftDuration.Value.ToString(), regex))
                modelState.AddModelError(nameof(model.MaxShiftDuration), "Houd minuten alstublieft in hele getallen");
            if (model.MaxWorkDurationPerDay.HasValue && !model.MaxDayDurationHours
                && !Regex.IsMatch(model.MaxWorkDurationPerDay.Value.ToString(), regex))
                modelState.AddModelError(nameof(model.MaxWorkDurationPerDay), "Houd minuten alstublieft in hele getallen");
            if (model.MaxWorkDurationPerHolidayWeek.HasValue && !model.MaxHolidayDurationHours
                && !Regex.IsMatch(model.MaxWorkDurationPerHolidayWeek.Value.ToString(), regex))
                modelState.AddModelError(nameof(model.MaxWorkDurationPerHolidayWeek), "Houd minuten alstublieft in hele getallen");
            if (model.MaxWorkDurationPerWeek.HasValue && !model.MaxWeekDurationHours
                && !Regex.IsMatch(model.MaxWorkDurationPerWeek.Value.ToString(), regex))
                modelState.AddModelError(nameof(model.MaxWorkDurationPerWeek), "Houd minuten alstublieft in hele getallen");
            if (model.BreakWorkDuration.HasValue && !model.MaxUninterruptedShiftDurationHours
                && !Regex.IsMatch(model.BreakWorkDuration.Value.ToString(), regex))
                modelState.AddModelError(nameof(model.BreakWorkDuration), "Houd minuten alstublieft in hele getallen");
            if (model.BreakMinBreakDuration.HasValue && !model.MinBreakTimeHours
                && !Regex.IsMatch(model.BreakMinBreakDuration.Value.ToString(), regex))
                modelState.AddModelError(nameof(model.BreakMinBreakDuration), "Houd minuten alstublieft in hele getallen");
            if (modelState.IsValid) return true;
            return false;
        }
    }
}
