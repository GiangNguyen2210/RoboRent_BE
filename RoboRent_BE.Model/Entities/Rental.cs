using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class Rental
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    
    [Required] public int? AccountId { get; set; } // the rental

    public int? EventActivityId { get; set; }
    
    public int? ActivityTypeId  { get; set; }
    
    public int? StaffId { get; set; }
    
    public DateTime? PlannedDeliveryTime { get; set; }
    public DateTime? PlannedPickupTime { get; set; }
    
    [ForeignKey("StaffId")] public virtual Account Staff { get; set; } = null!;

    [ForeignKey("EventActivityId")] public virtual EventActivity EventActivity { get; set; } = null!;
    
    [ForeignKey("AccountId")] public virtual Account Account { get; set; } = null!;
    
    [ForeignKey("ActivityTypeId")] public virtual ActivityType ActivityType { get; set; } = null!;

}