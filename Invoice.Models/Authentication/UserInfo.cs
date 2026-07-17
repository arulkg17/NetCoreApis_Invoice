namespace Invoice.Models.Authentication;

public class UserInfo
{
    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string EmailId { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
