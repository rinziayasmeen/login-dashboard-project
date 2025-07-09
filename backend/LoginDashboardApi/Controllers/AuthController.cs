using LoginDashboardApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;

namespace LoginDashboardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public AuthController(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }

        private static readonly List<(string Username, string Password)> Users = new()
        {
            ("admin", "admin123")
        };

        private const int MaxAttempts = 5;
        private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin login)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var cacheKey = $"login_attempts_{ip}";

            var attempt = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = Window;
                return 0;
            });

            if (attempt >= MaxAttempts)
            {
                return StatusCode(429, "Too many attempts. Try again later.");
            }

            _cache.Set(cacheKey, attempt + 1, Window);

            if (!Users.Any(u => u.Username == login.Username && u.Password == login.Password))
                return Unauthorized();

            // If login succeeds, reset attempts
            _cache.Remove(cacheKey);

            var token = GenerateJwtToken(login.Username);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string username)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                throw new Exception("JWT Secret Key is missing or too short. It must be at least 32 characters.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
