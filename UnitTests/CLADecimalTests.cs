using BumboSolid.HelperClasses;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UnitTests
{
    public class CLADecimalTests
    {
        // Maxavgweeklyworkdurationoverfourweeks in int should return true
        [Fact]
        public void Set_MaxAvgWeeklyWorkDurationOverFourWeeksInt_True()
        {
            CLAManageViewModel model = new()
            {
                MaxAvgWeeklyWorkDurationOverFourWeeks = 1,
                MaxAvgDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLANoMinuteDecimalsLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // Maxavgweeklyworkdurationoverfourweeks in decimal should return false
        [Fact]
        public void Set_MaxAvgWeeklyWorkDurationOverFourWeeksDecimal_False()
        {
            CLAManageViewModel model = new()
            {
                MaxAvgWeeklyWorkDurationOverFourWeeks = 0.5m,
                MaxAvgDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLANoMinuteDecimalsLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // Maxshiftduration in int should return true
        [Fact]
        public void Set_MaxShiftDurationInt_True()
        {
            CLAManageViewModel model = new()
            {
                MaxShiftDuration = 4,
                MaxTotalShiftDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLANoMinuteDecimalsLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // Maxshiftduration in decimal should return false
        [Fact]
        public void Set_MaxShiftDurationDecimal_False()
        {
            CLAManageViewModel model = new()
            {
                MaxShiftDuration = 7.5m,
                MaxTotalShiftDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLANoMinuteDecimalsLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // Maxworkdurationperday in int should return true
        [Fact]
        public void Set_MaxWorkDurationPerDayInt_True()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerDay = 6,
                MaxDayDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLANoMinuteDecimalsLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // Maxworkdurationperday in decimal should return false
        [Fact]
        public void Set_MaxWorkDurationPerDayDecimal_False()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerDay = 3.5m,
                MaxDayDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLANoMinuteDecimalsLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // Maxworkdurationperweek in int should return true
        [Fact]
        public void Set_MaxWorkDurationPerWeekInt_True()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerWeek = 5,
                MaxWeekDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLANoMinuteDecimalsLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // Maxworkdurationperweek in decimal should return false
        [Fact]
        public void Set_MaxWorkDurationPerWeekDecimal_False()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerWeek = 1.5m,
                MaxWeekDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLANoMinuteDecimalsLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }
    }
}