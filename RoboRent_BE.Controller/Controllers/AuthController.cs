using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
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

            // Issue JWT token for the authenticated user
            var jwtSection = _configuration.GetSection("Jwt");
            var jwtSecret = jwtSection["Secret"];
            var jwtIssuer = jwtSection["Issuer"]; 
            var jwtAudience = jwtSection["Audience"]; 
            var jwtExpiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var m) ? m : 60;

            if (string.IsNullOrWhiteSpace(jwtSecret))
            {
                // Fallback: proceed without token if not configured
                return Redirect(returnUrl ?? "/");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // session when login with existed google account
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Name, fullName ?? user.UserName ?? string.Empty),
                new Claim("accountId", account.Id.ToString()),
                new Claim("accountStatus", account.Status ?? string.Empty)
            };

            var jwtToken = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiresMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            // If a returnUrl was provided, append token as a query param for SPA to capture
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                var separator = returnUrl.Contains("?") ? "&" : "?";
                var redirectWithToken = $"{returnUrl}{separator}token={Uri.EscapeDataString(tokenString)}";
                return Redirect(redirectWithToken);
            }

            // Otherwise return token in body for API consumers
            return Ok(new { token = tokenString });
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

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Generate new JWT token
            var jwtSection = _configuration.GetSection("Jwt");
            var jwtSecret = jwtSection["Secret"];
            var jwtIssuer = jwtSection["Issuer"];
            var jwtAudience = jwtSection["Audience"];
            var jwtExpiresMinutes = int.TryParse(jwtSection["ExpiresMinutes"], out var m) ? m : 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
            };

            // Get account info if exists
            var account = await _accountService.GetAccountByUserIdAsync(user.Id);
            if (account != null)
            {
                claims.Add(new Claim("accountId", account.Id.ToString()));
                claims.Add(new Claim("accountStatus", account.Status ?? string.Empty));
            }

            var jwtToken = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiresMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Ok(new { token = tokenString });
        }
    }
}


