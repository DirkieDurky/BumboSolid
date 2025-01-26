using BumboSolid.Data.Models;

namespace BumboSolid.HelperClasses.CLARules
{
	public class CLAApplyRules
	{
		public bool ApplyCLARules(Shift shift, List<CLAEntry> CLAs, List<Shift> shifts)
		{
			bool validShift = true;
			int userId = (int)shift.EmployeeId;

			foreach (CLAEntry CLA in CLAs)
			{
				var thisWeekShifts = shifts.Where(s => s.EmployeeId == userId && shift.Week.WeekNumber == s.Week.WeekNumber && s.Week.Year == shift.Week.Year).ToList();

				if (ShiftDuration(shift, CLA) == false) validShift = false;
				if (AvgWorkHoursOverFourWeeks(shift, CLA, shifts, userId) == false) validShift = false;
				if (LatestWorkTime(shift, CLA) == false) validShift = false;
				if (EarliestWorkTime(shift, CLA) == false) validShift = false;
				if (MaxWorkDurationPerWeek(shift, CLA, thisWeekShifts) == false) validShift = false;
				if (MaxWorkDaysPerWeek(shift, CLA, thisWeekShifts) == false) validShift = false;
				if (MaxWorkDurationPerDay(shift, CLA, shifts, userId) == false) validShift = false;
			}

			return validShift;
		}

		// Shift duration
		public bool ShiftDuration(Shift shift, CLAEntry CLA)
		{
			if ((shift.EndTime - shift.StartTime).TotalMinutes > CLA.MaxShiftDuration) return false;
			return true;
		}

		// Average works hours over a span of 4 weeks
		public bool AvgWorkHoursOverFourWeeks(Shift shift, CLAEntry CLA, List<Shift> shifts, int userId)
		{
			var lastFourWeeksShifts = shifts.Where(s => s.EmployeeId == userId && shift.Week.WeekNumber - s.Week.WeekNumber < 4 && s.Week.Year == shift.Week.Year).ToList();
			var lastFourWeeksTotalMinutes = (shift.EndTime - shift.StartTime).TotalMinutes;
			foreach (Shift pastShift in lastFourWeeksShifts) lastFourWeeksTotalMinutes = lastFourWeeksTotalMinutes + (pastShift.EndTime - pastShift.StartTime).TotalMinutes;
			if (lastFourWeeksTotalMinutes > (CLA.MaxAvgWeeklyWorkDurationOverFourWeeks*4)) return false;
			return true;
		}

		// Latest work time
		public bool LatestWorkTime(Shift shift, CLAEntry CLA)
		{
			if (shift.EndTime > CLA.LatestWorkTime) return false;
			return true;
		}

		// Earliest work time
		public bool EarliestWorkTime(Shift shift, CLAEntry CLA)
		{
			if (shift.StartTime < CLA.EarliestWorkTime) return false;
			return true;
		}

		// Max work duration per week
		public bool MaxWorkDurationPerWeek(Shift shift, CLAEntry CLA, List<Shift> shifts)
		{
			var thisWeekTotalMinutes = (shift.EndTime - shift.StartTime).TotalMinutes;
			foreach (Shift pastShift in shifts) thisWeekTotalMinutes = thisWeekTotalMinutes + (pastShift.EndTime - pastShift.StartTime).TotalMinutes;
			if (thisWeekTotalMinutes > CLA.MaxWorkDurationPerWeek) return false;
			return true;
		}

		// Max work days per week
		public bool MaxWorkDaysPerWeek(Shift shift, CLAEntry CLA, List<Shift> shifts)
		{
			List<int> workDays = new List<int>();
			workDays.Add(shift.Weekday);
			foreach (Shift pastShift in shifts)
			{
				if (pastShift.Id == shift.Id) continue;
				if (workDays.Contains(pastShift.Weekday) == false) workDays.Add(pastShift.Weekday);
			}
			if (workDays.Count > CLA.MaxWorkDaysPerWeek) return false;
			return true;
		}

		// Max work duration per day
		public bool MaxWorkDurationPerDay(Shift shift, CLAEntry CLA, List<Shift> shifts, int userId)
		{
			var todayShifts = shifts.Where(s => s.EmployeeId == userId && shift.Weekday == s.Weekday && shift.Week.WeekNumber == s.Week.WeekNumber && s.Week.Year == shift.Week.Year).ToList();
			var todayTotalMinutes = (shift.EndTime - shift.StartTime).TotalMinutes;
			foreach (Shift pastShift in todayShifts)
			{
				if (pastShift.Id == shift.Id) continue;
				todayTotalMinutes = todayTotalMinutes + (pastShift.EndTime - pastShift.StartTime).TotalMinutes;
			}
			if (todayTotalMinutes > CLA.MaxWorkDurationPerDay) return false;
			return true;
		}
	}
}