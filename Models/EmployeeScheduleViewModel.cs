namespace BumboSolid.Models
{
    public class EmployeeScheduleViewModel
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int WeekFromNoW { get; set; }

        public virtual List<ShiftViewModel> Shifts { get; set; } = new List<ShiftViewModel>();


    }
}
