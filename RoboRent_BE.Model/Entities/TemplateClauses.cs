using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class TemplateClauses
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? ClauseCode { get; set; }  = string.Empty;

    public string? Title { get; set; }  = string.Empty;

    public string? Body { get; set; }   = string.Empty;

    public bool? IsMandatory  { get; set; }   = false;

    public bool? IsEditable  { get; set; } =  false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? ContractTemplatesId { get; set; }
    
    [ForeignKey("ContractTemplatesId")] public virtual ContractTemplates ContractTemplate { get; set; } =  null!;
    
}