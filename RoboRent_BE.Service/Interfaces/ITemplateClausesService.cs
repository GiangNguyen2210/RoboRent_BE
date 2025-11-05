using RoboRent_BE.Model.DTOS.TemplateClauses;
using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Service.Interfaces;

public interface ITemplateClausesService
{
    Task<IEnumerable<TemplateClausesResponse>> GetAllTemplateClausesAsync();
    Task<TemplateClausesResponse?> GetTemplateClausesByIdAsync(int id);
    Task<IEnumerable<TemplateClausesResponse>> GetTemplateClausesByContractTemplateIdAsync(int contractTemplateId);
    Task<IEnumerable<TemplateClausesResponse>> GetTemplateClausesByIsMandatoryAsync(bool isMandatory);
    Task<IEnumerable<TemplateClausesResponse>> GetTemplateClausesByIsEditableAsync(bool isEditable);
    Task<IEnumerable<TemplateClausesResponse>> GetAvailableTemplateClausesForDraftAsync(int contractTemplateId, int contractDraftId);
    Task<TemplateClausesResponse> CreateTemplateClausesAsync(CreateTemplateClausesRequest request);
    Task<TemplateClausesResponse?> UpdateTemplateClausesAsync(UpdateTemplateClausesRequest request);
    Task<bool> DeleteTemplateClausesAsync(int id);
}

