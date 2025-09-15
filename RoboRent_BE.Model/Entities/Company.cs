using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class Company
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string? CompanyName { get; set; } = string.Empty;
    
    public string? TIN { get; set; } = string.Empty;
    
    public string? TypeOfEnterprise { get; set; } = string.Empty;
    
    public string? HeadOfficeAddress { get; set; } = string.Empty;
    
    public string? LegalRepresentativeName { get; set; } = string.Empty;
    
    public string? LegalRepresentativePosition { get; set; } = string.Empty;
    
    public DateTime? DateOfEstablishment { get; set; } = DateTime.UtcNow;
    
    public string? OperationalStatus { get; set; } = string.Empty;
    
    public string? BusinessLines { get; set; } = string.Empty;
    
    public bool? isDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
}