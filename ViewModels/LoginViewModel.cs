using System.ComponentModel.DataAnnotations;

namespace QIP.Web.ViewModels;

public class LoginViewModel
{
    [Required]
    [RegularExpression("^\\d{4}$", ErrorMessage = "Enter a 4 digit code.")]
    public string Code { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}
