namespace RoboRent_BE.Service.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> HandleGoogleCallbackAsync(string? returnUrl, Func<string, string, string?> buildVerifyUrl);
    Task<string?> GenerateJwtForUserAsync(string userId);
}

public class AuthResultDto
{
    public string? Token { get; set; }
    public string? RedirectUrl { get; set; }
    public string? Error { get; set; }
}


