using RoboRent_BE.Model.Enums;

namespace RoboRent_BE.Model.DTOs.Chat;

// Response cho 1 message
public class ChatMessageResponse
{
    public int Id { get; set; }
    public int ChatRoomId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderRole { get; set; } = string.Empty; // "Staff" hoặc "Customer"
    public MessageType MessageType { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string>? VideoUrls { get; set; } // Parsed từ MediaUrls JSON
    public int? RelatedEntityId { get; set; }
    public string? Status { get; set; } // Cho demo: Pending/Accepted/Rejected
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ChatMessagesPageResponse
{
    public List<ChatMessageResponse> Messages { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}