using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.DTOS.ContractDrafts;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interface;
using RoboRent_BE.Service.Interfaces;

namespace RoboRent_BE.Service.Services;

public class ContractDraftsService : IContractDraftsService
{
    private readonly IContractDraftsRepository _contractDraftsRepository;
    private readonly ITemplateClausesRepository _templateClausesRepository;
    private readonly IDraftClausesRepository _draftClausesRepository;
    private readonly IContractTemplatesRepository _contractTemplatesRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;

    public ContractDraftsService(
        IContractDraftsRepository contractDraftsRepository, 
        ITemplateClausesRepository templateClausesRepository,
        IDraftClausesRepository draftClausesRepository,
        IContractTemplatesRepository contractTemplatesRepository,
        IRentalRepository rentalRepository,
        IPaymentService paymentService,
        IMapper mapper)
    {
        _contractDraftsRepository = contractDraftsRepository;
        _templateClausesRepository = templateClausesRepository;
        _draftClausesRepository = draftClausesRepository;
        _contractTemplatesRepository = contractTemplatesRepository;
        _rentalRepository = rentalRepository;
        _paymentService = paymentService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ContractDraftsResponse>> GetAllContractDraftsAsync()
    {
        var contractDrafts = await _contractDraftsRepository.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<ContractDraftsResponse>>(contractDrafts);
    }

    public async Task<ContractDraftsResponse?> GetContractDraftsByIdAsync(int id)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(cd => cd.Id == id, "ContractTemplate,Rental,Staff,Manager");
        if (contractDraft == null)
            return null;

        return _mapper.Map<ContractDraftsResponse>(contractDraft);
    }

    public async Task<IEnumerable<ContractDraftsResponse>> GetContractDraftsByRentalIdAsync(int rentalId)
    {
        var contractDrafts = await _contractDraftsRepository.GetContractDraftsByRentalIdAsync(rentalId);
        return _mapper.Map<IEnumerable<ContractDraftsResponse>>(contractDrafts);
    }

    public async Task<IEnumerable<ContractDraftsResponse>> GetContractDraftsByStaffIdAsync(int staffId)
    {
        var contractDrafts = await _contractDraftsRepository.GetContractDraftsByStaffIdAsync(staffId);
        return _mapper.Map<IEnumerable<ContractDraftsResponse>>(contractDrafts);
    }

    public async Task<IEnumerable<ContractDraftsResponse>> GetContractDraftsByManagerIdAsync(int managerId)
    {
        var contractDrafts = await _contractDraftsRepository.GetContractDraftsByManagerIdAsync(managerId);
        return _mapper.Map<IEnumerable<ContractDraftsResponse>>(contractDrafts);
    }

    public async Task<IEnumerable<ContractDraftsResponse>> GetContractDraftsByStatusAsync(string status)
    {
        var contractDrafts = await _contractDraftsRepository.GetContractDraftsByStatusAsync(status);
        return _mapper.Map<IEnumerable<ContractDraftsResponse>>(contractDrafts);
    }

