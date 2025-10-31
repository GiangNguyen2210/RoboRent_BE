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
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ModifyIdentityUser> userManager,
            SignInManager<ModifyIdentityUser> signInManager,
            IAccountService accountService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
            _emailService = emailService;
            _configuration = configuration;
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
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("External login info not found.");
            }

            // Try to sign in the user with this external login provider
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
            ModifyIdentityUser user;
            if (!signInResult.Succeeded)
            {
                // Create the user if not exists
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email is required from Google.");
                }

                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ModifyIdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = false,
                        Status = "PendingVerification"
                    };
                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        return BadRequest(createResult.Errors);
                    }

                    // Link external login
                    var addLogin = await _userManager.AddLoginAsync(user, info);
                    if (!addLogin.Succeeded)
                    {
                        return BadRequest(addLogin.Errors);
                    }
                }
                else
                {
                    // Ensure external login linked
                    var addLogin = await _userManager.AddLoginAsync(user, info);
                    // ignore if already linked
                }

                await _signInManager.SignInAsync(user, isPersistent: true);
            }
            else
            {
                user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            }

            // Create Account with PendingVerification if not exists
            var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);
            var account = await _accountService.CreatePendingAccountAsync(user.Id, fullName);

            // If first time (PendingVerification), send verification email
            if (account.Status == "PendingVerification")
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var verifyUrl = Url.ActionLink("Verify", "Auth", new { userId = user.Id, token });
                var html = $"<p>Hi {fullName},</p><p>Please verify your account:</p><p><a href=\"{verifyUrl}\">Verify Account</a></p>";
                await _emailService.SendEmailAsync(user.Email!, "Verify your RoboRent account", html);
            }

            return Redirect(returnUrl ?? "/");
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


