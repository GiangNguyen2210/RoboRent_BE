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
    Func<string, string?, string?> buildVerifyUrl)
    {
        ExternalLoginInfo? info;
        try
        {
            info = await _signInManager.GetExternalLoginInfoAsync();
        }
        catch (Exception ex)
        {
            await _signInManager.SignOutAsync();
            return new AuthResultDto { Error = $"Authentication error: {ex.Message}. Please try logging in again." };
        }

        if (info == null)
        {
            await _signInManager.SignOutAsync();
            return new AuthResultDto { Error = "External login info not found. The authentication session may have expired. Please try logging in again." };
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: true);

        ModifyIdentityUser user;
        if (!signInResult.Succeeded)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);
            var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var familyName = info.Principal.FindFirstValue(ClaimTypes.Surname);
            var picture = info.Principal.FindFirstValue("picture");

            if (string.IsNullOrEmpty(email))
            {
                return new AuthResultDto { Error = "Email is required from Google." };
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
                    return new AuthResultDto { Error = string.Join("; ", createResult.Errors.Select(e => e.Description)) };
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, "Customer");
                if (!addRoleResult.Succeeded)
                {
                    return new AuthResultDto { Error = string.Join("; ", addRoleResult.Errors.Select(e => e.Description)) };
                }
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                    return new AuthResultDto { Error = string.Join("; ", addLoginResult.Errors.Select(e => e.Description)) };
                }
            }
            else
            {
                var existingLogin = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (existingLogin == null)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        return new AuthResultDto { Error = string.Join("; ", addLoginResult.Errors.Select(e => e.Description)) };
                    }
                }
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles == null || userRoles.Count == 0)
                {
                    var addRoleResult = await _userManager.AddToRoleAsync(user, "Customer");
                    if (!addRoleResult.Succeeded)
                    {
                        return new AuthResultDto { Error = string.Join("; ", addRoleResult.Errors.Select(e => e.Description)) };
                    }
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: true);
        }
        else
        {
            user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey)!;
        }

        var fullNameFinal = info.Principal.FindFirstValue(ClaimTypes.Name);
        var givenNameFinal = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        var familyNameFinal = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var pictureFinal = info.Principal.FindFirstValue("picture");

        var account = await _accountService.CreatePendingAccountAsync(user.Id, fullNameFinal);

        if (account.Status == "PendingVerification")
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verifyUrl = buildVerifyUrl(user.Id, token);
            var html = GenerateVerificationEmailHtml(fullNameFinal, verifyUrl);
            await _emailService.SendEmailAsync(user.Email!, "Verify your RoboRent account", html);
        }

        var tokenString = await GenerateJwtForUserInternalAsync(user, account, fullNameFinal, givenNameFinal, familyNameFinal, pictureFinal);

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

    private async Task<string> GenerateJwtForUserInternalAsync(
        ModifyIdentityUser user,
        Account? account,
        string? name,
        string? givenName = null,
        string? familyName = null,
        string? picture = null)
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

        if (!string.IsNullOrEmpty(givenName))
            claims.Add(new Claim(ClaimTypes.GivenName, givenName));

        if (!string.IsNullOrEmpty(familyName))
            claims.Add(new Claim(ClaimTypes.Surname, familyName));

        if (!string.IsNullOrEmpty(picture))
            claims.Add(new Claim("picture", picture));

        if (account != null)
        {
            claims.Add(new Claim("accountId", account.Id.ToString()));
            claims.Add(new Claim("accountStatus", account.Status ?? string.Empty));
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
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

    private string GenerateVerificationEmailHtml(string? fullName, string verifyUrl)
    {
        var displayName = !string.IsNullOrEmpty(fullName) ? fullName : "there";
        
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Verify Your RoboRent Account</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Helvetica Neue', Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background-color: #f5f5f5;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""600"" style=""max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%); padding: 40px 40px 30px; text-align: center; border-radius: 8px 8px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 32px; font-weight: bold; letter-spacing: 2px; font-family: 'Orbitron', 'Helvetica Neue', Arial, sans-serif;"">
                                ROBORENT
                            </h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 40px 30px;"">
                            <h2 style=""margin: 0 0 20px 0; color: #1e293b; font-size: 24px; font-weight: 600;"">
                                Welcome to RoboRent!
                            </h2>
                            
                            <p style=""margin: 0 0 20px 0; color: #475569; font-size: 16px; line-height: 1.6;"">
                                Hi {displayName},
                            </p>
                            
                            <p style=""margin: 0 0 20px 0; color: #475569; font-size: 16px; line-height: 1.6;"">
                                Thank you for joining RoboRent! We're excited to have you on board. To get started and access all our features, please verify your email address by clicking the button below.
                            </p>
                            
                            <!-- Verify Button -->
                            <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""margin: 30px 0;"">
                                <tr>
                                    <td align=""center"" style=""padding: 0;"">
                                        <a href=""{verifyUrl}"" style=""display: inline-block; padding: 14px 32px; background-color: #2563eb; color: #ffffff; text-decoration: none; border-radius: 6px; font-size: 16px; font-weight: 600; transition: background-color 0.3s;"">
                                            Verify Your Account
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style=""margin: 20px 0 0 0; color: #64748b; font-size: 14px; line-height: 1.6;"">
                                If the button doesn't work, you can copy and paste this link into your browser:
                            </p>
                            <p style=""margin: 10px 0 0 0; color: #2563eb; font-size: 14px; word-break: break-all; line-height: 1.6;"">
                                <a href=""{verifyUrl}"" style=""color: #2563eb; text-decoration: underline;"">{verifyUrl}</a>
                            </p>
                            
                            <p style=""margin: 30px 0 0 0; color: #64748b; font-size: 14px; line-height: 1.6;"">
                                This verification link will expire in 24 hours for security reasons.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px 40px; background-color: #f8fafc; border-top: 1px solid #e2e8f0; border-radius: 0 0 8px 8px;"">
                            <p style=""margin: 0 0 10px 0; color: #64748b; font-size: 14px; line-height: 1.6; text-align: center;"">
                                If you didn't create an account with RoboRent, you can safely ignore this email.
                            </p>
                            <p style=""margin: 20px 0 0 0; color: #94a3b8; font-size: 12px; line-height: 1.6; text-align: center;"">
                                © {DateTime.Now.Year} RoboRent. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}