using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.TemplateClauses;

public class UpdateTemplateClausesRequest
{
    // Id is not needed in body - it comes from route parameter
    // ContractTemplatesId is not updatable - it's preserved from existing clause

    public string? ClauseCode { get; set; } = string.Empty;

    [Required]
    public string? Title { get; set; } = string.Empty;

    [Required]
    public string? Body { get; set; } = string.Empty;

    public bool? IsMandatory { get; set; } = false;

    public bool? IsEditable { get; set; } = false;
}

