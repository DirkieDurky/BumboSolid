using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BumboSolid.Data.Models;

public partial class Employee:IdentityUser<int>
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
}
