using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.TemplateClauses;

public class CreateTemplateClauseRequest
{
    [Required]
    public int ContractTemplateId { get; set; }

    [Required]
    public string TitleOrCode { get; set; } = string.Empty;

    public string? Body { get; set; } = string.Empty;

    public bool? IsMandatory { get; set; } = false;

    public bool? IsEditable { get; set; } = false;
}



