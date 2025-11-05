using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class DraftClauses
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Title { get; set; } = string.Empty;

    public string? Body { get; set; }  = string.Empty;

    public bool? IsModified { get; set; } =  false;

    public DateTime? CreatedAt { get; set; } =  DateTime.UtcNow;

    public int? ContractDraftsId { get; set; }
    
    [ForeignKey("ContractDraftsId")] public virtual ContractDrafts ContractDraft { get; set; } = null!;

    public int? TemplateClausesId { get; set; }
    
    [ForeignKey("TemplateClausesId")] public virtual TemplateClauses TemplateClause { get; set; } = null!;
}