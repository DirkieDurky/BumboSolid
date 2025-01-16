using Microsoft.AspNetCore.Identity;

namespace BumboSolid.Data.Models;

public partial class User : IdentityUser<int>
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; }

    public string? PlaceOfResidence { get; set; }

    public string? StreetName { get; set; }

    public int? StreetNumber { get; set; }

    public DateOnly BirthDate { get; set; }

    public DateOnly EmployedSince { get; set; }

    public virtual List<AvailabilityRule> AvailabilityRules { get; set; } = new List<AvailabilityRule>();

    public virtual List<FillRequest> FillRequests { get; set; } = new List<FillRequest>();

    public virtual List<Department> Departments { get; set; } = new List<Department>();

    public virtual List<Absence> Absences { get; set; } = new List<Absence>();

    public virtual List<Shift> Shifts { get; set; } = new List<Shift>();

    public virtual List<ClockedHours> ClockedHours { get; set; } = new List<ClockedHours>();

    // Added for custom display in dropdown
    public string Name => $"{FirstName} {LastName}";
}
