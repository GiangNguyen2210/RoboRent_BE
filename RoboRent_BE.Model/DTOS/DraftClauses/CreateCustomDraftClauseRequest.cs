using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.DraftClauses;

public class CreateCustomDraftClauseRequest
{
    [Required]
    public string? Title { get; set; } = string.Empty;

    [Required]
    public string? Body { get; set; } = string.Empty;

    [Required]
    public int? ContractDraftsId { get; set; }

    // Template properties (used if saveAsTemplate = true)
    public string? ClauseCode { get; set; } = string.Empty;
    
    public bool? IsMandatory { get; set; } = false;
    
    public bool? IsEditable { get; set; } = true;

    // Flag to indicate whether to also create a template clause
    public bool SaveAsTemplate { get; set; } = false;

    // Required if SaveAsTemplate = true (to know which template to add clause to)
    public int? ContractTemplatesId { get; set; }
}



