using Invoice.Authentication.Interfaces;
using Invoice.Authentication.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Authentication.Factory
{
    public class AuthenticationFactory : IAuthenticationFactory
    {
        private readonly IConfiguration configuration;
        private readonly IServiceProvider serviceProvider;
        public AuthenticationFactory(
        IConfiguration configuration, 
        IServiceProvider serviceProvider
            )
        { 
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
        }

        public IAuthenticationProvider Create()
        {
            var provider = configuration["Authentiucation:Provider"];

            return provider switch
            {
                "JWT" => serviceProvider.GetRequiredService<JwtAuthenticationProvider>() ?? new JwtAuthenticationProvider(),
                "OAuth2" => serviceProvider.GetRequiredService<OAuthAuthenticationProvider>() ?? new OAuthAuthenticationProvider(),
                _ => throw new Exception("Provider Not Found")
            };

        }
    }
}
