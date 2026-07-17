using System.ComponentModel.DataAnnotations;
namespace Invoice.Models.Authentication;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public String EmailId { get; set; } = string.Empty;
    [Required]
    public String Password { get; set; } = String.Empty;
}
