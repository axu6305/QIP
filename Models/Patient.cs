using System.ComponentModel.DataAnnotations;

namespace QIP.Web.Models;

public class Patient
{
    public int Id { get; set; }

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
    public DateOnly DateOfBirth { get; set; }

    [Required]
    public Sex Sex { get; set; }

    public ICollection<WardPresence> WardPresences { get; set; } = new List<WardPresence>();
}
