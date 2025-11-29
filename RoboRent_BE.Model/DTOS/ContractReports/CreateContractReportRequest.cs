using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.ContractReports;

public class CreateContractReportRequest
{
    [Required]
    public int? DraftClausesId { get; set; }
    
    [Required]
    public int AccusedId { get; set; }
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    public string? EvidencePath { get; set; } = string.Empty;
}


