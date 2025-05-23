﻿using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Data.Models;

public class Shift
{
    public int Id { get; set; }

    public int WeekId { get; set; }

    public byte Weekday { get; set; }

    public string Department { get; set; } = null!;

    [Required(ErrorMessage = "Dit veld is vereist")]
    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "Dit veld is vereist")]
    public TimeOnly EndTime { get; set; }

    public int? EmployeeId { get; set; }

    public virtual User? Employee { get; set; } = null!;

    public string? ExternalEmployeeName { get; set; }

    public byte IsBreak { get; set; }

    public virtual Department? DepartmentNavigation { get; set; } = null!;

    public virtual List<FillRequest> FillRequests { get; set; } = new List<FillRequest>();

    public virtual Week? Week { get; set; } = null!;

    public string? EmployeeName => Employee?.Name ?? ExternalEmployeeName;
}
