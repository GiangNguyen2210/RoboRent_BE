namespace RoboRent_BE.Model.DTOS.Admin;

public class AccountListResponse
{
    public int AccountId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Status { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? Role { get; set; }
}