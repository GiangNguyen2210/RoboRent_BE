using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Service.Interfaces;
using System.Security.Claims;

namespace RoboRent_BE.Controller.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ModifyIdentityUser> _userManager;
        private readonly SignInManager<ModifyIdentityUser> _signInManager;
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService;

        public AuthController(
            UserManager<ModifyIdentityUser> userManager,
            SignInManager<ModifyIdentityUser> signInManager,
            IAccountService accountService,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _authService = authService;
        }

        [HttpGet("google-login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin([FromQuery] string? returnUrl = null)
        {
            var redirectUrl = Url.ActionLink("GoogleCallback", "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("google-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl = null)
        {
            var result = await _authService.HandleGoogleCallbackAsync(returnUrl, (userId, token) =>
                Url.ActionLink("Verify", "Auth", new { userId, token }));

            if (!string.IsNullOrEmpty(result.Error))
            {
                return BadRequest(result.Error);
            }

            if (!string.IsNullOrEmpty(result.RedirectUrl))
            {
                return Redirect(result.RedirectUrl);
            }

            return Ok(new { token = result.Token });
        }

        [HttpGet("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> Verify([FromQuery] string userId, [FromQuery] string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _accountService.ActivateAccountAsync(user.Id);
            user.Status = "Active";
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Account verified and activated." });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var accountId = User.FindFirst("accountId")?.Value;
            var accountStatus = User.FindFirst("accountStatus")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found in token.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new
            {
                userId = user.Id,
                email = user.Email,
                userName = user.UserName,
                accountId = accountId,
                accountStatus = accountStatus,
                emailConfirmed = user.EmailConfirmed
            });
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found in token.");
            }

            var token = await _authService.GenerateJwtForUserAsync(userId);
            if (token == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new { token });
        }
    }
}


