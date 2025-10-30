using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class Rental
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? CompanyName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; } = string.Empty;
    
    public string? EventAddress { get; set; } = string.Empty;
    
    public string? Email { get; set; } = string.Empty;
    
    public DateTime? CreatedDate { get; set; } =  DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; } =  DateTime.UtcNow;
    
    public bool? IsDeleted { get; set; } =  false;
    
    public string? Status { get; set; } = string.Empty;
    
    [Required] public int? AccountId { get; set; }
    
    public int? EventId { get; set; }
    
    public int? RentalPackageId  { get; set; }
    
    [ForeignKey("EventId")] public virtual Event Event { get; set; } = null!;
    
    [ForeignKey("AccountId")] public virtual Account Account { get; set; } = null!;
    
    [ForeignKey("RentalPackageId")] public virtual RentalPackage RentalPackage { get; set; } = null!;
    
    public virtual ICollection<EventSchedule> EventSchedules { get; set; } = new List<EventSchedule>();


}