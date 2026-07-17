using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Invoice.Authentication.Token
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(
            int userId,
            string userName,
            string email,
            string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName,userName),
                new Claim(JwtRegisteredClaimNames.Email,email),

                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Email,email),
                new Claim(ClaimTypes.Role,role),

                new Claim(JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var token =
                new JwtSecurityToken(

                    issuer: _configuration["Jwt:Issuer"],

                    audience: _configuration["Jwt:Audience"],

                    claims: claims,

                    expires: DateTime.UtcNow.AddHours(2),

                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}