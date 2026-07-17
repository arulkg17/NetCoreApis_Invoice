using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Invoice.Authentication.Token
{
    public class TokenValidator
    {
        private readonly IConfiguration _configuration;

        public TokenValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Validate(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                handler.ValidateToken(
                    token,

                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer =
                            _configuration["Jwt:Issuer"],

                        ValidAudience =
                            _configuration["Jwt:Audience"],

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    _configuration["Jwt:Key"]!))
                    },

                    out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}