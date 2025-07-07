using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginDashboardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        [HttpGet("data")]
        [Authorize]
        public IActionResult GetData()
        {
            var data = new[]
            {
                new { Status = "Open", Count = 10 },
                new { Status = "In Progress", Count = 15 },
                new { Status = "Closed", Count = 5 },
            };
            return Ok(data);
        }
    }
}
