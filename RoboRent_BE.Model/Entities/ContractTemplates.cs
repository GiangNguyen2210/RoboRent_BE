using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class ContractTemplates
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string? TemplateCode { get; set; } = string.Empty;

    public string? Title { get; set; } =  string.Empty;

    public string? Description { get; set; } =  string.Empty;

    public string? BodyJson { get; set; }  =  string.Empty;

    public string? Status { get; set; } =   string.Empty;

    public string? Version { get; set; } =   string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? CreatedBy { get; set; }
    
    [ForeignKey("CreatedBy")] public virtual Account Created { get; set; } = null!;

    public DateTime UpdatedAt { get; set; } =  DateTime.UtcNow;

    public int? UpdatedBy  { get; set; }
    
    [ForeignKey("UpdatedBy")] public virtual Account Updated { get; set; }
}