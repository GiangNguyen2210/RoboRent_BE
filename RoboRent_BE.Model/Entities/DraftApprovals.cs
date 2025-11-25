using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class DraftApprovals
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Comments { get; set; } = string.Empty;

    public string? Status { get; set; }  = string.Empty;

    public DateTime? RequestedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }  = DateTime.UtcNow;

    public int? ContractDraftsId { get; set; }
    
    [ForeignKey("ContractDraftsId")] public virtual ContractDrafts ContractDraft { get; set; } = null!;

    public int? RequestedBy { get; set; }
    
    [ForeignKey("RequestedBy")] public virtual Account Staff { get; set; } = null!;
    
    public int? ReviewerId { get; set; }
    
    [ForeignKey("ReviewerId")] public virtual Account Manager { get; set; } =  null!;
    
}