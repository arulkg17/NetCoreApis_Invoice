using Invoice.Authentication.Interfaces;
using Invoice.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Authentication.Providers;

public class OAuthAuthenticationProvider: IAuthenticationProvider
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request) { 
        throw new NotImplementedException();
    }
}
