using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RoboRent_BE.Model.DTOS.ContractDrafts;
using RoboRent_BE.Model.DTOS.RentalDetail;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interface;
using RoboRent_BE.Service.Interfaces;
using System.Net;
using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using Docnet.Core;
using Docnet.Core.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.XObjects;

namespace RoboRent_BE.Service.Services;

public class ContractDraftsService : IContractDraftsService
{
    private readonly IContractDraftsRepository _contractDraftsRepository;
    private readonly ITemplateClausesRepository _templateClausesRepository;
    private readonly IDraftClausesRepository _draftClausesRepository;
    private readonly IContractTemplatesRepository _contractTemplatesRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalDetailRepository _rentalDetailRepository;
    private readonly IGroupScheduleRepository _groupScheduleRepository;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;

    public ContractDraftsService(
        IContractDraftsRepository contractDraftsRepository, 
        ITemplateClausesRepository templateClausesRepository,
        IDraftClausesRepository draftClausesRepository,
        IContractTemplatesRepository contractTemplatesRepository,
        IRentalRepository rentalRepository,
        IRentalDetailRepository rentalDetailRepository,
        IGroupScheduleRepository groupScheduleRepository,
        IPaymentService paymentService,
        IEmailService emailService,
        IMemoryCache memoryCache,
        IMapper mapper)
    {
        _contractDraftsRepository = contractDraftsRepository;
        _templateClausesRepository = templateClausesRepository;
        _draftClausesRepository = draftClausesRepository;
        _contractTemplatesRepository = contractTemplatesRepository;
        _rentalRepository = rentalRepository;
        _rentalDetailRepository = rentalDetailRepository;
        _groupScheduleRepository = groupScheduleRepository;
        _paymentService = paymentService;
        _emailService = emailService;
        _memoryCache = memoryCache;
        _mapper = mapper;
        
        // Set QuestPDF license (free for non-commercial use, or use your license key)
        QuestPDF.Settings.License = LicenseType.Community;
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
                return !status.Equals("Rejected", StringComparison.OrdinalIgnoreCase) &&
                !status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase);


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
        
