namespace RoboRent_BE.Model.DTOS.TemplateClauses;

public class TemplateClausesResponse
{
    public int Id { get; set; }

    public string? ClauseCode { get; set; } = string.Empty;

    public string? Title { get; set; } = string.Empty;

    public string? Body { get; set; } = string.Empty;

    public bool? IsMandatory { get; set; } = false;

    public bool? IsEditable { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public int? ContractTemplatesId { get; set; }

    // Navigation properties
    public string? ContractTemplateTitle { get; set; }
}
