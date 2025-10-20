using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoboRent_BE.Service.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ModifyIdentityUser> _userManager;
    private readonly SignInManager<ModifyIdentityUser> _signInManager;
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(
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

    public async Task<AuthResultDto> HandleGoogleCallbackAsync(
    string? returnUrl,
    Func<string, string, string?> buildVerifyUrl)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return new AuthResultDto { Error = "External login info not found." };
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: true);

        ModifyIdentityUser user;
        if (!signInResult.Succeeded)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(email))
            {
                return new AuthResultDto { Error = "Email is required from Google." };
            }

            user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // Create new user
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
                    return new AuthResultDto { Error = string.Join("; ", createResult.Errors.Select(e => e.Description)) };
                }

                
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                    return new AuthResultDto { Error = string.Join("; ", addLoginResult.Errors.Select(e => e.Description)) };
                }
            }
            else
            {
                // User exists, check if login already exists
                var existingLogin = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (existingLogin == null)
                {
                    // Add login only if it doesn't exist
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        return new AuthResultDto { Error = string.Join("; ", addLoginResult.Errors.Select(e => e.Description)) };
                    }
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: true);
        }
        else
        {
            user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey)!;
        }

        var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);
        var account = await _accountService.CreatePendingAccountAsync(user.Id, fullName);

        if (account.Status == "PendingVerification")
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verifyUrl = buildVerifyUrl(user.Id, token);
            var html = $"<p>Hi {fullName},</p><p>Please verify your account:</p><p><a href=\"{verifyUrl}\">Verify Account</a></p>";
            await _emailService.SendEmailAsync(user.Email!, "Verify your RoboRent account", html);
        }

        var tokenString = await GenerateJwtForUserInternalAsync(user, account, fullName);

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            var separator = returnUrl.Contains("?") ? "&" : "?";
            var redirectWithToken = $"{returnUrl}{separator}token={Uri.EscapeDataString(tokenString)}";
            return new AuthResultDto { RedirectUrl = redirectWithToken, Token = tokenString };
        }

        return new AuthResultDto { Token = tokenString };
    }

    public async Task<string?> GenerateJwtForUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        var account = await _accountService.GetAccountByUserIdAsync(user.Id);
        return await GenerateJwtForUserInternalAsync(user, account, user.UserName);
    }

    private async Task<string> GenerateJwtForUserInternalAsync(ModifyIdentityUser user, Account? account, string? name)
    {
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
            new Claim(JwtRegisteredClaimNames.Name, name ?? user.UserName ?? string.Empty)
        };

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

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}


