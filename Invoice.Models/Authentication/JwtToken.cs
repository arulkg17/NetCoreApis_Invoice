
namespace Invoice.Models.Authentication;

public class JwtToken
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime Expiration { get; set; }

    public string TokenType { get; set; } = "Bearer";
}
