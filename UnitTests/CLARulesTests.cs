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
				EndTime = new TimeOnly(13, 00),
				EmployeeId = 1,
				Week = firstWeek
			};
			Shift secondShift = new()
			{
				StartTime = new TimeOnly(12, 00),
				EndTime = new TimeOnly(15, 00),
				EmployeeId = 1,
				Week = secondWeek
			};
			CLAEntry rule = new()
			{
				MaxAvgWeeklyWorkDurationOverFourWeeks = 60
			};
			CLAApplyRules validator = new();

			model.Add(firstShift);
			bool result = validator.AvgWorkHoursOverFourWeeks(secondShift, rule, model, 1);

			Assert.True(result);
		}

		// Adding shift in a four week period adding up to be higher than the maximum average should return false
		[Fact]
		public void Create_AvgShiftDurationOverFourWeekPeriodAboveMaximum_False()
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
				EndTime = new TimeOnly(20, 00),
				EmployeeId = 1,
				Week = secondWeek
			};
			CLAEntry rule = new()
			{
				MaxAvgWeeklyWorkDurationOverFourWeeks = 120
			};
			CLAApplyRules validator = new();

			model.Add(firstShift);
			bool result = validator.AvgWorkHoursOverFourWeeks(secondShift, rule, model, 1);

			Assert.False(result);
		}

		// Adding shift with an end time lower than the latest allowed work time should return true
		[Fact]
		public void Create_ShiftEndTimeBelowStopTime_True()
		{
			Shift shiftModel = new()
			{
				StartTime = new TimeOnly(13, 00),
				EndTime = new TimeOnly(17, 00),
			};
			CLAEntry rule = new()
			{
				LatestWorkTime = new TimeOnly(18, 00),
			};
			CLAApplyRules validator = new();

			bool result = validator.LatestWorkTime(shiftModel, rule);

			Assert.True(result);
		}

		// Adding shift with an end time higher than the latest allowed work time should return false
		[Fact]
		public void Create_ShiftEndTimeAboveStopTime_False()
		{
			Shift shiftModel = new()
			{
				StartTime = new TimeOnly(12, 00),
				EndTime = new TimeOnly(19, 00),
			};
			CLAEntry rule = new()
			{
				LatestWorkTime = new TimeOnly(13, 00),
			};
			CLAApplyRules validator = new();

			bool result = validator.LatestWorkTime(shiftModel, rule);

			Assert.False(result);
		}

		// Adding shift with a start time higher than the earliest allowed work time should return true
		[Fact]
		public void Create_ShiftStartTimeAboveStartTime_True()
		{
			Shift shiftModel = new()
			{
				StartTime = new TimeOnly(9, 00),
				EndTime = new TimeOnly(11, 00),
			};
			CLAEntry rule = new()
			{
				EarliestWorkTime = new TimeOnly(7, 00),
			};
			CLAApplyRules validator = new();

			bool result = validator.EarliestWorkTime(shiftModel, rule);

			Assert.True(result);
		}

		// Adding shift with a start time lower than the earliest allowed work time should return false
		[Fact]
		public void Create_ShiftStartTimeBelowStartTime_False()
		{
			Shift shiftModel = new()
			{
				StartTime = new TimeOnly(7, 00),
				EndTime = new TimeOnly(13, 00),
			};
			CLAEntry rule = new()
			{
				EarliestWorkTime = new TimeOnly(9, 00),
			};
			CLAApplyRules validator = new();

			bool result = validator.EarliestWorkTime(shiftModel, rule);

			Assert.False(result);
		}

		// Adding shift in a week period adding up to be lower than the maximum allowed time should return true
		[Fact]
		public void Create_TotalShiftDurationBelowMaximumInWeek_True()
		{
			Week week = new()
			{
				WeekNumber = 1,
				Year = 2025
			};
			List<Shift> model = new();
			Shift firstShift = new()
			{
				StartTime = new TimeOnly(9, 00),
				EndTime = new TimeOnly(11, 00),
				Week = week
			};
			Shift secondShift = new()
			{
				StartTime = new TimeOnly(14, 00),
				EndTime = new TimeOnly(17, 00),
				Week = week
			};
			CLAEntry rule = new()
			{
				MaxWorkDurationPerWeek = 600
			};
			CLAApplyRules validator = new();

			model.Add(firstShift);
			bool result = validator.MaxWorkDurationPerWeek(secondShift, rule, model);

			Assert.True(result);
		}

		// Adding shift in a week period adding up to be higher than the maximum allowed time should return false
		[Fact]
		public void Create_TotalShiftDurationAboveMaximumInWeek_False()
		{
			Week week = new()
			{
				WeekNumber = 1,
				Year = 2025
			};
			List<Shift> model = new();
			Shift firstShift = new()
			{
				StartTime = new TimeOnly(7, 00),
				EndTime = new TimeOnly(13, 00),
				Week = week
			};
			Shift secondShift = new()
			{
				StartTime = new TimeOnly(9, 00),
				EndTime = new TimeOnly(15, 00),
				Week = week
			};
			CLAEntry rule = new()
			{
				MaxWorkDurationPerWeek = 120
			};
			CLAApplyRules validator = new();

			model.Add(firstShift);
			bool result = validator.MaxWorkDurationPerWeek(secondShift, rule, model);

			Assert.False(result);
		}

		// Adding shift in a week period adding up to be lower than the maximum allowed shifts should return true
		[Fact]
		public void Create_TotalShiftsBelowMaximum_True()
		{
			Week week = new()
			{
				WeekNumber = 1,
				Year = 2025
			};
			List<Shift> model = new();
			Shift firstShift = new()
			{
				StartTime = new TimeOnly(11, 00),
				EndTime = new TimeOnly(12, 00),
				Week = week,
				Weekday = 5
			};
			Shift secondShift = new()
			{
				StartTime = new TimeOnly(9, 00),
				EndTime = new TimeOnly(18, 00),
				Week = week,
				Weekday = 5
			};
			CLAEntry rule = new()
			{
				MaxWorkDaysPerWeek = 1
			};
			CLAApplyRules validator = new();

			model.Add(firstShift);
			bool result = validator.MaxWorkDaysPerWeek(secondShift, rule, model);

			Assert.True(result);
		}

		// Adding shift in a week period adding up to be higher than the maximum allowed shifts should return false
		[Fact]
		public void Create_TotalShiftsAboveMaximum_False()
		{
			Week week = new()
			{
				WeekNumber = 1,
				Year = 2025
			};
			List<Shift> model = new();
			Shift firstShift = new()
			{
				StartTime = new TimeOnly(7, 00),
				EndTime = new TimeOnly(13, 00),
				Week = week,
				Weekday = 1
			};
			Shift secondShift = new()
			{
				StartTime = new TimeOnly(16, 00),
				EndTime = new TimeOnly(17, 00),
				Week = week,
				Weekday = 2
			};
			CLAEntry rule = new()
			{
				MaxWorkDaysPerWeek = 1
			};
			CLAApplyRules validator = new();

			model.Add(firstShift);
			bool result = validator.MaxWorkDaysPerWeek(secondShift, rule, model);

			Assert.False(result);
		}

		// Adding shift in a day period adding up to be lower than the maximum allowed time should return true
		[Fact]
		public void Create_TotalShiftDurationBelowMaximumInDay_True()
		{
			Week week = new()
			{
				WeekNumber = 1,
				Year = 2025
			};
			List<Shift> model = new();
			Shift firstShift = new()
			{
				StartTime = new TimeOnly(9, 00),
				EndTime = new TimeOnly(10, 00),
				Week = week,
				Weekday = 2,
				EmployeeId = 1
			};
			Shift secondShift = new()
			{
				StartTime = new TimeOnly(10, 00),
				EndTime = new TimeOnly(11, 00),
				Week = week,
				Weekday = 2,
				EmployeeId = 1
			};
			CLAEntry rule = new()
			{
				MaxWorkDurationPerDay = 120
			};
			CLAApplyRules validator = new();

			model.Add(firstShift);
			bool result = validator.MaxWorkDurationPerDay(secondShift, rule, model, 1);

			Assert.True(result);
		}

		// Adding shift in a day period adding up to be higher than the maximum allowed time should return false
		[Fact]
		public void Create_TotalShiftDurationAboveMaximumInDay_False()
		{
			Week week = new()
			{
				WeekNumber = 1,
				Year = 2025
			};
			List<Shift> model = new();
			Shift firstShift = new()
			{
				StartTime = new TimeOnly(7, 00),
				EndTime = new TimeOnly(9, 00),
				Week = week,
				Weekday = 2,
				EmployeeId = 1
			};
			Shift secondShift = new()
			{
				StartTime = new TimeOnly(12, 00),
				EndTime = new TimeOnly(13, 00),
				Week = week,
				Weekday = 2,
				EmployeeId = 1
			};
			CLAEntry rule = new()
			{
				MaxWorkDurationPerDay = 120
			};
			CLAApplyRules validator = new();

			model.Add(firstShift);
			bool result = validator.MaxWorkDurationPerDay(secondShift, rule, model, 1);

			Assert.False(result);
		}
	}
}
