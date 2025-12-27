namespace RoboRent_BE.Model.DTOs.RoboType;

public class RoboTypeLiteResponse
{
    public int Id { get; set; }
    public string? TypeName { get; set; } = string.Empty;

    // optional nếu bạn có Name riêng
    public string? Name { get; set; } = string.Empty;
}