    public async Task<ContractDraftsResponse> CreateContractDraftsAsync(CreateContractDraftsRequest request, int staffId)
    {
        // Validate: Check if there's already a contract draft for this rentalId with status other than "Rejected" or "Draft"
        if (request.RentalId.HasValue)
        {
            var existingContractDrafts = await _contractDraftsRepository.GetContractDraftsByRentalIdAsync(request.RentalId.Value);
            var hasActiveContractDraft = existingContractDrafts.Any(cd => 
            {
                if (string.IsNullOrEmpty(cd.Status))
                    return false; // Treat null/empty as draft-like, allow creation
                
                var status = cd.Status.Trim();
                return !status.Equals("RejectedByCustomer", StringComparison.OrdinalIgnoreCase) &&
                !status.Equals("RejectedByManager", StringComparison.OrdinalIgnoreCase);


            });
            
            if (hasActiveContractDraft)
            {
                throw new InvalidOperationException(
                    "Cannot create a new contract draft. There is already an active or draft contract draft for this rental.");
            }
        }
        
        // Create the contract draft
        var contractDraft = _mapper.Map<ContractDrafts>(request);
        
        // Find Rental
        var rental = await _rentalRepository.GetAsync(r => r.Id == request.RentalId);
        rental.Status = "PendingContract";
        await _rentalRepository.UpdateAsync(rental);
        
        // Set StaffId from token (the person who creates this contract draft)
        contractDraft.StaffId = staffId;
       
        contractDraft.Status = "Draft";
        
        // Set CreatedAt to current time
        contractDraft.CreatedAt = DateTime.UtcNow;
        
        // Set UpdatedAt to null for now
        contractDraft.UpdatedAt = null;
        
        // If contract draft is created from a template, get data from template
        if (request.ContractTemplatesId.HasValue && request.ContractTemplatesId.Value > 0)
        {
            // Get contract template
            var contractTemplate = await _contractTemplatesRepository.GetAsync(
                ct => ct.Id == request.ContractTemplatesId.Value);
            
            if (contractTemplate != null)
            {
                // Only set manager from template if ManagerId was not provided in request
                // Priority: request.ManagerId > template.CreatedBy
                if (!request.ManagerId.HasValue || request.ManagerId.Value == 0)
                {
                    contractDraft.ManagerId = contractTemplate.CreatedBy;
                }
                else
                {
                    // Use ManagerId from request
                    contractDraft.ManagerId = request.ManagerId;
                }
                
                // Get the whole body JSON from contract template (always use template's BodyJson when template is provided)
                contractDraft.BodyJson = contractTemplate.BodyJson;
            }
        }
        
        var createdContractDraft = await _contractDraftsRepository.AddAsync(contractDraft);
        
        // If contract draft is created from a template, automatically copy all template clauses
        if (request.ContractTemplatesId.HasValue && request.ContractTemplatesId.Value > 0)
        {
            // Get all template clauses from the contract template
            var allTemplateClauses = await _templateClausesRepository
                .GetTemplateClausesByContractTemplateIdAsync(request.ContractTemplatesId.Value);
            
            // Create draft clauses for each template clause
            foreach (var templateClause in allTemplateClauses)
            {
                var draftClause = new DraftClauses
                {
                    Title = templateClause.Title,
                    Body = templateClause.Body,
                    IsModified = false, 
                    CreatedAt = DateTime.UtcNow, 
                    ContractDraftsId = createdContractDraft.Id,
                    TemplateClausesId = templateClause.Id
                };
                
                await _draftClausesRepository.AddAsync(draftClause);
            }
        }
        
        return _mapper.Map<ContractDraftsResponse>(createdContractDraft);
    }

    public async Task<ContractDraftsResponse?> UpdateContractDraftsAsync(UpdateContractDraftsRequest request)
    {
        var existingContractDraft = await _contractDraftsRepository.GetAsync(cd => cd.Id == request.Id, "ContractTemplate,Rental,Staff,Manager");
        if (existingContractDraft == null)
            return null;

        // Validate that no non-editable clauses have been modified
        await ValidateDraftClausesEditableAsync(request.Id);

        // Only allow update of Title, BodyJson, and Comments
        // Update Title if provided (allows empty string to clear the field)
        if (request.Title != null)
        {
            existingContractDraft.Title = request.Title;
        }
        
        // Update BodyJson if provided (allows empty string to clear the field)
        if (request.BodyJson != null)
        {
            existingContractDraft.BodyJson = request.BodyJson;
        }
        
        // Update Comments if provided (allows empty string to clear the field)
        if (request.Comments != null)
        {
            existingContractDraft.Comments = request.Comments;
        }
        
        // Set status to "Modified" when updating
        existingContractDraft.Status = "Modified";
        
        // Set UpdatedAt to current time when updating
        existingContractDraft.UpdatedAt = DateTime.UtcNow;
        
        var updatedContractDraft = await _contractDraftsRepository.UpdateAsync(existingContractDraft);
        
        return _mapper.Map<ContractDraftsResponse>(updatedContractDraft);
    }

    private async Task ValidateDraftClausesEditableAsync(int contractDraftId)
    {
        // Get all draft clauses for this contract draft with their template clauses
        var draftClauses = await _draftClausesRepository.GetDraftClausesByContractDraftIdAsync(contractDraftId);
        
        foreach (var draftClause in draftClauses)
        {
            // Only validate clauses that are linked to template clauses
            if (draftClause.TemplateClause != null)
            {
                var templateClause = draftClause.TemplateClause;
                
                // Check if the template clause is not editable
                if (templateClause.IsEditable == false)
                {
                    // Check if the draft clause content differs from the template clause
                    bool contentChanged = draftClause.Title != templateClause.Title || 
                                         draftClause.Body != templateClause.Body;
                    
                    if (contentChanged)
                    {
                        throw new InvalidOperationException(
                            $"Cannot update contract draft. Clause '{templateClause.Title}' has been modified but is not editable according to the template clause.");
                    }
                }
            }
        }
    }

