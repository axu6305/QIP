using System.ComponentModel.DataAnnotations;
using QIP.Web.Models;

namespace QIP.Web.ViewModels;

public class PatientFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Initials { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string HospitalNumber { get; set; } = string.Empty;

    [Required]
    public DateOnly? DateOfBirth { get; set; }

    [Required]
    public Sex? Sex { get; set; }

    [Required]
    public DateOnly? ReferenceDate { get; set; }
}
