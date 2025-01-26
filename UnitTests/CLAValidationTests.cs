using BumboSolid.HelperClasses;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UnitTests
{
	public class CLAValidationTests
    {
        // Start age before end age should return true
        [Fact]
        public void Compare_StartAgeBeforeEndAge_True()
        {
            CLAManageViewModel model = new()
            {
                AgeStart = 5,
                AgeEnd = 10
            };
            ModelStateDictionary modelState = new();
            CLAgeEndAfterAgeStartLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // End age before start age should return false
        [Fact]
        public void Compare_StartAgeAfterEndAge_False()
        {
            CLAManageViewModel model = new()
            {
                AgeStart = 10,
                AgeEnd = 5
            };
            ModelStateDictionary modelState = new();
            CLAgeEndAfterAgeStartLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // 5 days in a week should return true
        [Fact]
        public void Set_WorkDaysInWeek_True()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDaysPerWeek = 5
            };
            ModelStateDictionary modelState = new();
            CLASevenWeekDaysLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // 8 days in a week should return false
        [Fact]
        public void Set_TooManyWorkDaysInWeek_False()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDaysPerWeek = 8
            };
            ModelStateDictionary modelState = new();
            CLASevenWeekDaysLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // 30 minutes in a day should return true
        [Fact]
        public void Set_WorkDurationPerDayMinutes_True()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerDay = 30,
                MaxDayDurationHours =  false
            };
            ModelStateDictionary modelState = new();
            CLASevenWeekDaysLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // 1550 minutes in a day should return false
        [Fact]
        public void Set_TooManyWorkDurationPerDayMinutes_False()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDaysPerWeek = 1550,
                MaxDayDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLASevenWeekDaysLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // 2 hours in a day should return true
        [Fact]
        public void Set_WorkDurationPerDayHours_True()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerDay = 2,
                MaxDayDurationHours = true
            };
            ModelStateDictionary modelState = new();
            CLASevenWeekDaysLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // 40 minutes in a day should return false
        [Fact]
        public void Set_TooManyWorkDurationPerDayHours_False()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDaysPerWeek = 40,
                MaxDayDurationHours = true
            };
            ModelStateDictionary modelState = new();
            CLASevenWeekDaysLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // Filling agestart and a rule should return true
        [Fact]
        public void Set_AgeRangeAndRule_True()
        {
            CLAManageViewModel model = new()
            {
                AgeStart = 19,
                MaxAvgWeeklyWorkDurationOverFourWeeks = 45
            };
            ModelStateDictionary modelState = new();
            CLAViewModelNotEmptyLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // Only filling agestart should return false
        [Fact]
        public void Set_OnlyRule_False()
        {
            CLAManageViewModel model = new()
            {
                AgeStart = 19
            };
            ModelStateDictionary modelState = new();
            CLAViewModelNotEmptyLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // Ten minutes for maxavgweeklyworkdurationoverfourweeks should return true
        [Fact]
        public void Set_AvgWeeklyWorkDurationOverFourWeekMinutes_True()
        {
            CLAManageViewModel model = new()
            {
                MaxAvgWeeklyWorkDurationOverFourWeeks = 10,
                MaxAvgDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLAValidTimePerFourWeekAverageLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // Twenythousand minutes for maxavgweeklyworkdurationoverfourweeks should return false
        [Fact]
        public void Set_TooManyAvgWeeklyWorkDurationOverFourWeekMinutes_False()
        {
            CLAManageViewModel model = new()
            {
                MaxAvgWeeklyWorkDurationOverFourWeeks = 20000,
                MaxAvgDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLAValidTimePerFourWeekAverageLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // Five hours for maxavgweeklyworkdurationoverfourweeks should return true
        [Fact]
        public void Set_AvgWeeklyWorkDurationOverFourWeekHours_True()
        {
            CLAManageViewModel model = new()
            {
                MaxAvgWeeklyWorkDurationOverFourWeeks = 5,
                MaxAvgDurationHours = true
            };
            ModelStateDictionary modelState = new();
            CLAValidTimePerFourWeekAverageLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // Twohundred hours for maxavgweeklyworkdurationoverfourweeks should return false
        [Fact]
        public void Set_TooManyAvgWeeklyWorkDurationOverFourWeekHours_False()
        {
            CLAManageViewModel model = new()
            {
                MaxAvgWeeklyWorkDurationOverFourWeeks = 200,
                MaxAvgDurationHours = true
            };
            ModelStateDictionary modelState = new();
            CLAValidTimePerFourWeekAverageLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // Ten minutes for maxworkdurationperweek should return true
        [Fact]
        public void Set_MaxWorkDurationPerWeekMinutes_True()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerWeek = 10,
                MaxWeekDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLAValidTimePerWeekLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // Twenythousand minutes for maxworkdurationperweek should return false
        [Fact]
        public void Set_TooManyMaxWorkDurationPerWeekMinutes_False()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerWeek = 20000,
                MaxWeekDurationHours = false
            };
            ModelStateDictionary modelState = new();
            CLAValidTimePerWeekLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }

        // Five hours for maxworkdurationperweek should return true
        [Fact]
        public void Set_MaxWorkDurationPerWeekHours_True()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerWeek = 5,
                MaxWeekDurationHours = true
            };
            ModelStateDictionary modelState = new();
            CLAValidTimePerWeekLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.True(result);
        }

        // Twohundred hours for maxworkdurationperweek should return false
        [Fact]
        public void Set_TooManyMaxWorkDurationPerWeekHours_False()
        {
            CLAManageViewModel model = new()
            {
                MaxWorkDurationPerWeek = 200,
                MaxWeekDurationHours = true
            };
            ModelStateDictionary modelState = new();
            CLAValidTimePerWeekLogic validator = new();

            bool result = validator.ValidateModel(model, modelState);

            Assert.False(result);
        }
    }
}
