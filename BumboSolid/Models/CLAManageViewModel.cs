using System.ComponentModel.DataAnnotations;

namespace BumboSolid.Models;

public class CLAManageViewModel
{
    public int? Id { get; set; }

    [Range(0, 128, ErrorMessage = "Leeftijd moet een getal tussen 0 en 128 zijn")]
    public int? AgeStart { get; set; }

    [Range(0, 128, ErrorMessage = "Leeftijd moet een getal tussen 0 en 128 zijn")]
    public int? AgeEnd { get; set; }

    [RegularExpression(@"^\d+([\,](?:\d|(25)|(75)))?$", ErrorMessage = "Invalide waarde. Graag maximaal 1 kommagetal, of kwarten.")]
    public decimal? MaxWorkDurationPerDay { get; set; }

    public int? MaxWorkDaysPerWeek { get; set; }

    [RegularExpression(@"^\d+([\,](?:\d|(25)|(75)))?$", ErrorMessage = "Invalide waarde. Graag maximaal 1 kommagetal, of kwarten.")]
    public decimal? MaxWorkDurationPerWeek { get; set; }

    public TimeOnly? EarliestWorkTime { get; set; }

    public TimeOnly? LatestWorkTime { get; set; }

    [RegularExpression(@"^\d+([\,](?:\d|(25)|(75)))?$", ErrorMessage = "Invalide waarde. Graag maximaal 1 kommagetal, of kwarten.")]
    public decimal? MaxShiftDuration { get; set; }

    [RegularExpression(@"^\d+([\,](?:\d|(25)|(75)))?$", ErrorMessage = "Invalide waarde. Graag maximaal 1 kommagetal, of kwarten.")]
    public decimal? MaxAvgWeeklyWorkDurationOverFourWeeks { get; set; }

    public bool MaxDayDurationHours { get; set; } = true;

    public bool MaxWeekDurationHours { get; set; } = true;

    public bool MaxAvgDurationHours { get; set; } = true;

    public bool MaxTotalShiftDurationHours { get; set; } = true;
