using QIP.Web.Models;

namespace QIP.Web.ViewModels;

public class WardDashboardViewModel
{
    public DateOnly ReferenceDate { get; set; }

    public IReadOnlyList<DateOnly> Dates { get; set; } = [];

    public IReadOnlyList<PatientRowViewModel> Patients { get; set; } = [];
}

public class PatientRowViewModel
{
    public int Id { get; set; }

    public string Surname { get; set; } = string.Empty;

    public string Initials { get; set; } = string.Empty;

    public string HospitalNumber { get; set; } = string.Empty;

    public string DobDisplay { get; set; } = string.Empty;

    public string DateOfBirthIso { get; set; } = string.Empty;

    public Sex Sex { get; set; }

    public int TotalWardDays { get; set; }

    public DateOnly? LastPresenceDate { get; set; }

    public IReadOnlyList<WardDayCellViewModel> Days { get; set; } = [];
}

public class WardDayCellViewModel
{
    public DateOnly Date { get; set; }

    public bool IsPresent { get; set; }

    public bool IsCurrentDate { get; set; }
}
