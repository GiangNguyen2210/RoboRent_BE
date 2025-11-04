using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.ContractTemplates;

public class CreateTemplateWithParsedClausesRequest
{
    public string? TemplateCode { get; set; } = string.Empty;

    [Required]
    public string? Title { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public string? BodyJson { get; set; } = string.Empty;

    public string? Version { get; set; } = string.Empty;
}


