using BumboSolid.Data.Models;

namespace BumboSolid.HelperClasses.CLARules
{
	public class CLAApplyRules
	{
		public bool ApplyCLARules(Shift shift, List<CLAEntry> CLAs, List<Shift> shifts, int userId)
		{
			bool validShift = true;

			foreach (CLAEntry CLA in CLAs)
			{
				// Shift duration
				if ((shift.EndTime - shift.StartTime).TotalMinutes > CLA.MaxShiftDuration) validShift = false;

				// Average works hours over a span of 4 weeks
				var lastFourWeeksShifts = shifts.Where(s => s.EmployeeId == userId && shift.Week.WeekNumber - s.Week.WeekNumber < 3 && s.Week.Year == shift.Week.Year).ToList();
				var lastFourWeeksTotalMinutes = (shift.EndTime - shift.StartTime).Minutes;
				foreach (Shift pastShift in lastFourWeeksShifts) lastFourWeeksTotalMinutes = lastFourWeeksTotalMinutes + (pastShift.EndTime - pastShift.StartTime).Minutes;
				if (lastFourWeeksTotalMinutes > CLA.MaxAvgWeeklyWorkDurationOverFourWeeks) validShift = false;

				// Latest work time
				if (shift.EndTime > CLA.LatestWorkTime) validShift = false;

				// Earliest work time
				if (shift.StartTime < CLA.EarliestWorkTime) validShift = false;

				// Max work duration per week
				var thisWeekShifts = shifts.Where(s => s.EmployeeId == userId && shift.Week.WeekNumber == s.Week.WeekNumber && s.Week.Year == shift.Week.Year).ToList();
				var thisWeekTotalMinutes = (shift.EndTime - shift.StartTime).Minutes;
				foreach (Shift pastShift in thisWeekShifts) thisWeekTotalMinutes = thisWeekTotalMinutes + (pastShift.EndTime - pastShift.StartTime).Minutes;
				if (thisWeekTotalMinutes > CLA.MaxWorkDurationPerWeek) validShift = false;

				// Max work days per week
				List<int> workDays = new List<int>();
				workDays.Add(shift.Weekday);
				foreach (Shift pastShift in thisWeekShifts) if (workDays.Contains(pastShift.Weekday) == false) workDays.Add(pastShift.Weekday);
				if (workDays.Count > CLA.MaxWorkDaysPerWeek) validShift = false;

				// Max work duration per day
				var todayShifts = shifts.Where(s => s.EmployeeId == userId && shift.Weekday == s.Weekday && shift.Week.WeekNumber == s.Week.WeekNumber && s.Week.Year == shift.Week.Year).ToList();
				var todayTotalMinutes = (shift.EndTime - shift.StartTime).Minutes;
				foreach (Shift pastShift in todayShifts) todayTotalMinutes = todayTotalMinutes + (pastShift.EndTime - pastShift.StartTime).Minutes;
				if (todayTotalMinutes > CLA.MaxWorkDurationPerDay) validShift = false;

				// Max work duration per holiday week
				var todayShifts = shifts.Where(s => s.EmployeeId == userId && shift.Weekday == s.Weekday && shift.Week.WeekNumber == s.Week.WeekNumber && s.Week.Year == shift.Week.Year).ToList();
				var todayTotalMinutes = (shift.EndTime - shift.StartTime).Minutes;
				foreach (Shift pastShift in todayShifts) todayTotalMinutes = todayTotalMinutes + (pastShift.EndTime - pastShift.StartTime).Minutes;
				if (todayTotalMinutes > CLA.MaxWorkDurationPerDay) validShift = false;
			}

			return validShift;
		}
	}
}
