
using Invoice.Authentication.Providers;
using Invoice.Authentication.Token;
using Microsoft.Extensions.DependencyInjection;

namespace Invoice.Authentication.Extensions;

public static class ServiceCdollectionExtension
{
    public static IServiceCollection AddAuthenticationLibrary(
        this IServiceCollection services
        )
    {
        services.AddScoped<JwtAuthenticationProvider>();
        services.AddScoped<OAuthAuthenticationProvider>();
        services.AddScoped<JwtTokenGenerator>();
        services.AddScoped<TokenValidator>();
        return services;
            }
}
