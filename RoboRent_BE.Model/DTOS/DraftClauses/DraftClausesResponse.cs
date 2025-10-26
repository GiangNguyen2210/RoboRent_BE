namespace RoboRent_BE.Model.DTOS.DraftClauses;

public class DraftClausesResponse
{
    public int Id { get; set; }

    public string? Title { get; set; } = string.Empty;

    public string? Body { get; set; } = string.Empty;

    public bool? IsModified { get; set; } = false;

    public DateTime? CreatedAt { get; set; }

    public int? ContractDraftsId { get; set; }

    public int? TemplateClausesId { get; set; }

    // Navigation properties
    public string? ContractDraftTitle { get; set; }
    public string? TemplateClauseTitle { get; set; }
}
