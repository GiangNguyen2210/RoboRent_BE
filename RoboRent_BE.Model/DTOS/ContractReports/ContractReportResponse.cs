namespace RoboRent_BE.Model.DTOS.ContractReports;

public class ContractReportResponse
{
    public int Id { get; set; }
    
    public int? DraftClausesId { get; set; }
    public string? DraftClauseTitle { get; set; }
    
    public int? ReporterId { get; set; }
    public string? ReporterName { get; set; }
    public string? ReportRole { get; set; }
    
    public int? AccusedId { get; set; }
    public string? AccusedName { get; set; }
    
    public string? Description { get; set; }
    public string? EvidencePath { get; set; }
    
    public string? Status { get; set; }
    public string? Resolution { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public int? ReviewedBy { get; set; }
    public string? ReviewerName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    
    public int? PaymentId { get; set; }
    public string? PaymentLink { get; set; }
}


