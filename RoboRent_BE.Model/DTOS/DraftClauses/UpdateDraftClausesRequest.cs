using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.DraftClauses;

public class UpdateDraftClausesRequest
{
    [Required]
    public int Id { get; set; }

    public string? Title { get; set; } = string.Empty;

    public string? Body { get; set; } = string.Empty;

    // NOTE: IsModified is automatically calculated by the service and should not be set by users

    [Required]
    public int? ContractDraftsId { get; set; }

    public int? TemplateClausesId { get; set; }
}
