namespace QIP.Web.Models;

public class WardPresence
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public DateOnly Date { get; set; }

    public Patient Patient { get; set; } = null!;
}
