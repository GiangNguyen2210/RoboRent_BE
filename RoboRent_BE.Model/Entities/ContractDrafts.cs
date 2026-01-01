using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class ContractDrafts
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Title { get; set; } = string.Empty;

    public string? BodyJson { get; set; }  = string.Empty;

    public string? Comments { get; set; }  = string.Empty;

    public string? Status { get; set; }  = string.Empty;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }  = DateTime.UtcNow;

    public string? OriginalBodyJson { get; set; } = string.Empty; // Store original contract before customer signs

    public int? ContractTemplatesId { get; set; }
    
    [ForeignKey("ContractTemplatesId")] public virtual ContractTemplates ContractTemplate { get; set; } =  null!;

    public int? RentalId { get; set; }
    
    [ForeignKey("RentalId")] public virtual Rental Rental { get; set; } = null!;

    public int? StaffId { get; set; }
    
    [ForeignKey("StaffId")] public virtual Account Staff { get; set; } = null!;

    public int? ManagerId { get; set; }
    
    [ForeignKey("ManagerId")] public virtual Account Manager { get; set; } = null!;
}