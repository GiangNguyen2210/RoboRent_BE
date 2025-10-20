namespace RoboRent_BE.Model.DTOS.RentalOrder;

public class OrderResonse
{
    public int Id { get; set; }

    public string? EventName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; } = string.Empty;
    
    public string? Email { get; set; } = string.Empty;
    
    public DateTime? CreatedDate { get; set; } =  DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; } =  DateTime.UtcNow;
    
    public bool? IsDeleted { get; set; } =  false;
    
    public string? Status { get; set; } = string.Empty;
    
    public int? AccountId { get; set; }
    
    public int? EventId { get; set; }
    
    public int? RentalPackageId  { get; set; }
}