using BumboSolid.Data.Models;
using BumboSolid.HelperClasses.CLARules;

namespace UnitTests
{
	public class CLARulesTests
	{
		// Adding shift with a duration lower than the maximum allowed shift duration should return true
		[Fact]
		public void Create_ShiftDurationBelowMaximum_True()
		{
			Shift shiftModel = new()
			{
				StartTime = new TimeOnly(9, 00),
				EndTime = new TimeOnly(11, 00),
			};
			CLAEntry rule = new()
			{
				MaxShiftDuration = 120
			};
			CLAApplyRules validator = new();

			bool result = validator.ShiftDuration(shiftModel, rule);

			Assert.True(result);
		}

		// Adding shift with a duration higher than the maximum allowed shift duration should return false
		[Fact]
		public void Create_ShiftDurationAboveMaximum_False()
		{
			Shift shiftModel = new()
			{
				StartTime = new TimeOnly(9, 00),
				EndTime = new TimeOnly(11, 00),
			};
			CLAEntry rule = new()
			{
				MaxShiftDuration = 60
			};
			CLAApplyRules validator = new();

			bool result = validator.ShiftDuration(shiftModel, rule);

			Assert.False(result);
		}

		// Adding shift in a four week period adding up to be lower than the maximum average should return true
		[Fact]
		public void Create_AvgShiftDurationOverFourWeekPeriodBelowMaximum_True()
		{
			Week firstWeek = new()
			{
				WeekNumber = 1,
				Year = 2025
			};
			Week secondWeek = new()
			{
				WeekNumber = 2,
				Year = 2025
			};
			List<Shift> model = new();
			Shift firstShift = new()
			{
				StartTime = new TimeOnly(12, 00),
				EndTime = new TimeOnly(21, 00),
				EmployeeId = 1,
				Week = firstWeek
			};
			Shift secondShift = new()
			{
				StartTime = new TimeOnly(12, 00),
				EndTime = new TimeOnly(18, 00),
				EmployeeId = 1,
				Week = secondWeek
			};
			CLAEntry rule = new()
			{
				MaxAvgWeeklyWorkDurationOverFourWeeks = 1
			};
			CLAApplyRules validator = new();

			model.Add(firstShift);
			bool result = validator.AvgWorkHoursOverFourWeeks(secondShift, rule, model, 1);

			Assert.True(result);
		}
	}
}
