namespace RoboRent_BE.Model.DTOs.Chat;

public class ChatRoomResponse
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public StaffInfoDto Staff { get; set; } = null!;
    public CustomerInfoDto Customer { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<ChatMessageResponse> Messages { get; set; } = new();
}

public class StaffInfoDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }  // Identity UserId for SignalR targeting
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

public class CustomerInfoDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }  // Identity UserId for SignalR targeting
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}
