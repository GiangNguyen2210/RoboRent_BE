namespace RoboRent_BE.Model.DTOS.ContractTemplates;

public class ContractTemplatesResponse
{
    public int Id { get; set; }

    public string? TemplateCode { get; set; } = string.Empty;

    public string? Title { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public string? BodyJson { get; set; } = string.Empty;

    public string? Status { get; set; } = string.Empty;

    public string? Version { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    // Navigation properties
    public string? CreatedByName { get; set; }
    public string? UpdatedByName { get; set; }
}

