using Invoice.Models.Authentication;

namespace Invoice.Authentication.Interfaces;

public interface IAuthenticationProvider
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
