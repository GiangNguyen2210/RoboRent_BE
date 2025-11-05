using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class EventSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public DateTime Date { get; set; } =  DateTime.UtcNow;
    
    public string? Description { get; set; } = string.Empty;
    
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly EndTime { get; set; }
    
    public string? EventAddress { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } =  false;
    
    public int? RentalId { get; set; }
    
    [ForeignKey("RentalId")] public virtual Rental Rental { get; set; } = null!;
}