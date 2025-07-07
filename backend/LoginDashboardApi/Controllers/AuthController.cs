using LoginDashboardApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace LoginDashboardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static readonly List<(string Username, string Password)> Users = new()
        {
            ("admin", "admin123")
        };

        private static readonly Dictionary<string, (int Count, DateTime Timestamp)> RateLimitTracker = new();
        private const int MaxAttempts = 5;
        private const int WindowSeconds = 60;

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin login)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;

            if (RateLimitTracker.ContainsKey(ip))
            {
                var (count, timestamp) = RateLimitTracker[ip];
                if ((now - timestamp).TotalSeconds <= WindowSeconds)
                {
                    if (count >= MaxAttempts)
                        return StatusCode(429, "Too many attempts. Try later.");
                    RateLimitTracker[ip] = (count + 1, timestamp);
                }
                else
                {
                    RateLimitTracker[ip] = (1, now);
                }
            }
            else
            {
                RateLimitTracker[ip] = (1, now);
            }

            if (!Users.Any(u => u.Username == login.Username && u.Password == login.Password))
                return Unauthorized();

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
