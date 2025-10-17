using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RoboRent_BE.Controller.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        [AllowAnonymous]
        public IActionResult PublicEndpoint()
        {
            return Ok(new { message = "This is a public endpoint - no authentication required" });
        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult ProtectedEndpoint()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var accountId = User.FindFirst("accountId")?.Value;
            var accountStatus = User.FindFirst("accountStatus")?.Value;

            return Ok(new
            {
                message = "This is a protected endpoint - JWT authentication required",
                user = new
                {
                    id = userId,
                    email = userEmail,
                    name = userName,
                    accountId = accountId,
                    accountStatus = accountStatus
                },
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminEndpoint()
        {
            return Ok(new { message = "This is an admin-only endpoint" });
        }
    }
}
