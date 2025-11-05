using RoboRent_BE.Model.Entities;

namespace RoboRent_BE.Repository.Interfaces;

public interface ITemplateClausesRepository : IGenericRepository<TemplateClauses>
{
    Task<TemplateClauses?> GetByIdAsync(int id);
    Task<IEnumerable<TemplateClauses>> GetTemplateClausesByContractTemplateIdAsync(int contractTemplateId);
    Task<IEnumerable<TemplateClauses>> GetTemplateClausesByIsMandatoryAsync(bool isMandatory);
    Task<IEnumerable<TemplateClauses>> GetTemplateClausesByIsEditableAsync(bool isEditable);
    Task<IEnumerable<TemplateClauses>> GetAllWithIncludesAsync();
    Task<IEnumerable<TemplateClauses>> GetAvailableTemplateClausesForDraftAsync(int contractTemplateId, int contractDraftId);
}

