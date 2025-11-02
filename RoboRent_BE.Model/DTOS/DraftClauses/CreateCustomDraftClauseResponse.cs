namespace RoboRent_BE.Model.DTOS.DraftClauses;

public class CreateCustomDraftClauseResponse
{
    public DraftClausesResponse DraftClause { get; set; } = null!;
    
    // Will be populated if SaveAsTemplate = true
    public int? CreatedTemplateClauseId { get; set; }
    
    public bool WasSavedAsTemplate { get; set; }
    
    public string Message { get; set; } = string.Empty;
}

