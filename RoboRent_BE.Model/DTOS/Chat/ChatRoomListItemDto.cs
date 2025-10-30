namespace RoboRent_BE.Model.DTOs.Chat;

public class ChatRoomListItemDto
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    
    // For Staff view - show customer info
    public string? CustomerName { get; set; }
    
    // For Customer view - show staff info (optional)
    public string? StaffName { get; set; }
    
    // Rental info
    public string? PackageName { get; set; }
    public string? EventDate { get; set; }
    public string? Status { get; set; }
    
    // Last message info
    public string? LastMessage { get; set; }
    public DateTime? LastMessageTime { get; set; }
    
    // Unread count
    public int UnreadCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

public class ChatRoomListResponse
{
    public List<ChatRoomListItemDto> Rooms { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}