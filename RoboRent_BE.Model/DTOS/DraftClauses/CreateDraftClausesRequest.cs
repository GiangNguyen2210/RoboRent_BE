using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.DraftClauses;

public class CreateDraftClausesRequest
{
    public string? Title { get; set; } = string.Empty;

    public string? Body { get; set; } = string.Empty;

    // NOTE: IsModified is automatically set to false on creation and should not be set by users

    [Required]
    public int? ContractDraftsId { get; set; }

    public int? TemplateClausesId { get; set; }
}
