using Invoice.Authentication.Interfaces;

namespace Invoice.Authentication.Factory;

public interface IAuthenticationFactory
{
    IAuthenticationProvider Create();
}
