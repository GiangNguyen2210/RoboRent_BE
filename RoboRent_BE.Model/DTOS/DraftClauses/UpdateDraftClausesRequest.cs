using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.DraftClauses;

public class UpdateDraftClausesRequest
{
    [Required]
    public int Id { get; set; }

    public string? Title { get; set; } = string.Empty;

    public string? Body { get; set; } = string.Empty;

    public bool? IsModified { get; set; } = false;

    [Required]
    public int? ContractDraftsId { get; set; }

    public int? TemplateClausesId { get; set; }
}
