using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class ContractReports
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? DraftClausesId { get; set; }
    
    [ForeignKey("DraftClausesId")] public virtual DraftClauses DraftClauses { get; set; } = null!;

    public int? ReporterId { get; set; }
    
    [ForeignKey("ReporterId")] public virtual Account Account { get; set; } =  null!;

    public string? ReportRole { get; set; } =  string.Empty;

    public int? AccusedId  { get; set; }
    
    [ForeignKey("AccusedId")] public virtual Account Accused { get; set; } = null!;

    public string? ReportCategory { get; set; } =  string.Empty;

    public string? Description { get; set; } =   string.Empty;

    public string? EvidencePath { get; set; } =   string.Empty;

    public string? Status { get; set; } =  string.Empty;

    public string? Resolution { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public int? ReviewedBy { get; set; }
    
    [ForeignKey("ReviewedBy")] public virtual Account Manager { get; set; } =  null!;

    public DateTime? ReviewedAt { get; set; } =  DateTime.UtcNow;

    public string? ResolutionType { get; set; } =  string.Empty;

    public int? PaymentId { get; set; }
    
    [ForeignKey("PaymentId")] 
    public virtual PaymentRecord PaymentRecord { get; set; } = null!;
}