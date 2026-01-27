using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Service.Interfaces;
using System.Security.Claims;
using RoboRent_BE.Model.DTOS.Account;

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
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ModifyIdentityUser> userManager,
            SignInManager<ModifyIdentityUser> signInManager,
            IAccountService accountService,
            IAuthService authService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpGet("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromQuery] string? returnUrl = null)
        {
            // Clear any stale authentication cookies before starting new login
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            var appUrl = _configuration["AppUrl"];
            var scheme = Request.Scheme; // "https"
            var host = Request.Host.Value;

            if (!string.IsNullOrEmpty(appUrl) && Uri.TryCreate(appUrl, UriKind.Absolute, out var uri))
            {
                scheme = uri.Scheme;
                host = uri.Authority;
            }

            var redirectUrl = $"{scheme}://{host}/api/Auth/google-callback";
            if (!string.IsNullOrEmpty(returnUrl))
            {
                redirectUrl += $"?returnUrl={Uri.EscapeDataString(returnUrl)}";
            }
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("google-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl = null)
        {
            try
            {
                // Check for error parameters (in case of remote failure)
                if (Request.Query.ContainsKey("error"))
                {
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return BadRequest(new { error = Request.Query["error"].ToString(), message = "Authentication failed. Please try logging in again." });
                }

                var appUrl = _configuration["AppUrl"];
                var scheme = Request.Scheme;
                var host = Request.Host.Value;

                if (!string.IsNullOrEmpty(appUrl) && Uri.TryCreate(appUrl, UriKind.Absolute, out var uri))
                {
                    scheme = uri.Scheme;
                    host = uri.Authority;
                }

                var result = await _authService.HandleGoogleCallbackAsync(returnUrl, (userId, token) =>
                    Url.ActionLink("Verify", "Auth", new { userId, token }, protocol: scheme, host: host));

                if (!string.IsNullOrEmpty(result.Error))
                {
                    // Clear cookies on error
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return BadRequest(new { error = result.Error, message = "Please try logging in again by visiting the google-login endpoint." });
                }

                if (!string.IsNullOrEmpty(result.RedirectUrl))
                {
                    return Redirect(result.RedirectUrl);
                }

                return Ok(new { token = result.Token });
            }
            catch (Exception ex)
            {
                // Clear all authentication cookies on any exception
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                // Log the exception if you have a logger
                return BadRequest(new 
                { 
                    error = "AuthenticationFailureException",
                    message = "An error occurred during authentication. Please try logging in again.",
                    details = ex.Message
                });
            }
        }

        [HttpGet("auth-error")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthError([FromQuery] string? error = null)
        {
            // Ensure all authentication cookies are cleared
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return BadRequest(new 
            { 
                error = error ?? "Authentication failed",
                message = "There was an error during authentication. Your cookies have been cleared. Please try logging in again."
            });
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
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found in token.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var userRoles = await _userManager.GetRolesAsync(user);
            var role = userRoles.FirstOrDefault() ?? "Customer";
            var accountId = User.FindFirst("accountId")?.Value;
            var accountStatus = User.FindFirst("accountStatus")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
            var picture = User.FindFirst("picture")?.Value;

            return Ok(new
            {
                userId = user.Id,
                email,
                firstName,
                lastName,
                picture,
                accountId,
                accountStatus,
                emailConfirmed = user.EmailConfirmed,
                role
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

        [HttpPatch("update-phone-number")]
        [Authorize]
        public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneNumberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _accountService.UpdatePhoneNumberAsync(userId, request.PhoneNumber);
            if (!success)
            {
                return NotFound(new { message = "Account not found." });
            }

            return Ok(new { message = "Phone number updated successfully.", phoneNumber = request.PhoneNumber });
        }
    }
}


