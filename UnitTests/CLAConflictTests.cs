using BumboSolid.Data.Models;
using BumboSolid.HelperClasses;
using BumboSolid.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UnitTests
{
    public class CLAConflictTests
    {
        // Filling maxworkdurationperday for a new entry with an age range that already exists in the db should return false
        [Fact]
        public void Set_MaxWorkDurationPerDayWithExistingEntry_False()
        {
            CLAEntry model = new()
            {
                MaxWorkDurationPerDay = 1
            };
            CLAManageViewModel viewModel = new()
            {
                MaxWorkDurationPerDay = 2
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.False(result);
        }

        // Filling maxworkdurationperday for a new entry with an age range that does not already exists in the db should return true
        [Fact]
        public void Set_MaxWorkDurationPerDayWithNoExistingEntry_True()
        {
            CLAEntry model = new();
            CLAManageViewModel viewModel = new()
            {
                MaxWorkDurationPerDay = 2
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.True(result);
        }

        // Filling maxworkdaysperweek for a new entry with an age range that already exists in the db should return false
        [Fact]
        public void Set_MaxWorkDaysPerWeekWithExistingEntry_False()
        {
            CLAEntry model = new()
            {
                MaxWorkDaysPerWeek = 1
            };
            CLAManageViewModel viewModel = new()
            {
                MaxWorkDaysPerWeek = 7
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.False(result);
        }

        // Filling maxworkdaysperweek for a new entry with an age range that does not already exists in the db should return true
        [Fact]
        public void Set_MaxWorkDaysPerWeekWithNoExistingEntry_True()
        {
            CLAEntry model = new();
            CLAManageViewModel viewModel = new()
            {
                MaxWorkDaysPerWeek = 4
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.True(result);
        }

        // Filling maxworkdurationperweek for a new entry with an age range that already exists in the db should return false
        [Fact]
        public void Set_MaxWorkDurationPerWeekWithExistingEntry_False()
        {
            CLAEntry model = new()
            {
                MaxWorkDurationPerWeek = 35
            };
            CLAManageViewModel viewModel = new()
            {
                MaxWorkDurationPerWeek = 19
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.False(result);
        }

        // Filling maxworkdurationperweek for a new entry with an age range that does not already exists in the db should return true
        [Fact]
        public void Set_MaxWorkDurationPerWeekWithNoExistingEntry_True()
        {
            CLAEntry model = new();
            CLAManageViewModel viewModel = new()
            {
                MaxWorkDurationPerWeek = 8
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.True(result);
        }

        // Filling earlistworktime for a new entry with an age range that already exists in the db should return false
        [Fact]
        public void Set_EarliestWorkTimeWithExistingEntry_False()
        {
            TimeOnly modelTime = new(9, 30);
            TimeOnly viewModelTime = new(10, 15);
            CLAEntry model = new()
            {
                EarliestWorkTime = modelTime
            };
            CLAManageViewModel viewModel = new()
            {
                EarliestWorkTime = viewModelTime
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.False(result);
        }

        // Filling earlistworktime for a new entry with an age range that does not already exists in the db should return true
        [Fact]
        public void Set_EarliestWorkTimeWithNoExistingEntry_True()
        {
            TimeOnly viewModelTime = new(8, 45);
            CLAEntry model = new();
            CLAManageViewModel viewModel = new()
            {
                EarliestWorkTime = viewModelTime,
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.True(result);
        }

        // Filling latestworktime for a new entry with an age range that already exists in the db should return false
        [Fact]
        public void Set_LatestWorkTimeWithExistingEntry_False()
        {
            TimeOnly modelTime = new(17, 30);
            TimeOnly viewModelTime = new(18, 30);
            CLAEntry model = new()
            {
                LatestWorkTime = modelTime
            };
            CLAManageViewModel viewModel = new()
            {
                LatestWorkTime = viewModelTime
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.False(result);
        }

        // Filling latestworktime for a new entry with an age range that does not already exists in the db should return true
        [Fact]
        public void Set_LatestWorkTimeWithNoExistingEntry_True()
        {
            TimeOnly viewModelTime = new(16, 15);
            CLAEntry model = new();
            CLAManageViewModel viewModel = new()
            {
                LatestWorkTime = viewModelTime,
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.True(result);
        }

        // Filling maxshiftduration for a new entry with an age range that already exists in the db should return false
        [Fact]
        public void Set_MaxShiftDurationWithExistingEntry_True()
        {
            CLAEntry model = new()
            {
                MaxShiftDuration = 13
            };
            CLAManageViewModel viewModel = new()
            {
                MaxShiftDuration = 5
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.False(result);
        }

        // Filling maxshiftduration for a new entry with an age range that does not already exists in the db should return true
        [Fact]
        public void Set_MaxShiftDurationWithNoExistingEntry_False()
        {
            CLAEntry model = new();
            CLAManageViewModel viewModel = new()
            {
                MaxShiftDuration = 9
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.True(result);
        }

        // Filling maxavgweeklyworkdurationoverfourweeks for a new entry with an age range that already exists in the db should return false
        [Fact]
        public void Set_MaxAvgWeeklyWorkDurationOverFourWeeksWithExistingEntry_True()
        {
            CLAEntry model = new()
            {
                MaxAvgWeeklyWorkDurationOverFourWeeks = 33
            };
            CLAManageViewModel viewModel = new()
            {
                MaxAvgWeeklyWorkDurationOverFourWeeks = 19
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.False(result);
        }

        // Filling maxavgweeklyworkdurationoverfourweeks for a new entry with an age range that does not already exists in the db should return true
        [Fact]
        public void Set_MaxAvgWeeklyWorkDurationOverFourWeeksWithNoExistingEntry_False()
        {
            CLAEntry model = new();
            CLAManageViewModel viewModel = new()
            {
                MaxAvgWeeklyWorkDurationOverFourWeeks = 22
            };
            ModelStateDictionary modelState = new();
            CLANoConflictFields validator = new();

            bool result = validator.NoConflicts(model, viewModel, modelState);

            Assert.True(result);
        }
    }
}
