using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IConfiguration _configuration;

        public LoginController(ILogger<ProductController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {

            if (!ModelState.IsValid)
                return BadRequest();
            HttpContext.Session.SetString("MyTest", "My set value test");

            var configUsername = _configuration["UserCredentials:Username"];
            var configPassword = _configuration["UserCredentials:Password"];

            if (dto.UserName != configUsername || dto.Password != configPassword)
                return Unauthorized();
            var jwtKey = _configuration["Jwt:Key"]; // "v2UJQxTrwUCqqJkehkxvSUZKQCX6gNmRWq7q1bWa3Jw="; // Use config in real app
            var jwtIssuer = _configuration["Jwt:Issuer"]; // "yourapiissuer";

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, dto.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var cre = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: cre
                );
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });

        }
    }
}