        // Populate the table with rental details if rental ID is provided and body JSON exists
        // The method will find any table containing "STT" header, regardless of which "Điều" it's in
        if (request.RentalId.HasValue && !string.IsNullOrWhiteSpace(contractDraft.BodyJson))
        {
            contractDraft.BodyJson = await PopulateTableWithRentalDetailsAsync(contractDraft.BodyJson, request.RentalId.Value);
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

        // Prevent updates if contract draft status is "Active"
        if (existingContractDraft.Status == "Active"|| existingContractDraft.Status == "Cancelled" || existingContractDraft.Status == "Rejected")
        {
            throw new InvalidOperationException("Cannot update contract draft.");
        }

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

        // Signature box styles - rectangle box for clear signing area
        var signatureBoxStyle = @"border: 2px solid #000; min-height: 60px; padding: 10px; display: flex; align-items: center; justify-content: center;";
        var signedBoxStyle = @"font-family: 'Brush Script MT', cursive; font-size: 28px; border: 2px solid #000; min-height: 60px; padding: 10px; display: flex; align-items: center; justify-content: center;";
        var emptyBoxPlaceholder = @"<span style=""color: #999; font-size: 14px; font-style: italic;""></span>";

        if (hasSignatureSection)
        {
            // Update existing signature section
            if (side == "left")
            {
                // Add/update manager signature (left side) with rectangle box
                var signatureBoxHtml = $@"<div id=""manager-signature-box"" style=""{signedBoxStyle}"">
            {signature}
        </div>";
                
                var dateValue = $@"<div style=""margin-top: 10px; font-size: 12px;"">
            {DateTime.UtcNow.ToString("MM/dd/yyyy")}
        </div>";

                // Check if manager signature section exists
                if (bodyJson.Contains("Manager Signature:"))
                {
                    // Replace the manager signature box using regex
                    var pattern = @"<div id=""manager-signature-box""[^>]*>.*?</div>";
                    if (Regex.IsMatch(bodyJson, pattern, RegexOptions.Singleline))
                    {
                        bodyJson = Regex.Replace(bodyJson, pattern, signatureBoxHtml, RegexOptions.Singleline);
                    }
                    else
                    {
                        // Fallback: Find and replace the old structure
                        var managerSigIndex = bodyJson.IndexOf("Manager Signature:", StringComparison.OrdinalIgnoreCase);
                        if (managerSigIndex >= 0)
                        {
                            var afterStrongClose = bodyJson.IndexOf("</strong>", managerSigIndex);
                            if (afterStrongClose >= 0)
                            {
                                var afterFirstDiv = bodyJson.IndexOf("</div>", afterStrongClose);
                                if (afterFirstDiv >= 0)
                                {
                                    var signatureDivStart = afterFirstDiv + 6;
                                    var signatureDivEnd = bodyJson.IndexOf("</div>", signatureDivStart);
                                    if (signatureDivEnd >= 0)
                                    {
                                        var dateDivStart = signatureDivEnd + 6;
                                        var dateDivEnd = bodyJson.IndexOf("</div>", dateDivStart);
                                        if (dateDivEnd >= 0)
                                        {
                                            var beforeSignature = bodyJson.Substring(0, signatureDivStart);
                                            var afterDate = bodyJson.Substring(dateDivEnd + 6);
                                            bodyJson = beforeSignature + "\n        " + signatureBoxHtml + "\n        " + dateValue + afterDate;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var managerSignatureDiv = $@"<div style=""flex: 1; text-align: left; padding-right: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Manager Signature:</strong>
        </div>
        {signatureBoxHtml}
        {dateValue}
    </div>";
                    
                    bodyJson = bodyJson.Replace(
                        @"<div style=""flex: 1; text-align: right",
                        managerSignatureDiv + @"<div style=""flex: 1; text-align: right");
                }
                
                // IMPORTANT: Ensure customer signature box exists when manager signs
                // This ensures both borders are always present
                if (!bodyJson.Contains("customer-signature-box"))
                {
                    // Add customer signature box if it doesn't exist
                    var customerBoxHtml = $@"<div id=""customer-signature-box"" style=""{signatureBoxStyle}"">
            {emptyBoxPlaceholder}
        </div>";
                    var customerSignatureDiv = $@"<div style=""flex: 1; text-align: right; padding-left: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Customer Signature:</strong>
        </div>
        {customerBoxHtml}
        <div style=""margin-top: 10px; font-size: 12px;"">
            &nbsp;
        </div>
    </div>";
                    
                    // Try to insert customer signature section before the closing div of signature section
                    var signatureSectionEnd = bodyJson.IndexOf("<!-- End Signature Section -->", StringComparison.OrdinalIgnoreCase);
                    if (signatureSectionEnd >= 0)
                    {
                        // Check if customer signature already exists in some form
                        if (!bodyJson.Contains("Customer Signature:"))
                        {
                            // Insert customer signature before the end comment
                            bodyJson = bodyJson.Insert(signatureSectionEnd, customerSignatureDiv);
                        }
                    }
                }
            }
            else
            {
                // Add/update customer signature (right side) with rectangle box
                var signatureBoxHtml = $@"<div id=""customer-signature-box"" style=""{signedBoxStyle}"">
            {signature}
        </div>";

                var customerSignatureDiv = $@"<div style=""flex: 1; text-align: right; padding-left: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Customer Signature:</strong>
        </div>
        {signatureBoxHtml}
        <div style=""margin-top: 10px; font-size: 12px;"">
            {DateTime.UtcNow.ToString("MM/dd/yyyy")}
        </div>
    </div>";

                // Check if customer signature already exists
                if (bodyJson.Contains("Customer Signature:"))
                {
                    // Replace existing customer signature using regex
                    var pattern = @"<div style=""flex: 1; text-align: right[^>]*>.*?Customer Signature:.*?</div>\s*</div>\s*</div>";
                    bodyJson = Regex.Replace(bodyJson, pattern, customerSignatureDiv, RegexOptions.Singleline);
                }
                else
                {
                    bodyJson = bodyJson.Replace(
                        @"</div>\s*</div>\s*<!-- End Signature Section -->",
                        customerSignatureDiv + @"</div></div><!-- End Signature Section -->");
                }
            }

            return bodyJson;
        }
        else
        {
            // Create new signature section with both sides - using rectangle boxes
            string signatureHtml;
            if (side == "left")
            {
                // Manager signs first - manager has signature, customer has empty box
                signatureHtml = $@"
<div id=""{signatureSectionId}"" style=""display: flex; justify-content: space-between; margin-top: 50px; padding-top: 20px; border-top: 2px solid #000;"">
    <div style=""flex: 1; text-align: left; padding-right: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Manager Signature:</strong>
        </div>
        <div id=""manager-signature-box"" style=""{signedBoxStyle}"">
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
        <div id=""customer-signature-box"" style=""{signatureBoxStyle}"">
            {emptyBoxPlaceholder}
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
        <div id=""manager-signature-box"" style=""{signatureBoxStyle}"">
            {emptyBoxPlaceholder}
        </div>
        <div style=""margin-top: 10px; font-size: 12px;"">
            &nbsp;
        </div>
    </div>
    <div style=""flex: 1; text-align: right; padding-left: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Customer Signature:</strong>
        </div>
        <div id=""customer-signature-box"" style=""{signedBoxStyle}"">
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
        // Use same font-size (30px) as AddSignatureToContract for consistency
        var emptyManagerSignatureDiv = @"<div style=""flex: 1; text-align: left; padding-right: 20px;"">
        <div style=""margin-bottom: 10px;"">
            <strong>Manager Signature:</strong>
        </div>
        <div style=""font-family: 'Brush Script MT', cursive; font-size: 30px; min-height: 60px; border-bottom: 1px solid #000; padding-bottom: 5px;"">
            &nbsp;
        </div>
        <div style=""margin-top: 10px; font-size: 12px;"">
            &nbsp;
        </div>
    </div>";

        // Improved regex pattern to match the nested div structure more reliably
        // Matches: opening div with style, then nested divs until we find "Manager Signature:", 
        // then matches all content until we close all 3 inner divs and the outer div
        var pattern = @"<div style=""flex: 1; text-align: left[^>]*>[\s\S]*?Manager Signature:[\s\S]*?</div>\s*</div>\s*</div>\s*</div>";
        bodyJson = System.Text.RegularExpressions.Regex.Replace(bodyJson, pattern, emptyManagerSignatureDiv, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

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
    /// Update BodyJson of contract draft when a draft clause is updated.
    /// This method finds the clause in BodyJson by template clause ID and updates its title and body.
    /// </summary>
    public async Task UpdateBodyJsonFromDraftClauseAsync(int contractDraftId, int draftClauseId, string newTitle, string newBody)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(cd => cd.Id == contractDraftId);
        if (contractDraft == null || string.IsNullOrWhiteSpace(contractDraft.BodyJson))
            return;

        var draftClause = await _draftClausesRepository.GetAsync(dc => dc.Id == draftClauseId, "TemplateClause");
        if (draftClause == null || draftClause.TemplateClausesId == null)
            return; // Only update BodyJson for clauses linked to template clauses

        var templateClauseId = draftClause.TemplateClausesId.Value;
        var bodyJson = contractDraft.BodyJson;

        // Find the clause section in BodyJson by template clause ID
        var clauseSection = GetClauseSectionByTemplateClauseId(bodyJson, templateClauseId);
        if (clauseSection == null)
            return; // Clause not found in BodyJson

        var (oldHeaderHtml, oldBodyHtml) = clauseSection.Value;

        // Build new header HTML with updated title
        // Extract the existing header structure but replace the title content
        var headerMatch = Regex.Match(oldHeaderHtml, @"(<p>\s*<strong[^>]*>)(.*?)(</strong>\s*</p>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (headerMatch.Success)
        {
            var newHeaderHtml = headerMatch.Groups[1].Value + newTitle + headerMatch.Groups[3].Value;
            
            // Find the start and end indices of the clause section
            var clauseIdPattern = $@"data-clause-id=[""']?{templateClauseId}[""']?";
            var clauseIdMatch = Regex.Match(bodyJson, clauseIdPattern, RegexOptions.IgnoreCase);
            if (clauseIdMatch.Success)
            {
                var beforeClauseId = bodyJson[..clauseIdMatch.Index];
                var paragraphStart = beforeClauseId.LastIndexOf("<p>", StringComparison.OrdinalIgnoreCase);
                if (paragraphStart == -1)
                {
                    paragraphStart = beforeClauseId.LastIndexOf("<p ", StringComparison.OrdinalIgnoreCase);
                }
                
                if (paragraphStart != -1)
                {
                    var clauseStartIndex = paragraphStart;
                    var headerEndPattern = @"</strong>\s*</p>";
                    var headerEndMatch = Regex.Match(
                        bodyJson[clauseStartIndex..],
                        headerEndPattern,
                        RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    
                    if (headerEndMatch.Success)
                    {
                        var headerEndIndex = clauseStartIndex + headerEndMatch.Index + headerEndMatch.Length;
                        
                        // Find end of the clause body
                        var nextClausePattern = @"<p>\s*<strong[^>]*>\s*Điều";
                        var nextClauseMatch = Regex.Match(
                            bodyJson[headerEndIndex..],
                            nextClausePattern,
                            RegexOptions.IgnoreCase);
                        
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
                        
                        // Replace the entire clause section (header + body)
                        var newBodyContent = !string.IsNullOrWhiteSpace(newBody) ? newBody : oldBodyHtml;
                        var updatedClauseSection = newHeaderHtml + newBodyContent;
                        
                        bodyJson = bodyJson.Substring(0, clauseStartIndex) + 
                                  updatedClauseSection + 
                                  bodyJson.Substring(clauseEndIndex);
                        
                        // Update the contract draft's BodyJson
                        contractDraft.BodyJson = bodyJson;
                        contractDraft.UpdatedAt = DateTime.UtcNow;
                        await _contractDraftsRepository.UpdateAsync(contractDraft);
                    }
                }
            }
        }
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
        // The AddSignatureToContract method handles both adding new and replacing existing signatures
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

    public async Task<ContractDraftsResponse?> ManagerCancelContractAsync(int id, ManagerCancelRequest request, int managerId)
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
            throw new UnauthorizedAccessException("You are not authorized to cancel this contract");

        // Update status to Cancelled
        contractDraft.Status = "Cancelled";
        if (!string.IsNullOrEmpty(request.Reason))
        {
            contractDraft.Comments = string.IsNullOrEmpty(contractDraft.Comments) 
                ? $"Cancelled by Manager: {request.Reason}" 
                : $"{contractDraft.Comments}\nCancelled by Manager: {request.Reason}";
        }
        contractDraft.UpdatedAt = DateTime.UtcNow;

        var updatedContractDraft = await _contractDraftsRepository.UpdateAsync(contractDraft);
        
        // Update rental status to Cancelled
        if (contractDraft.RentalId.HasValue)
        {
            var rental = await _rentalRepository.GetAsync(r => r.Id == contractDraft.RentalId.Value);
            if (rental != null)
            {
                rental.Status = "ForceCancelled";
                await _rentalRepository.UpdateAsync(rental);
            }
        }
        
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
        contractDraft.Status = "Rejected";
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

    /// <summary>
    /// Populates the table containing "STT" header with rental details.
    /// The method searches for any table with "STT" in its header row, regardless of which "Điều" section it's in.
    /// </summary>
    private async Task<string> PopulateTableWithRentalDetailsAsync(string bodyJson, int rentalId)
    {
        if (string.IsNullOrWhiteSpace(bodyJson))
            return bodyJson;

        try
        {
            // Get rental details with included RoboType and Rental
            var rentalDetails = await _rentalDetailRepository.GetRentalDetailsByRentalIdAsync(rentalId);
            if (rentalDetails == null || !rentalDetails.Any())
                return bodyJson;

            // Get GroupSchedule (EventSchedule) for start time and end time
            var groupSchedule = await _groupScheduleRepository.GetByRentalAsync(rentalId);

            // Group rental details by RoboTypeId and count quantity
            // Include all rental details even if some info is missing (as per user requirement)
            var groupedDetails = rentalDetails
                .GroupBy(rd => rd.RoboTypeId)
                .Select(g => new
                {
                    RoboTypeId = g.Key,
                    RobotTypeName = g.First().RoboType?.TypeName ?? string.Empty,
                    Quantity = g.Count()
                })
                .Where(g => g.RoboTypeId.HasValue) // Only filter out if RoboTypeId is null
                .ToList();

            if (!groupedDetails.Any())
                return bodyJson;

            // Format time as [HH:mm-HH:mm] from GroupSchedule (EventSchedule)
            string timeRange = string.Empty;
            if (groupSchedule != null && groupSchedule.StartTime.HasValue && groupSchedule.EndTime.HasValue)
            {
                var startTime = groupSchedule.StartTime.Value;
                var endTime = groupSchedule.EndTime.Value;
                timeRange = $"[{startTime.Hour:D2}:{startTime.Minute:D2}-{endTime.Hour:D2}:{endTime.Minute:D2}]";
            }

            // Build table rows HTML
            var tableRows = new System.Text.StringBuilder();
            int rowNumber = 1;
            
            foreach (var detail in groupedDetails)
            {
                // HTML encode the robot type name to prevent XSS and handle special characters
                var robotTypeName = WebUtility.HtmlEncode(detail.RobotTypeName ?? string.Empty);
                
                tableRows.AppendLine($@"			<tr>
				<td>
					<p>{rowNumber}</p>
				</td>
				<td>
					<p>{robotTypeName}</p>
				</td>
				<td>
					<p>{detail.Quantity}</p>
				</td>
				<td>
					<p>{timeRange}</p>
				</td>
				<td>
					<p>&nbsp;</p>
				</td>
			</tr>");
                rowNumber++;
            }

            // Find any table that contains "STT" in its header row (regardless of which "Điều" it's in)
            // This makes the solution flexible and works for tables in Điều 2, Điều 3, or any other section
            var tablePattern = @"<table[^>]*>.*?<tbody>.*?(<tr[^>]*>.*?STT.*?</tr>)";
            var tableMatch = Regex.Match(bodyJson, tablePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!tableMatch.Success)
                return bodyJson;

            // Find where the header row ends
            var headerRowEnd = tableMatch.Index + tableMatch.Length;
            
            // Find </tbody> to know where to stop replacing (skip all existing data rows)
            var afterHeaderRow = bodyJson.Substring(headerRowEnd);
            var tbodyEndPattern = @"</tbody>\s*";
            var tbodyEndMatch = Regex.Match(afterHeaderRow, tbodyEndPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!tbodyEndMatch.Success)
                return bodyJson;

            // Replace everything between header row end and </tbody> with new rows
            var replaceStart = headerRowEnd;
            var replaceEnd = headerRowEnd + tbodyEndMatch.Index;
            
            bodyJson = bodyJson.Substring(0, replaceStart) + 
                       tableRows.ToString() + 
                       bodyJson.Substring(replaceEnd);

            return bodyJson;
        }
        catch (Exception)
        {
            // If any error occurs, return original bodyJson
            return bodyJson;
        }
    }

    public async Task<bool> SendVerificationCodeAsync(int contractDraftId, int customerId)
    {
        // Validate contract draft and customer
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == contractDraftId,
            "ContractTemplate,Rental,Staff,Manager");
        
        if (contractDraft == null)
            throw new InvalidOperationException("Contract draft not found");

        // Validate status
        if (contractDraft.Status != "PendingCustomerSignature")
            throw new InvalidOperationException("Contract is not in PendingCustomerSignature status");

        // Get rental to check customer
        var dbContext = _contractDraftsRepository.GetDbContext();
        var rental = await dbContext.Rentals
            .Include(r => r.Account)
            .ThenInclude(a => a.ModifyIdentityUser)
            .FirstOrDefaultAsync(r => r.Id == contractDraft.RentalId);
        
        if (rental == null || rental.AccountId != customerId)
            throw new UnauthorizedAccessException("You are not authorized to request verification code for this contract");

        // Get customer email from user account
        var userEmail = rental.Account?.ModifyIdentityUser?.Email;
        if (string.IsNullOrEmpty(userEmail))
            throw new InvalidOperationException("Customer email not found");

        var customerName = rental.Account?.FullName ?? "Customer";

        // Generate 6-digit OTP (1 to 999999)
        var random = new Random();
        var otpCode = random.Next(1, 1000000).ToString("D6");

        // Store OTP in memory cache with 5 minute expiration
        var cacheKey = $"otp_{contractDraftId}_{customerId}";
        var otpData = new OtpData
        {
            Code = otpCode,
            ContractDraftId = contractDraftId,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        _memoryCache.Set(cacheKey, otpData, TimeSpan.FromMinutes(5));

        // Send email with OTP
        var emailHtml = GenerateOtpEmailHtml(customerName, otpCode);
        await _emailService.SendEmailAsync(userEmail, "Contract Signing Verification Code", emailHtml);

        return true;
    }

    public async Task<bool> VerifyCodeAsync(int contractDraftId, string code, int customerId)
    {
        // Validate contract draft and customer
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == contractDraftId,
            "ContractTemplate,Rental,Staff,Manager");
        
        if (contractDraft == null)
            throw new InvalidOperationException("Contract draft not found");

        // Validate status
        if (contractDraft.Status != "PendingCustomerSignature")
            throw new InvalidOperationException("Contract is not in PendingCustomerSignature status");

        // Get rental to check customer
        var dbContext = _contractDraftsRepository.GetDbContext();
        var rental = await dbContext.Rentals
            .FirstOrDefaultAsync(r => r.Id == contractDraft.RentalId);
        
        if (rental == null || rental.AccountId != customerId)
            throw new UnauthorizedAccessException("You are not authorized to verify code for this contract");

        // Check OTP from cache
        var cacheKey = $"otp_{contractDraftId}_{customerId}";
        if (!_memoryCache.TryGetValue(cacheKey, out OtpData? otpData) || otpData == null)
            throw new InvalidOperationException("Verification code not found or expired. Please request a new code.");

        // Validate code
        if (otpData.Code != code)
            throw new InvalidOperationException("Invalid verification code.");

        // Check if OTP is expired
        if (DateTime.UtcNow > otpData.ExpiresAt)
        {
            _memoryCache.Remove(cacheKey);
            throw new InvalidOperationException("Verification code has expired. Please request a new code.");
        }

        // Remove OTP from cache and store verification status (valid for 5 minutes after verification)
        _memoryCache.Remove(cacheKey);
        var verificationKey = $"verified_{contractDraftId}_{customerId}";
        var verificationData = new VerificationData
        {
            ContractDraftId = contractDraftId,
            CustomerId = customerId,
            VerifiedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5) // Must sign within 5 minutes
        };
        _memoryCache.Set(verificationKey, verificationData, TimeSpan.FromMinutes(5));

        return true;
    }

    private string GenerateOtpEmailHtml(string? customerName, string otpCode)
    {
        var displayName = !string.IsNullOrEmpty(customerName) ? customerName : "there";
        
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Contract Signing Verification Code</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Helvetica Neue', Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background-color: #f5f5f5;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""600"" style=""max-width: 600px; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%); padding: 40px 40px 30px; text-align: center; border-radius: 8px 8px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 32px; font-weight: bold; letter-spacing: 2px; font-family: 'Orbitron', 'Helvetica Neue', Arial, sans-serif;"">
                                ROBORENT
                            </h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 40px 30px;"">
                            <h2 style=""margin: 0 0 20px 0; color: #1e293b; font-size: 24px; font-weight: 600;"">
                                Contract Signing Verification
                            </h2>
                            
                            <p style=""margin: 0 0 20px 0; color: #475569; font-size: 16px; line-height: 1.6;"">
                                Hi {displayName},
                            </p>
                            
                            <p style=""margin: 0 0 20px 0; color: #475569; font-size: 16px; line-height: 1.6;"">
                                You are about to sign a contract. Please use the verification code below to complete the signing process.
                            </p>
                            
                            <!-- OTP Code Display -->
                            <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""margin: 30px 0;"">
                                <tr>
                                    <td align=""center"" style=""padding: 0;"">
                                        <div style=""display: inline-block; padding: 20px 40px; background-color: #f8fafc; border: 2px solid #2563eb; border-radius: 8px; text-align: center;"">
                                            <div style=""color: #2563eb; font-size: 36px; font-weight: bold; letter-spacing: 8px; font-family: 'Courier New', monospace;"">
                                                {otpCode}
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style=""margin: 20px 0 0 0; color: #64748b; font-size: 14px; line-height: 1.6;"">
                                Enter this code in the verification form to proceed with signing your contract.
                            </p>
                            
                            <p style=""margin: 20px 0 0 0; color: #64748b; font-size: 14px; line-height: 1.6;"">
                                This verification code will expire in 5 minutes for security reasons.
                            </p>
                            
                        
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px 40px; background-color: #f8fafc; border-top: 1px solid #e2e8f0; border-radius: 0 0 8px 8px;"">
                            <p style=""margin: 0 0 10px 0; color: #64748b; font-size: 14px; line-height: 1.6; text-align: center;"">
                                If you didn't request this verification code, please ignore this email or contact support if you have concerns.
                            </p>
                            <p style=""margin: 20px 0 0 0; color: #94a3b8; font-size: 12px; line-height: 1.6; text-align: center;"">
                                © {DateTime.Now.Year} RoboRent. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    // Helper classes for OTP storage
    private class OtpData
    {
        public string Code { get; set; } = string.Empty;
        public int ContractDraftId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    private class VerificationData
    {
        public int ContractDraftId { get; set; }
        public int CustomerId { get; set; }
        public DateTime VerifiedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// Represents extracted signature data from uploaded file
    /// </summary>
    private class ExtractedSignatureData
    {
        public SignatureType Type { get; set; }
        public string? TextSignature { get; set; }
        public string? FontFamily { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageBase64 { get; set; }
        public string? ImageMimeType { get; set; }
    }

    private enum SignatureType
    {
        None,
        Text,
        Image
    }

    /// <summary>
    /// Extracts signature data from uploaded PDF file - handles both text and image signatures
    /// Uses coordinate-based detection to find text in the customer signature box area
    /// </summary>
    private ExtractedSignatureData ExtractSignatureFromPdf(byte[] pdfBytes)
    {
        var result = new ExtractedSignatureData { Type = SignatureType.None };
        
        try
        {
            using var document = PdfDocument.Open(pdfBytes);
            
            // First, find the "Customer Signature" label to determine the signature box area
            var customerLabelPosition = FindCustomerSignatureLabelPosition(document);
            
            if (customerLabelPosition.HasValue)
            {
                var (labelX, labelY, labelPageIndex, pageWidth, pageHeight) = customerLabelPosition.Value;
                
                // Define customer signature box area (right side, below the label)
                // Customer signature box is typically on the right half of the page, near the bottom
                var boxLeftX = pageWidth * 0.45; // Slightly left of center to catch right-side text
                var boxRightX = pageWidth;
                var boxTopY = labelY + 50; // Area below the label (PDF Y increases downward)
                var boxBottomY = labelY + 200; // Allow space for the signature box
                
                // Extract text with coordinates from the relevant page
                var customerSignatureTexts = new List<(string Text, double Y)>();
                int currentPageIndex = 0;
                
                foreach (var page in document.GetPages())
                {
                    // Only process the page where we found the label (or last page if label not found)
                    if (customerLabelPosition.HasValue && currentPageIndex != labelPageIndex && labelPageIndex >= 0)
                    {
                        currentPageIndex++;
                        continue;
                    }
                    
                    // Get all letters with their positions
                    var letters = page.Letters.ToList();
                    
                    if (letters.Count == 0)
                    {
                        currentPageIndex++;
                        continue;
                    }
                    
                    // Group letters into words by proximity
                    var words = new List<(string Text, double X, double Y)>();
                    var currentWord = new List<Letter>();
                    
                    foreach (var letter in letters.OrderBy(l => l.StartBaseLine.Y).ThenBy(l => l.StartBaseLine.X))
                    {
                        var letterX = letter.StartBaseLine.X;
                        var letterY = letter.StartBaseLine.Y;
                        
                        // Check if this letter is in the customer signature box area
                        // Note: In PDF coordinates, Y=0 is at bottom, so we check if Y is between boxBottomY and boxTopY
                        bool inBoxArea = letterX >= boxLeftX && letterX <= boxRightX;
                        
                        // For Y coordinate: if label was found, use relative positioning; otherwise use bottom area
                        if (labelPageIndex >= 0)
                        {
                            // Use relative positioning from label
                            inBoxArea = inBoxArea && letterY >= boxTopY && letterY <= boxBottomY;
                        }
                        else
                        {
                            // Fallback: use bottom 30% of page, right half
                            var bottomAreaStart = pageHeight * 0.7;
                            inBoxArea = inBoxArea && letterY >= bottomAreaStart;
                        }
                        
                        if (inBoxArea)
                        {
                            // Check if this letter is part of the current word or a new word
                            if (currentWord.Count == 0 || 
                                Math.Abs(letterX - currentWord.Last().StartBaseLine.X) < letter.Width * 2.5)
                            {
                                currentWord.Add(letter);
                            }
                            else
                            {
                                // Finish current word and start new one
                                if (currentWord.Count > 0)
                                {
                                    var wordText = string.Join("", currentWord.Select(l => l.Value));
                                    var wordY = currentWord.Average(l => l.StartBaseLine.Y);
                                    words.Add((wordText, currentWord.Min(l => l.StartBaseLine.X), wordY));
                                }
                                currentWord = new List<Letter> { letter };
                            }
                        }
                        else if (currentWord.Count > 0)
                        {
                            // Letter is outside box, finish current word
                            var wordText = string.Join("", currentWord.Select(l => l.Value));
                            var wordY = currentWord.Average(l => l.StartBaseLine.Y);
                            words.Add((wordText, currentWord.Min(l => l.StartBaseLine.X), wordY));
                            currentWord.Clear();
                        }
                    }
                    
                    // Add the last word if any
                    if (currentWord.Count > 0)
                    {
                        var wordText = string.Join("", currentWord.Select(l => l.Value));
                        var wordY = currentWord.Average(l => l.StartBaseLine.Y);
                        words.Add((wordText, currentWord.Min(l => l.StartBaseLine.X), wordY));
                    }
                    
                    // Filter out common labels and dates, collect signature text
                    foreach (var word in words)
                    {
                        var cleanText = word.Text.Trim();
                        
                        // Skip if it's the label itself or common labels
                        if (cleanText.Contains("Customer", StringComparison.OrdinalIgnoreCase) ||
                            cleanText.Contains("Signature", StringComparison.OrdinalIgnoreCase) ||
                            cleanText.Contains("Manager", StringComparison.OrdinalIgnoreCase) ||
                            cleanText.Contains("End", StringComparison.OrdinalIgnoreCase))
                            continue;
                        
                        // Skip date patterns
                        if (Regex.IsMatch(cleanText, @"^\d{1,2}[/-]\d{1,2}[/-]\d{2,4}$"))
                            continue;
                        
                        // Skip single characters that are likely punctuation or formatting
                        if (cleanText.Length == 1 && !char.IsLetterOrDigit(cleanText[0]))
                            continue;
                        
                        // This is likely the customer signature text
                        if (!string.IsNullOrWhiteSpace(cleanText) && cleanText.Length >= 1)
                        {
                            customerSignatureTexts.Add((cleanText, word.Y));
                        }
                    }
                    
                    currentPageIndex++;
                }
                
                // If we found text in the customer signature box area, use it
                if (customerSignatureTexts.Any())
                {
                    // Combine all text found in the box area (in case signature spans multiple words)
                    // Order by Y position (bottom to top, since PDF Y=0 is at bottom)
                    var signatureText = string.Join(" ", customerSignatureTexts
                        .OrderBy(t => t.Y) // Order by Y position (bottom to top)
                        .Select(t => t.Text)
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .Distinct());
                    
                    if (!string.IsNullOrWhiteSpace(signatureText))
                    {
                        result.Type = SignatureType.Text;
                        result.TextSignature = signatureText.Trim();
                        result.FontFamily = "'Brush Script MT', cursive"; // Same style as manager
                        return result;
                    }
                }
            }
            
            // Fallback: Try traditional text extraction method (for backwards compatibility)
            var fullText = new System.Text.StringBuilder();
            foreach (var page in document.GetPages())
            {
                fullText.AppendLine(page.Text);
            }
            var pdfText = fullText.ToString();
            
            var customerSigIndex = pdfText.IndexOf("Customer Signature", StringComparison.OrdinalIgnoreCase);
            if (customerSigIndex >= 0)
            {
                var afterLabel = pdfText.Substring(customerSigIndex + "Customer Signature".Length);
                afterLabel = afterLabel.TrimStart(':', ' ', '\t', '\r', '\n');
                var lines = afterLabel.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    var cleanLine = line.Trim();
                    if (string.IsNullOrWhiteSpace(cleanLine))
                        continue;
                    if (Regex.IsMatch(cleanLine, @"^\d{1,2}[/-]\d{1,2}[/-]\d{2,4}$"))
                        continue;
                    if (cleanLine.Contains("Manager", StringComparison.OrdinalIgnoreCase) ||
                        cleanLine.Contains("End Signature", StringComparison.OrdinalIgnoreCase))
                        break;
                    if (cleanLine.Length >= 1)
                    {
                        result.Type = SignatureType.Text;
                        result.TextSignature = cleanLine;
                        result.FontFamily = "'Brush Script MT', cursive";
                        return result;
                    }
                }
            }
            
            // Fallback: Try to find images on the customer side (right half of page)
            var signatureImages = new List<(byte[] Data, string MimeType)>();
            
            foreach (var page in document.GetPages())
            {
                var pageWidth = page.Width;
                var midPageX = pageWidth / 2;
                
                try
                {
                    var images = page.GetImages().ToList();
                    foreach (var image in images)
                    {
                        var imageX = image.Bounds.Left;
                        if (imageX >= midPageX - 100)
                        {
                            if (image.TryGetPng(out var imageBytes))
                            {
                                signatureImages.Add((imageBytes, "image/png"));
                            }
                            else if (image.RawBytes.Count > 0)
                            {
                                signatureImages.Add((image.RawBytes.ToArray(), "image/jpeg"));
                            }
                        }
                    }
                }
                catch
                {
                    // Image extraction failed
                }
            }
            
            if (signatureImages.Any())
            {
                var signatureImage = signatureImages.Last();
                result.Type = SignatureType.Image;
                result.ImageData = signatureImage.Data;
                result.ImageMimeType = signatureImage.MimeType;
                result.ImageBase64 = Convert.ToBase64String(signatureImage.Data);
                return result;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error extracting signature from PDF: {ex.Message}");
        }
        
        return result;
    }
    
    /// <summary>
    /// Finds the position of "Customer Signature" label in the PDF to determine signature box coordinates
    /// Uses a simpler approach: looks for text in the right half of the page near the bottom
    /// </summary>
    private (double X, double Y, int PageIndex, double PageWidth, double PageHeight)? FindCustomerSignatureLabelPosition(PdfDocument document)
    {
        try
        {
            int pageIndex = 0;
            foreach (var page in document.GetPages())
            {
                var pageWidth = page.Width;
                var pageHeight = page.Height;
                var letters = page.Letters.ToList();
                
                if (letters.Count == 0)
                {
                    pageIndex++;
                    continue;
                }
                
                // Group letters into words
                var words = new List<(string Text, double X, double Y)>();
                var currentWord = new List<Letter>();
                
                foreach (var letter in letters.OrderBy(l => l.StartBaseLine.Y).ThenBy(l => l.StartBaseLine.X))
                {
                    var letterX = letter.StartBaseLine.X;
                    var letterY = letter.StartBaseLine.Y;
                    
                    // Check if this letter should be part of the current word
                    if (currentWord.Count == 0 || 
                        Math.Abs(letterX - currentWord.Last().StartBaseLine.X) < letter.Width * 2)
                    {
                        currentWord.Add(letter);
                    }
                    else
                    {
                        // Finish current word
                        if (currentWord.Count > 0)
                        {
                            var wordText = string.Join("", currentWord.Select(l => l.Value));
                            var wordX = currentWord.Min(l => l.StartBaseLine.X);
                            var wordY = currentWord.Average(l => l.StartBaseLine.Y);
                            words.Add((wordText, wordX, wordY));
                        }
                        currentWord = new List<Letter> { letter };
                    }
                }
                
                // Add the last word
                if (currentWord.Count > 0)
                {
                    var wordText = string.Join("", currentWord.Select(l => l.Value));
                    var wordX = currentWord.Min(l => l.StartBaseLine.X);
                    var wordY = currentWord.Average(l => l.StartBaseLine.Y);
                    words.Add((wordText, wordX, wordY));
                }
                
                // Look for "Customer Signature" label - typically in the bottom third of the page, right half
                var bottomThirdStart = pageHeight * 0.66; // Bottom third starts at 66% of page height
                var rightHalfStart = pageWidth * 0.5; // Right half starts at 50% of page width
                
                for (int i = 0; i < words.Count - 1; i++)
                {
                    var word1 = words[i];
                    var word2 = words[i + 1];
                    
                    // Check if words are close together horizontally and vertically
                    var horizontalDistance = Math.Abs(word2.X - (word1.X + word1.Text.Length * 10)); // Approximate width
                    var verticalDistance = Math.Abs(word2.Y - word1.Y);
                    
                    if (horizontalDistance < 50 && verticalDistance < 20)
                    {
                        var combined = (word1.Text + " " + word2.Text).Trim();
                        if (combined.Contains("Customer", StringComparison.OrdinalIgnoreCase) &&
                            combined.Contains("Signature", StringComparison.OrdinalIgnoreCase))
                        {
                            // Check if it's in the right area (right half, bottom third)
                            var labelX = Math.Max(word1.X, word2.X);
                            var labelY = Math.Max(word1.Y, word2.Y);
                            
                            if (labelX >= rightHalfStart - 100 && labelY >= bottomThirdStart - 100)
                            {
                                return (labelX, labelY, pageIndex, pageWidth, pageHeight);
                            }
                        }
                    }
                }
                
                // If we didn't find the exact label, return a default position for right side, bottom area
                // This is a fallback for when the label text isn't found but we still want to search the signature area
                return (pageWidth * 0.75, pageHeight * 0.8, pageIndex, pageWidth, pageHeight);
            }
        }
        catch
        {
            // If coordinate-based search fails, return null to use fallback method
        }
        
        return null;
    }

    /// <summary>
    /// Maps PDF font names to CSS font families
    /// </summary>
    private string DetectSignatureFontFamily(string? pdfFontName)
    {
        if (string.IsNullOrEmpty(pdfFontName))
            return "'Brush Script MT', cursive"; // Default signature font
        
        var fontLower = pdfFontName.ToLowerInvariant();
        
        // Script/cursive fonts
        if (fontLower.Contains("brush") || fontLower.Contains("script") || fontLower.Contains("cursive"))
            return "'Brush Script MT', cursive";
        
        if (fontLower.Contains("segoe") && fontLower.Contains("script"))
            return "'Segoe Script', cursive";
        
        if (fontLower.Contains("lucida") && fontLower.Contains("handwriting"))
            return "'Lucida Handwriting', cursive";
        
        if (fontLower.Contains("comic"))
            return "'Comic Sans MS', cursive";
        
        if (fontLower.Contains("monotype") && fontLower.Contains("corsiva"))
            return "'Monotype Corsiva', cursive";
        
        if (fontLower.Contains("palace") || fontLower.Contains("script"))
            return "'Palace Script MT', cursive";
        
        if (fontLower.Contains("freestyle"))
            return "'Freestyle Script', cursive";
        
        if (fontLower.Contains("bradley"))
            return "'Bradley Hand ITC', cursive";
        
        if (fontLower.Contains("mistral"))
            return "'Mistral', cursive";
        
        // Handwritten style fonts
        if (fontLower.Contains("hand") || fontLower.Contains("write"))
            return "'Segoe Script', cursive";
        
        // Default to cursive for signature styling
        return "'Brush Script MT', cursive";
    }

    /// <summary>
    /// Extracts signature data from uploaded Word document
    /// </summary>
    private ExtractedSignatureData ExtractSignatureFromWord(Stream fileStream)
    {
        var result = new ExtractedSignatureData { Type = SignatureType.None };
        
        try
        {
            using var wordDoc = WordprocessingDocument.Open(fileStream, false);
            var mainPart = wordDoc.MainDocumentPart;
            if (mainPart == null)
                return result;
            
            var body = mainPart.Document?.Body;
            if (body == null)
                return result;
            
            // Look for images in the document (signature images)
            var imageParts = mainPart.ImageParts.ToList();
            if (imageParts.Any())
            {
                // Get the last image (likely the signature)
                var lastImage = imageParts.Last();
                using var imageStream = lastImage.GetStream();
                using var ms = new MemoryStream();
                imageStream.CopyTo(ms);
                var imageBytes = ms.ToArray();
                
                if (imageBytes.Length > 100) // Minimum size for a valid signature image
                {
                    result.Type = SignatureType.Image;
                    result.ImageData = imageBytes;
                    result.ImageMimeType = lastImage.ContentType;
                    result.ImageBase64 = Convert.ToBase64String(imageBytes);
                    return result;
                }
            }
            
            // If no image, extract text and look for signature
            var fullText = body.InnerText;
            var customerSignature = ExtractCustomerSignatureFromText(fullText);
            
            if (!string.IsNullOrWhiteSpace(customerSignature))
            {
                result.Type = SignatureType.Text;
                result.TextSignature = customerSignature;
                result.FontFamily = "'Brush Script MT', cursive"; // Default for Word extractions
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error extracting signature from Word: {ex.Message}");
        }
        
        return result;
    }

    /// <summary>
    /// Extracts customer signature text from plain text content
    /// </summary>
    private string ExtractCustomerSignatureFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        
        // Find "Customer Signature" and extract text after it
        var customerSigIndex = text.IndexOf("Customer Signature", StringComparison.OrdinalIgnoreCase);
        if (customerSigIndex < 0)
            return string.Empty;
        
        // Get text after the label
        var afterLabel = text.Substring(customerSigIndex + "Customer Signature".Length);
        
        // Remove common separators and whitespace
        afterLabel = afterLabel.TrimStart(':', ' ', '\t', '\r', '\n');
        
        // Find where to stop (Manager Signature, End, or date pattern)
        var stopPatterns = new[] { "Manager Signature", "End Signature", "<!-- End" };
        var endIndex = afterLabel.Length;
        
        foreach (var pattern in stopPatterns)
        {
            var idx = afterLabel.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
            if (idx > 0 && idx < endIndex)
                endIndex = idx;
        }
        
        // Also stop at date patterns
        var dateMatch = Regex.Match(afterLabel, @"\d{1,2}[/-]\d{1,2}[/-]\d{2,4}");
        if (dateMatch.Success && dateMatch.Index < endIndex)
            endIndex = dateMatch.Index;
        
        var signatureText = afterLabel.Substring(0, endIndex).Trim();
        
        // Clean up and validate
        signatureText = Regex.Replace(signatureText, @"\s+", " ").Trim();
        
        // Must be reasonable length for a signature
        if (signatureText.Length >= 2 && signatureText.Length <= 100)
            return signatureText;
        
        return string.Empty;
    }

    /// <summary>
    /// Adds customer signature to contract body JSON - supports both text and image signatures
    /// Uses the rectangle box structure with id="customer-signature-box"
    /// </summary>
    private string AddCustomerSignatureToContract(string bodyJson, ExtractedSignatureData signatureData)
    {
        if (string.IsNullOrEmpty(bodyJson) || signatureData.Type == SignatureType.None)
            return bodyJson;
        
        string signatureContent;
        
        if (signatureData.Type == SignatureType.Image && !string.IsNullOrEmpty(signatureData.ImageBase64))
        {
            // Image signature - embed as base64 img tag
            var mimeType = signatureData.ImageMimeType ?? "image/png";
            signatureContent = $@"<img src=""data:{mimeType};base64,{signatureData.ImageBase64}"" alt=""Customer Signature"" style=""max-height: 50px; max-width: 200px;"" />";
        }
        else if (signatureData.Type == SignatureType.Text && !string.IsNullOrEmpty(signatureData.TextSignature))
        {
            // Text signature - HTML encode for safety
            signatureContent = WebUtility.HtmlEncode(signatureData.TextSignature);
        }
        else
        {
            return bodyJson;
        }
        
        // New box style for signed signature
        var signedBoxStyle = @"font-family: 'Brush Script MT', cursive; font-size: 28px; border: 2px solid #000; min-height: 60px; padding: 10px; display: flex; align-items: center; justify-content: center;";
        
        // Try to find and replace the customer-signature-box div (new rectangle box structure)
        var boxPattern = @"<div id=""customer-signature-box""[^>]*>[\s\S]*?</div>";
        var boxMatch = Regex.Match(bodyJson, boxPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        if (boxMatch.Success)
        {
            // Replace the entire customer signature box with the signed version
            var newBox = $@"<div id=""customer-signature-box"" style=""{signedBoxStyle}"">
            {signatureContent}
        </div>";
            
            bodyJson = bodyJson.Substring(0, boxMatch.Index) + newBox + bodyJson.Substring(boxMatch.Index + boxMatch.Length);
            
            // Also update the date
            var datePattern = @"(customer-signature-box[\s\S]*?<div style=""margin-top: 10px; font-size: 12px;"">)\s*(&nbsp;|[\s]*)\s*(</div>)";
            var dateMatch = Regex.Match(bodyJson, datePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (dateMatch.Success)
            {
                var newDate = $"{dateMatch.Groups[1].Value}\n            {DateTime.UtcNow.ToString("MM/dd/yyyy")}\n        {dateMatch.Groups[3].Value}";
                bodyJson = bodyJson.Substring(0, dateMatch.Index) + newDate + bodyJson.Substring(dateMatch.Index + dateMatch.Length);
            }
            
            return bodyJson;
        }
        
        // Fallback: Try old pattern for backwards compatibility
        var customerSignaturePattern = @"(<div[^>]*>[\s\S]*?<strong>Customer Signature:</strong>[\s\S]*?</div>[\s\S]*?<div[^>]*font-family:[^>]*>)([\s\S]*?)(</div>[\s\S]*?<div[^>]*margin-top:[^>]*>[\s\S]*?</div>[\s\S]*?</div>)";
        
        var match = Regex.Match(bodyJson, customerSignaturePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        if (match.Success)
        {
            var replacement = $"{match.Groups[1].Value}\n            {signatureContent}\n        {match.Groups[3].Value}";
            bodyJson = bodyJson.Substring(0, match.Index) + replacement + bodyJson.Substring(match.Index + match.Length);
        }
        else
        {
            // Try simpler pattern
            var simplePattern = @"(Customer Signature:[\s\S]*?<div[^>]*cursive[^>]*>)\s*([\s\S]*?)\s*(</div>)";
            var simpleMatch = Regex.Match(bodyJson, simplePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            if (simpleMatch.Success)
            {
                var replacement = $"{simpleMatch.Groups[1].Value}\n            {signatureContent}\n        {simpleMatch.Groups[3].Value}";
                bodyJson = bodyJson.Substring(0, simpleMatch.Index) + replacement + bodyJson.Substring(simpleMatch.Index + simpleMatch.Length);
            }
            else
            {
                // Final fallback: Use AddSignatureToContract
                if (signatureData.Type == SignatureType.Text && !string.IsNullOrEmpty(signatureData.TextSignature))
                {
                    bodyJson = AddSignatureToContract(bodyJson, signatureData.TextSignature, "right");
                }
            }
        }
        
        return bodyJson;
    }

    public async Task<byte[]> DownloadContractAsPdfAsync(int id, int userId)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == id,
            "ContractTemplate,Rental,Staff,Manager");

        if (contractDraft == null)
            throw new InvalidOperationException("Contract draft not found");

        // Check authorization - user must be staff, manager, admin, or the customer
        var dbContext = _contractDraftsRepository.GetDbContext();
        var rental = await dbContext.Rentals
            .FirstOrDefaultAsync(r => r.Id == contractDraft.RentalId);

        if (rental == null || (rental.AccountId != userId && contractDraft.StaffId != userId && contractDraft.ManagerId != userId))
        {
            // Check if user is admin (would need to check roles, but for now just allow if they have access)
            throw new UnauthorizedAccessException("You are not authorized to download this contract");
        }

        if (string.IsNullOrEmpty(contractDraft.BodyJson))
            throw new InvalidOperationException("Contract body is empty");

        // Store original body JSON snapshot when customer downloads for signing
        if (contractDraft.Status == "PendingCustomerSignature")
        {
            contractDraft.OriginalBodyJson = contractDraft.BodyJson;
            await _contractDraftsRepository.UpdateAsync(contractDraft);
        }

        // Convert HTML to PDF using PuppeteerSharp (headless Chrome)
        // This preserves all HTML/CSS formatting, tables, styles, etc.
        
        var htmlContent = contractDraft.BodyJson;
        
        // Wrap HTML in a complete document if it's not already
        if (!htmlContent.Contains("<html", StringComparison.OrdinalIgnoreCase))
        {
            htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        table {{ border-collapse: collapse; width: 100%; }}
        table, th, td {{ border: 1px solid black; padding: 8px; }}
        th {{ background-color: #f2f2f2; }}
    </style>
</head>
<body>
    {htmlContent}
</body>
</html>";
        }
        
        // Download Chromium if not already downloaded (first time only)
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        
        // Launch headless browser and generate PDF
        var launchOptions = new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        };
        
        await using var browser = await Puppeteer.LaunchAsync(launchOptions);
        await using var page = await browser.NewPageAsync();
        
        // Set HTML content
        await page.SetContentAsync(htmlContent);
        
        // Generate PDF with options
        var pdfOptions = new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "20mm",
                Bottom = "20mm",
                Left = "15mm",
                Right = "15mm"
            },
            DisplayHeaderFooter = false // No header/footer - only show BodyJson content
        };
        
        var pdfBytes = await page.PdfDataAsync(pdfOptions);
        
        return pdfBytes;
    }

    public async Task<byte[]> DownloadContractAsWordAsync(int id, int userId)
    {
        var contractDraft = await _contractDraftsRepository.GetAsync(
            cd => cd.Id == id,
            "ContractTemplate,Rental,Staff,Manager");

        if (contractDraft == null)
            throw new InvalidOperationException("Contract draft not found");

        // Check authorization
        var dbContext = _contractDraftsRepository.GetDbContext();
        var rental = await dbContext.Rentals
            .FirstOrDefaultAsync(r => r.Id == contractDraft.RentalId);

        if (rental == null || (rental.AccountId != userId && contractDraft.StaffId != userId && contractDraft.ManagerId != userId))
        {
            throw new UnauthorizedAccessException("You are not authorized to download this contract");
        }

        if (string.IsNullOrEmpty(contractDraft.BodyJson))
            throw new InvalidOperationException("Contract body is empty");

        // Store original body JSON snapshot when customer downloads for signing
        if (contractDraft.Status == "PendingCustomerSignature")
        {
            contractDraft.OriginalBodyJson = contractDraft.BodyJson;
            await _contractDraftsRepository.UpdateAsync(contractDraft);
        }

        // Convert HTML to Word document
        using var stream = new MemoryStream();
        using (var wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
        {
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            var body = mainPart.Document.AppendChild(new Body());

            // Convert HTML to plain text (simplified - for better results, use a proper HTML to Word converter)
            var htmlText = contractDraft.BodyJson;
            // Remove HTML tags for basic conversion
            var plainText = System.Text.RegularExpressions.Regex.Replace(htmlText, "<.*?>", string.Empty);
            plainText = System.Net.WebUtility.HtmlDecode(plainText);

            var paragraph = body.AppendChild(new Paragraph());
            var run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(plainText));
        }

        return stream.ToArray();
    }

    public async Task<ContractDraftsResponse?> CustomerSignContractWithFileAsync(int id, IFormFile signedContractFile, int customerId)
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

        // Validate that original body JSON exists
        if (string.IsNullOrEmpty(contractDraft.OriginalBodyJson))
            throw new InvalidOperationException("Original contract not found. Please download the contract first before signing.");

        // Determine file type
        bool isPdfFile = signedContractFile.ContentType == "application/pdf" || 
                         signedContractFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
        bool isWordFile = signedContractFile.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" || 
                          signedContractFile.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase);
        bool isHtmlFile = signedContractFile.ContentType == "text/html" || 
                          signedContractFile.FileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                          signedContractFile.FileName.EndsWith(".htm", StringComparison.OrdinalIgnoreCase);

        ExtractedSignatureData signatureData = new ExtractedSignatureData { Type = SignatureType.None };
        string extractedContent = string.Empty;

        // Process file based on type
        if (isPdfFile)
        {
            // Read PDF bytes
            using var memoryStream = new MemoryStream();
            await signedContractFile.CopyToAsync(memoryStream);
            var pdfBytes = memoryStream.ToArray();
            
            // Extract signature using PdfPig (supports both text and images)
            signatureData = ExtractSignatureFromPdf(pdfBytes);
            
            // Also extract text content for validation using Docnet
            try
            {
                using var docReader = DocLib.Instance.GetDocReader(pdfBytes, new PageDimensions(1080, 1920));
                var textBuilder = new System.Text.StringBuilder();
                
                for (int i = 0; i < docReader.GetPageCount(); i++)
                {
                    using var pageReader = docReader.GetPageReader(i);
                    var pageText = pageReader.GetText();
                    textBuilder.AppendLine(pageText);
                }
                
                extractedContent = textBuilder.ToString();
                
                // Fallback: If PdfPig didn't find signature, try extracting from Docnet text
                if (signatureData.Type == SignatureType.None && !string.IsNullOrEmpty(extractedContent))
                {
                    var customerSigIndex = extractedContent.IndexOf("Customer Signature", StringComparison.OrdinalIgnoreCase);
                    if (customerSigIndex >= 0)
                    {
                        var searchWindow = extractedContent.Substring(customerSigIndex, Math.Min(500, extractedContent.Length - customerSigIndex));
                        var pattern = Regex.Match(searchWindow, @"Customer\s+Signature:?\s*([A-Za-z][A-Za-z0-9\s.'-]{1,100})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        if (pattern.Success)
                        {
                            var sigText = pattern.Groups[1].Value.Trim();
                            if (!string.IsNullOrWhiteSpace(sigText) && 
                                !sigText.Equals("Signature", StringComparison.OrdinalIgnoreCase) &&
                                !Regex.IsMatch(sigText, @"^\d{1,2}[/-]\d{1,2}[/-]\d{2,4}$"))
                            {
                                signatureData = new ExtractedSignatureData
                                {
                                    Type = SignatureType.Text,
                                    TextSignature = sigText,
                                    FontFamily = "'Brush Script MT', cursive"
                                };
                            }
                        }
                        
                        // If pattern didn't match, try line-by-line approach
                        if (signatureData.Type == SignatureType.None)
                        {
                            var afterLabel = extractedContent.Substring(customerSigIndex + "Customer Signature".Length);
                            afterLabel = afterLabel.TrimStart(':', ' ', '\t', '\r', '\n');
                            var lines = afterLabel.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var line in lines)
                            {
                                var cleanLine = line.Trim();
                                if (string.IsNullOrWhiteSpace(cleanLine) ||
                                    Regex.IsMatch(cleanLine, @"^\d{1,2}[/-]\d{1,2}[/-]\d{2,4}$") ||
                                    cleanLine.Contains("Manager", StringComparison.OrdinalIgnoreCase) ||
                                    cleanLine.Contains("End Signature", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (cleanLine.Contains("Manager", StringComparison.OrdinalIgnoreCase) ||
                                        cleanLine.Contains("End Signature", StringComparison.OrdinalIgnoreCase))
                                        break;
                                    continue;
                                }
                                if (cleanLine.Length >= 1)
                                {
                                    signatureData = new ExtractedSignatureData
                                    {
                                        Type = SignatureType.Text,
                                        TextSignature = cleanLine,
                                        FontFamily = "'Brush Script MT', cursive"
                                    };
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // If Docnet fails, use PdfPig for text extraction
                try
                {
                    using var document = PdfDocument.Open(pdfBytes);
                    var textBuilder = new System.Text.StringBuilder();
                    foreach (var page in document.GetPages())
                    {
                        textBuilder.AppendLine(page.Text);
                    }
                    extractedContent = textBuilder.ToString();
                }
                catch
                {
                    // If both fail, proceed without content validation
                    extractedContent = "PDF_CONTENT_EXTRACTION_FAILED";
                }
            }
        }
        else if (isWordFile)
        {
            // Extract signature from Word document
            using var fileStream = signedContractFile.OpenReadStream();
            
            // Need to copy to memory stream since we need to read multiple times
            using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);
            ms.Position = 0;
            
            signatureData = ExtractSignatureFromWord(ms);
            
            // Also extract text for validation
            ms.Position = 0;
            try
            {
                using var wordDoc = WordprocessingDocument.Open(ms, false);
                var body = wordDoc.MainDocumentPart?.Document?.Body;
                if (body != null)
                {
                    extractedContent = body.InnerText;
                }
            }
            catch
            {
                extractedContent = "WORD_CONTENT_EXTRACTION_FAILED";
            }
        }
        else if (isHtmlFile)
        {
            // Read HTML content
            using var reader = new StreamReader(signedContractFile.OpenReadStream());
            var htmlContent = await reader.ReadToEndAsync();
            extractedContent = htmlContent;
            
            // Try to extract signature from HTML
            var customerSignatureText = ExtractCustomerSignature(htmlContent, contractDraft.OriginalBodyJson);
            if (!string.IsNullOrWhiteSpace(customerSignatureText))
            {
                signatureData = new ExtractedSignatureData
                {
                    Type = SignatureType.Text,
                    TextSignature = customerSignatureText,
                    FontFamily = "'Brush Script MT', cursive"
                };
            }
            
            // Check for base64 images in the customer signature area
            var imagePattern = @"Customer Signature:[\s\S]*?<img[^>]*src=[""']data:([^;]+);base64,([^""']+)[""'][^>]*>";
            var imgMatch = Regex.Match(htmlContent, imagePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (imgMatch.Success)
            {
                signatureData = new ExtractedSignatureData
                {
                    Type = SignatureType.Image,
                    ImageMimeType = imgMatch.Groups[1].Value,
                    ImageBase64 = imgMatch.Groups[2].Value,
                    ImageData = Convert.FromBase64String(imgMatch.Groups[2].Value)
                };
            }
        }
        else
        {
            throw new InvalidOperationException("Unsupported file format. Please upload a PDF, Word document (.docx), or HTML file.");
        }

        // Validate contract integrity (skip if content extraction failed)
        if (!extractedContent.Contains("EXTRACTION_FAILED"))
        {
            var isValid = ValidateContractIntegrity(contractDraft.OriginalBodyJson, extractedContent);
            
            if (!isValid)
            {
                throw new InvalidOperationException("Contract has been modified outside of the signature section. Only signature changes are allowed.");
            }
        }

        // Validate that a signature was found
        if (signatureData.Type == SignatureType.None)
        {
            throw new InvalidOperationException("Customer signature not found in the uploaded file. Please ensure you have added your signature to the document before uploading. You can sign using text (typed signature) or by inserting/drawing a signature image.");
        }

        // Update body JSON with the customer signature
        if (isHtmlFile && signatureData.Type == SignatureType.None)
        {
            // For HTML files without extracted signature, use the content directly
            using var reader = new StreamReader(signedContractFile.OpenReadStream());
            signedContractFile.OpenReadStream().Position = 0;
            contractDraft.BodyJson = await reader.ReadToEndAsync();
        }
        else
        {
            // Apply the extracted signature to the original body JSON
            contractDraft.BodyJson = AddCustomerSignatureToContract(contractDraft.OriginalBodyJson, signatureData);
        }

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

    /// <summary>
    /// Validates that the uploaded contract only has changes in the signature section
    /// </summary>
    private bool ValidateContractIntegrity(string originalBodyJson, string uploadedContent)
    {
        // Convert HTML to plain text for fair comparison
        var originalPlainText = ConvertHtmlToPlainText(originalBodyJson);
        var uploadedPlainText = ConvertHtmlToPlainText(uploadedContent);

        // Normalize both strings for comparison (remove extra whitespace, newlines)
        var normalizedOriginal = NormalizeTextForComparison(originalPlainText);
        var normalizedUploaded = NormalizeTextForComparison(uploadedPlainText);

        // For now, do a lenient comparison - check if the main content is similar
        // We'll use a similarity approach: if most of the content matches, it's valid
        // This is more lenient to handle PDF extraction differences
        
        // Simple approach: check if uploaded contains most key terms from original
        // Remove common words and check similarity
        var originalWords = normalizedOriginal.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 3) // Filter out short words
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        var uploadedWords = normalizedUploaded.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 3)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Calculate how many original words are in the uploaded content
        var matchingWords = originalWords.Intersect(uploadedWords, StringComparer.OrdinalIgnoreCase).Count();
        var totalOriginalWords = originalWords.Count;

        if (totalOriginalWords == 0)
            return true; // Empty original, allow

        // If at least 80% of significant words match, consider it valid
        // This accounts for formatting differences from PDF extraction
        var matchPercentage = (double)matchingWords / totalOriginalWords;
        
        return matchPercentage >= 0.80; // 80% similarity threshold
    }
    
    /// <summary>
    /// Convert HTML to plain text by stripping all HTML tags
    /// </summary>
    private string ConvertHtmlToPlainText(string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;
        
        // Remove HTML tags
        var plainText = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
        
        // Decode HTML entities
        plainText = System.Net.WebUtility.HtmlDecode(plainText);
        
        // Remove special characters and normalize
        plainText = System.Text.RegularExpressions.Regex.Replace(plainText, @"[\u200B-\u200D\uFEFF]", string.Empty);
        plainText = System.Text.RegularExpressions.Regex.Replace(plainText, @"&nbsp;", " ");
        
        return plainText;
    }
    
    /// <summary>
    /// Normalize text for comparison by removing extra whitespace
    /// </summary>
    private string NormalizeTextForComparison(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
        
        // Remove newlines and normalize whitespace
        text = text.Replace("\r", " ").Replace("\n", " ");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
        
        return text.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Extracts the signature section from HTML
    /// </summary>
    private string ExtractSignatureSection(string html)
    {
        var signatureSectionPattern = @"<div\s+id=[""']contract-signatures-section[""'][^>]*>.*?</div>\s*<!--\s*End\s+Signature\s+Section\s*-->";
        var match = Regex.Match(html, signatureSectionPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return match.Success ? match.Value : string.Empty;
    }

    /// <summary>
    /// Removes the signature section from HTML for comparison
    /// </summary>
    private string RemoveSignatureSection(string html)
    {
        var signatureSectionPattern = @"<div\s+id=[""']contract-signatures-section[""'][^>]*>.*?</div>\s*<!--\s*End\s+Signature\s+Section\s*-->";
        return Regex.Replace(html, signatureSectionPattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }

    /// <summary>
    /// Extracts customer signature from signature section
    /// </summary>
    private string ExtractCustomerSignatureFromSection(string signatureSection)
    {
        var customerPattern = @"Customer\s+Signature:.*?<div[^>]*>([^<]*)</div>";
        var match = Regex.Match(signatureSection, customerPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }

    /// <summary>
    /// Extracts manager signature from signature section
    /// </summary>
    private string ExtractManagerSignatureFromSection(string signatureSection)
    {
        var managerPattern = @"Manager\s+Signature:.*?<div[^>]*>([^<]*)</div>";
        var match = Regex.Match(signatureSection, managerPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }

    /// <summary>
    /// Extracts customer signature from uploaded content
    /// </summary>
    private string ExtractCustomerSignature(string uploadedContent, string originalBodyJson)
    {
        // Try to extract from HTML structure - improved pattern to capture signature content
        // Pattern 1: Look for content in the customer signature div with cursive font
        var customerPatternWithFont = @"Customer\s+Signature:.*?<div[^>]*font-family:\s*['""]?Brush\s+Script\s+MT['""]?[^>]*>([^<]+)</div>";
        var match = Regex.Match(uploadedContent, customerPatternWithFont, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        if (match.Success)
        {
            var signature = match.Groups[1].Value.Trim();
            if (!string.IsNullOrEmpty(signature) && signature != "&nbsp;" && signature != " ")
            {
                return WebUtility.HtmlDecode(signature);
            }
        }

        // Pattern 2: Try broader pattern for customer signature section
        var customerPattern = @"Customer\s+Signature:\s*</strong>\s*</div>\s*<div[^>]*>([^<]+)</div>";
        match = Regex.Match(uploadedContent, customerPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        if (match.Success)
        {
            var signature = match.Groups[1].Value.Trim();
            if (!string.IsNullOrEmpty(signature) && signature != "&nbsp;" && signature != " ")
            {
                return WebUtility.HtmlDecode(signature);
            }
        }

        // Pattern 3: Look for any non-empty text after "Customer Signature:" label
        var anyTextPattern = @"Customer\s+Signature:[^<]*(?:<[^>]+>)*\s*([A-Za-z0-9][A-Za-z0-9\s.'-]{2,50})";
        match = Regex.Match(uploadedContent, anyTextPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        if (match.Success)
        {
            var signature = match.Groups[1].Value.Trim();
            if (!string.IsNullOrEmpty(signature) && signature.Length > 2)
            {
                return WebUtility.HtmlDecode(signature);
            }
        }

        // Pattern 4: For plain text (PDF/Word extraction), look for text after "Customer Signature:"
        var lines = uploadedContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("Customer Signature", StringComparison.OrdinalIgnoreCase))
            {
                // Check next few lines for a signature
                for (int j = i + 1; j < Math.Min(i + 5, lines.Length); j++)
                {
                    var line = lines[j].Trim();
                    // Skip empty lines, dates, and common labels
                    if (string.IsNullOrWhiteSpace(line) || 
                        Regex.IsMatch(line, @"^\d{1,2}[/-]\d{1,2}[/-]\d{2,4}$") ||
                        line.Contains("Manager Signature", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    
                    // Found a potential signature line
                    if (line.Length >= 2 && line.Length <= 100)
                    {
                        return line;
                    }
                }
            }
        }

        // If no signature found, return empty string (caller will handle validation)
        return string.Empty;
    }
}

