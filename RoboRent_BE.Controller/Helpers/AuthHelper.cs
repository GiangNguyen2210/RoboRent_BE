using System.Security.Claims;

namespace RoboRent_BE.Controller.Helpers;

public static class AuthHelper
{
    /// <summary>
    /// Lấy accountId từ JWT token claims
    /// </summary>
    public static int GetCurrentUserId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst("accountId")?.Value;
        if (string.IsNullOrEmpty(claim))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return int.Parse(claim);
    }
    
    /// <summary>
    /// Lấy role từ JWT token claims
    /// </summary>
    public static string GetCurrentUserRole(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(claim))
        {
            throw new UnauthorizedAccessException("User role not found");
        }
        return claim;
    }
}