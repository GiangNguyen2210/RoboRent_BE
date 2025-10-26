using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.TemplateClauses;

public class UpdateTemplateClausesRequest
{
    [Required]
    public int Id { get; set; }

    public string? ClauseCode { get; set; } = string.Empty;

    [Required]
    public string? Title { get; set; } = string.Empty;

    [Required]
    public string? Body { get; set; } = string.Empty;

    public bool? IsMandatory { get; set; } = false;

    public bool? IsEditable { get; set; } = false;

    [Required]
    public int? ContractTemplatesId { get; set; }
}
