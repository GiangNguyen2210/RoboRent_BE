namespace RoboRent_BE.Model.DTOS.RentalOrder;

public class OrderResponse
{
    public int Id { get; set; }

    public string? EventName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; } = string.Empty;
    
    public string? Email { get; set; } = string.Empty;
    
    public string? Description { get; set; } = string.Empty;
    
    public string? Address { get; set; } = string.Empty;
    
    public string? City { get; set; } = string.Empty;
    
    public TimeOnly? StartTime { get; set; }
    
    public TimeOnly? EndTime { get; set; }
    
    public DateTime? CreatedDate { get; set; } =  DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; } =  DateTime.UtcNow;
    
    public DateTime? RequestedDate { get; set; } =  DateTime.UtcNow;
    
    public DateTime? ReceivedDate { get; set; } =  DateTime.UtcNow;
    
    public DateTime? EventDate { get; set; } =  DateTime.UtcNow;
    
    public bool? IsDeleted { get; set; } =  false;
    
    public string? Status { get; set; } = string.Empty;
    
    public int? AccountId { get; set; }

    public int? EventActivityId { get; set; }
    
    public int? ActivityTypeId  { get; set; }
    
    public int? StaffId { get; set; }
    
    public string? EventActivityName { get; set; } = string.Empty;
    
    public string? ActivityTypeName { get; set; } = string.Empty;
}