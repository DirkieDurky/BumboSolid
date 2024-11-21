using System;
using System.Collections.Generic;

namespace BumboSolid.Data.Models;

public partial class Employee
{
    public int AspNetUserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PlaceOfResidence { get; set; }

    public string? StreetName { get; set; }

    public int? StreetNumber { get; set; }

    public DateOnly BirthDate { get; set; }

    public DateOnly EmployedSince { get; set; }

    public virtual List<AvailabilityDay> AvailabilityDays { get; set; } = new List<AvailabilityDay>();

    public virtual List<FillRequest> FillRequests { get; set; } = new List<FillRequest>();
}
