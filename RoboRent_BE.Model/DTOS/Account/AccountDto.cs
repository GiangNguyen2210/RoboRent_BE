namespace RoboRent_BE.Model.DTOS.Account;

public class AccountDto
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    // Thêm fields khác nếu cần (e.g., Status cho subscription check)
}