    public async Task<bool> DeleteContractDraftsAsync(int id)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(cd => cd.Id == id);
        if (contractDraft == null)
            return false;

        await _contractDraftsRepository.DeleteAsync(contractDraft);
        return true;
    }

    private string AddSignatureToContract(string bodyJson, string signature, string side)
    {
        if (string.IsNullOrEmpty(bodyJson))
            return bodyJson;

        var signatureSectionId = "contract-signatures-section";
        var hasSignatureSection = bodyJson.Contains(signatureSectionId);

        if (hasSignatureSection)
        {
            // Update existing signature section
            if (side == "left")
            {
                // Add/update manager signature (left side)
                var managerSignatureDiv = $@"<div style=""flex: 1; text-align: left; padding-right: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Manager Signature:</strong>
        </div>
        <div style=""font-family: 'Brush Script MT', cursive; font-size: 30px; min-height: 60px; border-bottom: 1px solid #000; padding-bottom: 5px;"">
            {signature}
        </div>
        <div style=""margin-top: 10px; font-size: 12px;"">
            {DateTime.UtcNow.ToString("MM/dd/yyyy")}
        </div>
    </div>";

                // Check if manager signature already exists
                if (bodyJson.Contains("Manager Signature:"))
                {
                    // Replace existing manager signature using regex
                    var pattern = @"<div style=""flex: 1; text-align: left[^>]*>.*?Manager Signature:.*?</div>\s*</div>\s*</div>";
                    bodyJson = System.Text.RegularExpressions.Regex.Replace(bodyJson, pattern, managerSignatureDiv, System.Text.RegularExpressions.RegexOptions.Singleline);
                }
                else
                {
                    // Insert manager signature before customer signature
                    bodyJson = bodyJson.Replace(
                        @"<div style=""flex: 1; text-align: right",
                        managerSignatureDiv + @"<div style=""flex: 1; text-align: right");
                }
            }
            else
            {
                // Add/update customer signature (right side)
                var customerSignatureDiv = $@"<div style=""flex: 1; text-align: right; padding-left: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Customer Signature:</strong>
        </div>
        <div style=""font-family: 'Brush Script MT', cursive; font-size: 24px; min-height: 60px; border-bottom: 1px solid #000; padding-bottom: 5px;"">
            {signature}
        </div>
        <div style=""margin-top: 10px; font-size: 12px;"">
            {DateTime.UtcNow.ToString("MM/dd/yyyy")}
        </div>
    </div>";

                // Check if customer signature already exists
                if (bodyJson.Contains("Customer Signature:"))
                {
                    // Replace existing customer signature using regex
                    var pattern = @"<div style=""flex: 1; text-align: right[^>]*>.*?Customer Signature:.*?</div>\s*</div>\s*</div>";
                    bodyJson = System.Text.RegularExpressions.Regex.Replace(bodyJson, pattern, customerSignatureDiv, System.Text.RegularExpressions.RegexOptions.Singleline);
                }
                else
                {
                    // Insert customer signature after manager signature
                    bodyJson = bodyJson.Replace(
                        @"</div>\s*</div>\s*<!-- End Signature Section -->",
                        customerSignatureDiv + @"</div></div><!-- End Signature Section -->");
                }
            }

            return bodyJson;
        }
        else
        {
            // Create new signature section with both sides
            string signatureHtml;
            if (side == "left")
            {
                // Manager signs first
                signatureHtml = $@"
<div id=""{signatureSectionId}"" style=""display: flex; justify-content: space-between; margin-top: 50px; padding-top: 20px; border-top: 2px solid #000;"">
    <div style=""flex: 1; text-align: left; padding-right: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Manager Signature:</strong>
        </div>
        <div style=""font-family: 'Brush Script MT', cursive; font-size: 24px; min-height: 60px; border-bottom: 1px solid #000; padding-bottom: 5px;"">
            {signature}
        </div>
        <div style=""margin-top: 10px; font-size: 12px;"">
            {DateTime.UtcNow.ToString("MM/dd/yyyy")}
        </div>
    </div>
    <div style=""flex: 1; text-align: right; padding-left: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Customer Signature:</strong>
        </div>
        <div style=""font-family: 'Brush Script MT', cursive; font-size: 24px; min-height: 60px; border-bottom: 1px solid #000; padding-bottom: 5px;"">
            &nbsp;
        </div>
        <div style=""margin-top: 10px; font-size: 12px;"">
            &nbsp;
        </div>
    </div>
</div>
<!-- End Signature Section -->";
            }
            else
            {
                // Customer signs first (unlikely but handle it)
                signatureHtml = $@"
<div id=""{signatureSectionId}"" style=""display: flex; justify-content: space-between; margin-top: 50px; padding-top: 20px; border-top: 2px solid #000;"">
    <div style=""flex: 1; text-align: left; padding-right: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Manager Signature:</strong>
        </div>
        <div style=""font-family: 'Brush Script MT', cursive; font-size: 24px; min-height: 60px; border-bottom: 1px solid #000; padding-bottom: 5px;"">
            &nbsp;
        </div>
        <div style=""margin-top: 10px; font-size: 12px;"">
            &nbsp;
        </div>
    </div>
    <div style=""flex: 1; text-align: right; padding-left: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Customer Signature:</strong>
        </div>
        <div style=""font-family: 'Brush Script MT', cursive; font-size: 24px; min-height: 60px; border-bottom: 1px solid #000; padding-bottom: 5px;"">
            {signature}
        </div>
        <div style=""margin-top: 10px; font-size: 12px;"">
            {DateTime.UtcNow.ToString("MM/dd/yyyy")}
        </div>
    </div>
</div>
<!-- End Signature Section -->";
            }

            // Insert before closing tags
            if (bodyJson.Contains("</body>"))
            {
                return bodyJson.Replace("</body>", signatureHtml + "</body>");
            }
            else if (bodyJson.Contains("</html>"))
            {
                return bodyJson.Replace("</html>", signatureHtml + "</html>");
            }
            else
            {
                return bodyJson + signatureHtml;
            }
        }
    }

    private string RemoveManagerSignatureFromContract(string bodyJson)
    {
        if (string.IsNullOrEmpty(bodyJson))
            return bodyJson;

        // Check if manager signature exists
        if (!bodyJson.Contains("Manager Signature:"))
            return bodyJson;

        // Replace manager signature with empty placeholder
        var emptyManagerSignatureDiv = @"<div style=""flex: 1; text-align: left; padding-right: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Manager Signature:</strong>
        </div>
        <div style=""font-family: 'Brush Script MT', cursive; font-size: 24px; min-height: 60px; border-bottom: 1px solid #000; padding-bottom: 5px;"">
            &nbsp;
        </div>
        <div style=""margin-top: 10px; font-size: 12px;"">
            &nbsp;
        </div>
    </div>";

        // Remove existing manager signature using regex
        var pattern = @"<div style=""flex: 1; text-align: left[^>]*>.*?Manager Signature:.*?</div>\s*</div>\s*</div>";
        bodyJson = System.Text.RegularExpressions.Regex.Replace(bodyJson, pattern, emptyManagerSignatureDiv, System.Text.RegularExpressions.RegexOptions.Singleline);

        return bodyJson;
    }

    /// <summary>
    /// Extract a clause header/body section from a contract BodyJson by template clause id.
    /// The BodyJson is expected to contain headers like:
    /// &lt;p&gt;&lt;strong data-clause-id="{templateClauseId}"&gt;Điều X. ...&lt;/strong&gt;&lt;/p&gt; followed by the clause body.
    /// </summary>
    /// <param name="bodyJson">Full HTML body of the contract draft.</param>
    /// <param name="templateClauseId">ID of the template clause referenced by data-clause-id.</param>
    /// <returns>Tuple of (headerHtml, bodyHtml) or null if not found.</returns>
    private (string headerHtml, string bodyHtml)? GetClauseSectionByTemplateClauseId(string? bodyJson, int templateClauseId)
    {
        if (string.IsNullOrWhiteSpace(bodyJson))
            return null;

        var clauseIdPattern = $@"data-clause-id=[""']?{templateClauseId}[""']?";
        var clauseIdMatch = System.Text.RegularExpressions.Regex.Match(
            bodyJson,
            clauseIdPattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (!clauseIdMatch.Success)
            return null;

        // Find the start of the surrounding <p> for the header
        var beforeClauseId = bodyJson[..clauseIdMatch.Index];
        var paragraphStart = beforeClauseId.LastIndexOf("<p>", StringComparison.OrdinalIgnoreCase);
        if (paragraphStart == -1)
        {
            paragraphStart = beforeClauseId.LastIndexOf("<p ", StringComparison.OrdinalIgnoreCase);
        }

        if (paragraphStart == -1)
            return null;

        int clauseStartIndex = paragraphStart;

        // Find the end of the header: </strong></p>
        var headerEndPattern = @"</strong>\s*</p>";
        var headerEndMatch = System.Text.RegularExpressions.Regex.Match(
            bodyJson[clauseStartIndex..],
            headerEndPattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

        if (!headerEndMatch.Success)
            return null;

        var headerEndIndex = clauseStartIndex + headerEndMatch.Index + headerEndMatch.Length;

        // Find end of the clause body (next clause header or end of document)
        var nextClausePattern = @"<p>\s*<strong[^>]*>\s*Điều";
        var nextClauseMatch = System.Text.RegularExpressions.Regex.Match(
            bodyJson[headerEndIndex..],
            nextClausePattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        int clauseEndIndex;
        if (nextClauseMatch.Success)
        {
            clauseEndIndex = headerEndIndex + nextClauseMatch.Index;
        }
        else
        {
            var remainingText = bodyJson[headerEndIndex..];
            var closingDivIndex = remainingText.LastIndexOf("</div>", StringComparison.OrdinalIgnoreCase);
            clauseEndIndex = closingDivIndex > 0
                ? headerEndIndex + closingDivIndex
                : bodyJson.Length;
        }

        var headerHtml = bodyJson.Substring(clauseStartIndex, headerEndIndex - clauseStartIndex);
        var bodyHtml = bodyJson.Substring(headerEndIndex, clauseEndIndex - headerEndIndex);

        return (headerHtml, bodyHtml);
    }

    /// <summary>
    /// Normalize HTML for comparison (ignore trivial whitespace differences).
    /// </summary>
    private string NormalizeHtmlForComparison(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var withoutNewLines = input.Replace("\r", string.Empty).Replace("\n", string.Empty);
        return System.Text.RegularExpressions.Regex.Replace(withoutNewLines, @"\s+", " ").Trim();
    }

    /// <summary>
    /// Extract plain text title from a clause header HTML (&lt;p&gt;&lt;strong&gt;...&lt;/strong&gt;&lt;/p&gt;).
    /// </summary>
    private string? ExtractTitleFromHeaderHtml(string headerHtml)
    {
        if (string.IsNullOrWhiteSpace(headerHtml))
            return null;

        var match = System.Text.RegularExpressions.Regex.Match(
            headerHtml,
            @"<strong[^>]*>(.*?)</strong>",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);

        if (!match.Success)
            return null;

        var inner = match.Groups[1].Value;
        // Remove any remaining HTML tags inside the strong
        inner = System.Text.RegularExpressions.Regex.Replace(inner, "<.*?>", string.Empty);
        return inner.Trim();
    }

    /// <summary>
    /// Validate edits inside BodyJson for clauses linked to template clauses and
    /// update the corresponding draft clauses when the template clause is editable.
    /// Staff can freely edit content outside of these clause sections.
    /// </summary>
    private async Task ValidateAndApplyBodyJsonClauseEditsAsync(ContractDrafts contractDraft, string newBodyJson)
    {
        if (contractDraft == null)
            throw new ArgumentNullException(nameof(contractDraft));

        if (string.IsNullOrWhiteSpace(contractDraft.BodyJson))
        {
            // No existing body to compare against – nothing to validate at clause level
            return;
        }

        var draftClauses = await _draftClausesRepository.GetDraftClausesByContractDraftIdAsync(contractDraft.Id);

        foreach (var draftClause in draftClauses)
        {
            // Only clauses that are linked to a template clause participate in this validation
            if (draftClause.TemplateClausesId == null && draftClause.TemplateClause == null)
                continue;

            var templateClauseId = draftClause.TemplateClausesId ?? draftClause.TemplateClause!.Id;

            var oldSection = GetClauseSectionByTemplateClauseId(contractDraft.BodyJson, templateClauseId);
            var newSection = GetClauseSectionByTemplateClauseId(newBodyJson, templateClauseId);

            // If we cannot find the clause in the original body, skip – nothing to compare
            if (oldSection == null)
                continue;

            // Treat removal of a clause in BodyJson as a change to that clause
            var oldCombined = NormalizeHtmlForComparison(oldSection.Value.headerHtml + oldSection.Value.bodyHtml);
            var newCombined = NormalizeHtmlForComparison(
                (newSection?.headerHtml ?? string.Empty) + (newSection?.bodyHtml ?? string.Empty));

            if (oldCombined == newCombined)
            {
                // No change for this clause section
                continue;
            }

            // If clause is linked to a template clause, enforce IsEditable
            var templateClause = draftClause.TemplateClause;
            var isEditable = templateClause?.IsEditable ?? true;

            if (!isEditable)
            {
                var clauseTitle = templateClause?.Title ?? draftClause.Title ?? $"Clause {templateClauseId}";
                throw new InvalidOperationException(
                    $"Cannot update contract draft. Clause '{clauseTitle}' has been modified in the contract body but is not editable according to the template clause.");
            }

            // At this point the clause is editable – update the draft clause content to reflect the new BodyJson
            if (newSection != null)
            {
                var newTitle = ExtractTitleFromHeaderHtml(newSection.Value.headerHtml) ?? draftClause.Title;
                draftClause.Title = newTitle;
                draftClause.Body = newSection.Value.bodyHtml?.Trim();
            }
            else
            {
                // Clause section removed from BodyJson – mark as modified and clear body
                draftClause.Body = string.Empty;
            }

            draftClause.IsModified = true;
            await _draftClausesRepository.UpdateAsync(draftClause);
        }
    }

    public async Task<ContractDraftsResponse?> ManagerSignContractAsync(int id, ManagerSignatureRequest request, int managerId)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == id, 
            "ContractTemplate,Rental,Staff,Manager");
        
        if (contractDraft == null)
            return null;

        // Validate status and manager
        if (contractDraft.Status != "PendingManagerSignature")
            throw new InvalidOperationException("Contract is not in PendingManagerSignature status");

        if (contractDraft.ManagerId != managerId)
            throw new UnauthorizedAccessException("You are not authorized to sign this contract");

        // Add manager signature to contract (left side)
        contractDraft.BodyJson = AddSignatureToContract(contractDraft.BodyJson ?? "", request.Signature, "left");

        // Update status to PendingCustomerSignature
        contractDraft.Status = "PendingCustomerSignature";
        contractDraft.UpdatedAt = DateTime.UtcNow;

        var updatedContractDraft = await _contractDraftsRepository.UpdateAsync(contractDraft);
        return _mapper.Map<ContractDraftsResponse>(updatedContractDraft);
    }

    public async Task<ContractDraftsResponse?> CustomerSignContractAsync(int id, CustomerSignatureRequest request, int customerId)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == id, 
            "ContractTemplate,Rental,Staff,Manager");
        
        if (contractDraft == null)
            return null;

        // Validate status and customer
        if (contractDraft.Status != "PendingCustomerSignature")
            throw new InvalidOperationException("Contract is not in PendingCustomerSignature status");

        // Get rental to check customer
        var dbContext = _contractDraftsRepository.GetDbContext();
        var rental = await dbContext.Rentals
            .FirstOrDefaultAsync(r => r.Id == contractDraft.RentalId);
        
        if (rental == null || rental.AccountId != customerId)
            throw new UnauthorizedAccessException("You are not authorized to sign this contract");

        // Add customer signature to contract (right side)
        contractDraft.BodyJson = AddSignatureToContract(contractDraft.BodyJson ?? "", request.Signature, "right");

        // Update status to Active
        contractDraft.Status = "Active";
        contractDraft.UpdatedAt = DateTime.UtcNow;

        var updatedContractDraft = await _contractDraftsRepository.UpdateAsync(contractDraft);
        rental.Status = "PendingDeposit";
        await _rentalRepository.UpdateAsync(rental);
        var paymentResult = await _paymentService.CreateDepositPaymentAsync(rental.Id);
        var response = _mapper.Map<ContractDraftsResponse>(updatedContractDraft);
        response.DepositPayment = new DepositPaymentInfo
        {
            OrderCode = paymentResult.OrderCode,
            Amount = paymentResult.Amount,
            CheckoutUrl = paymentResult.CheckoutUrl,
            ExpiresAt = paymentResult.ExpiredAt ?? DateTime.UtcNow.AddDays(7)
        };
    
        return response;
    }

    public async Task<ContractDraftsResponse?> ManagerRejectContractAsync(int id, ManagerRejectRequest request, int managerId)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == id, 
            "ContractTemplate,Rental,Staff,Manager");
        
        if (contractDraft == null)
            return null;

        // Validate status and manager
        if (contractDraft.Status != "PendingManagerSignature")
            throw new InvalidOperationException("Contract is not in PendingManagerSignature status");

        if (contractDraft.ManagerId != managerId)
            throw new UnauthorizedAccessException("You are not authorized to reject this contract");

        // Update status to RejectedByManager
        contractDraft.Status = "RejectedByManager";
        if (!string.IsNullOrEmpty(request.Reason))
        {
            contractDraft.Comments = string.IsNullOrEmpty(contractDraft.Comments) 
                ? $"Rejected by Manager: {request.Reason}" 
                : $"{contractDraft.Comments}\nRejected by Manager: {request.Reason}";
        }
        contractDraft.UpdatedAt = DateTime.UtcNow;

        var updatedContractDraft = await _contractDraftsRepository.UpdateAsync(contractDraft);
        return _mapper.Map<ContractDraftsResponse>(updatedContractDraft);
    }

    public async Task<ContractDraftsResponse?> CustomerRejectContractAsync(int id, CustomerRejectRequest request, int customerId)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == id, 
            "ContractTemplate,Rental,Staff,Manager");
        
        if (contractDraft == null)
            return null;

        // Validate status and customer
        if (contractDraft.Status != "PendingCustomerSignature")
            throw new InvalidOperationException("Contract is not in PendingCustomerSignature status");

        // Get rental to check customer
        var dbContext = _contractDraftsRepository.GetDbContext();
        var rental = await dbContext.Rentals
            .FirstOrDefaultAsync(r => r.Id == contractDraft.RentalId);
        
        if (rental == null || rental.AccountId != customerId)
            throw new UnauthorizedAccessException("You are not authorized to reject this contract");

        // Update status to RejectedByCustomer
        contractDraft.Status = "RejectedByCustomer";
        if (!string.IsNullOrEmpty(request.Reason))
        {
            contractDraft.Comments = string.IsNullOrEmpty(contractDraft.Comments) 
                ? $"Rejected by Customer: {request.Reason}" 
                : $"{contractDraft.Comments}\nRejected by Customer: {request.Reason}";
        }
        contractDraft.UpdatedAt = DateTime.UtcNow;

        var updatedContractDraft = await _contractDraftsRepository.UpdateAsync(contractDraft);
        return _mapper.Map<ContractDraftsResponse>(updatedContractDraft);
    }

    public async Task<ContractDraftsResponse?> CustomerRequestChangeAsync(int id, CustomerRequestChangeRequest request, int customerId)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == id, 
            "ContractTemplate,Rental,Staff,Manager");
        
        if (contractDraft == null)
            return null;

        // Validate status and customer
        if (contractDraft.Status != "PendingCustomerSignature")
            throw new InvalidOperationException("Contract is not in PendingCustomerSignature status");

        // Get rental to check customer
        var dbContext = _contractDraftsRepository.GetDbContext();
        var rental = await dbContext.Rentals
            .FirstOrDefaultAsync(r => r.Id == contractDraft.RentalId);
        
        if (rental == null || rental.AccountId != customerId)
            throw new UnauthorizedAccessException("You are not authorized to request changes to this contract");

        // Remove manager signature when customer requests change
        // This ensures that if manager had signed before, the signature is removed for the revision
        if (!string.IsNullOrEmpty(contractDraft.BodyJson))
        {
            contractDraft.BodyJson = RemoveManagerSignatureFromContract(contractDraft.BodyJson);
        }

        // Update status to ChangeRequested
        contractDraft.Status = "ChangeRequested";
        if (!string.IsNullOrEmpty(request.Comment))
        {
            contractDraft.Comments = string.IsNullOrEmpty(contractDraft.Comments) 
                ? $"Change Requested by Customer: {request.Comment}" 
                : $"{contractDraft.Comments}\nChange Requested by Customer: {request.Comment}";
        }
        contractDraft.UpdatedAt = DateTime.UtcNow;

        var updatedContractDraft = await _contractDraftsRepository.UpdateAsync(contractDraft);
        rental.Status = "PendingContract";
        await _rentalRepository.UpdateAsync(rental);
        return _mapper.Map<ContractDraftsResponse>(updatedContractDraft);
    }

    public async Task<IEnumerable<ContractDraftsResponse>> GetPendingManagerSignatureContractsAsync(int managerId)
    {
        var contractDrafts = await _contractDraftsRepository.GetContractDraftsByManagerIdAsync(managerId);
        var pendingContracts = contractDrafts
            .Where(cd => cd.Status == "PendingManagerSignature")
            .ToList();
        return _mapper.Map<IEnumerable<ContractDraftsResponse>>(pendingContracts);
    }

    public async Task<IEnumerable<ContractDraftsResponse>> GetPendingCustomerSignatureContractsAsync(int customerId)
    {
        var contractDrafts = await _contractDraftsRepository.GetContractDraftsByCustomerIdAsync(customerId);
        var pendingContracts = contractDrafts
            .Where(cd => cd.Status == "PendingCustomerSignature")
            .ToList();
        return _mapper.Map<IEnumerable<ContractDraftsResponse>>(pendingContracts);
    }

    public async Task<IEnumerable<ContractDraftsResponse>> GetChangeRequestedContractsAsync()
    {
        var contractDrafts = await _contractDraftsRepository.GetContractDraftsByStatusAsync("ChangeRequested");
        return _mapper.Map<IEnumerable<ContractDraftsResponse>>(contractDrafts);
    }

    public async Task<ContractDraftsResponse?> SendToManagerAsync(int id, int staffId)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == id, 
            "ContractTemplate,Rental,Staff,Manager");
        
        if (contractDraft == null)
            return null;

        // Validate status and staff
        if (contractDraft.Status != "Modified" && contractDraft.Status != "Draft")
            throw new InvalidOperationException("Contract must be in 'Draft' or 'Modified' status to send to manager");

        if (contractDraft.StaffId != staffId)
            throw new UnauthorizedAccessException("You are not authorized to send this contract to manager");

        // Update status to PendingManagerSignature
        contractDraft.Status = "PendingManagerSignature";
        contractDraft.UpdatedAt = DateTime.UtcNow;

        var updatedContractDraft = await _contractDraftsRepository.UpdateAsync(contractDraft);
        return _mapper.Map<ContractDraftsResponse>(updatedContractDraft);
    }

    public async Task<ContractDraftsResponse?> ReviseContractAsync(int id, UpdateContractDraftsRequest request, int staffId)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == id, 
            "ContractTemplate,Rental,Staff,Manager");
        
        if (contractDraft == null)
            return null;

        // Validate status and staff
        if (contractDraft.Status != "ChangeRequested" && contractDraft.Status !="Draft")
            throw new InvalidOperationException("Contract must be in 'ChangeRequested' or 'Draft' status to revise");

        if (contractDraft.StaffId != staffId)
            throw new UnauthorizedAccessException("You are not authorized to revise this contract");

        // If BodyJson is being updated, validate edits against template clause permissions
        // and propagate allowed changes into draft clauses.
        if (request.BodyJson != null)
        {
            await ValidateAndApplyBodyJsonClauseEditsAsync(contractDraft, request.BodyJson);
        }

        // Also keep the existing safety net that ensures non-editable clauses in DraftClauses
        // haven't been changed directly against their templates.
        await ValidateDraftClausesEditableAsync(id);

        // Update contract fields
        if (request.Title != null)
        {
            contractDraft.Title = request.Title;
        }
        
        if (request.BodyJson != null)
        {
            contractDraft.BodyJson = request.BodyJson;
        }
        
        if (request.Comments != null)
        {
            contractDraft.Comments = request.Comments;
        }

        // Update status back to Modified
        contractDraft.Status = "Modified";
        contractDraft.UpdatedAt = DateTime.UtcNow;

        var updatedContractDraft = await _contractDraftsRepository.UpdateAsync(contractDraft);
        return _mapper.Map<ContractDraftsResponse>(updatedContractDraft);
    }
}

