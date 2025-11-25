using AutoMapper;
using RoboRent_BE.Model.DTOS.TemplateClauses;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;
using System.Text.RegularExpressions;

namespace RoboRent_BE.Service.Services;

public class TemplateClausesService : ITemplateClausesService
{
    private readonly ITemplateClausesRepository _templateClausesRepository;
    private readonly IContractTemplatesRepository _contractTemplatesRepository;
    private readonly IMapper _mapper;

    public TemplateClausesService(ITemplateClausesRepository templateClausesRepository, IContractTemplatesRepository contractTemplatesRepository, IMapper mapper)
    {
        _templateClausesRepository = templateClausesRepository;
        _contractTemplatesRepository = contractTemplatesRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TemplateClausesResponse>> GetAllTemplateClausesAsync()
    {
        var templateClauses = await _templateClausesRepository.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<TemplateClausesResponse>>(templateClauses);
    }

    public async Task<TemplateClausesResponse?> GetTemplateClausesByIdAsync(int id)
    {
        var templateClause = await _templateClausesRepository.GetAsync(tc => tc.Id == id, "ContractTemplate");
        if (templateClause == null)
            return null;

        return _mapper.Map<TemplateClausesResponse>(templateClause);
    }

    public async Task<IEnumerable<TemplateClausesResponse>> GetTemplateClausesByContractTemplateIdAsync(int contractTemplateId)
    {
        var templateClauses = await _templateClausesRepository.GetTemplateClausesByContractTemplateIdAsync(contractTemplateId);
        return _mapper.Map<IEnumerable<TemplateClausesResponse>>(templateClauses);
    }

    public async Task<IEnumerable<TemplateClausesResponse>> GetTemplateClausesByIsMandatoryAsync(bool isMandatory)
    {
        var templateClauses = await _templateClausesRepository.GetTemplateClausesByIsMandatoryAsync(isMandatory);
        return _mapper.Map<IEnumerable<TemplateClausesResponse>>(templateClauses);
    }

    public async Task<IEnumerable<TemplateClausesResponse>> GetTemplateClausesByIsEditableAsync(bool isEditable)
    {
        var templateClauses = await _templateClausesRepository.GetTemplateClausesByIsEditableAsync(isEditable);
        return _mapper.Map<IEnumerable<TemplateClausesResponse>>(templateClauses);
    }

    public async Task<IEnumerable<TemplateClausesResponse>> GetAvailableTemplateClausesForDraftAsync(int contractTemplateId, int contractDraftId)
    {
        var templateClauses = await _templateClausesRepository.GetAvailableTemplateClausesForDraftAsync(contractTemplateId, contractDraftId);
        return _mapper.Map<IEnumerable<TemplateClausesResponse>>(templateClauses);
    }

    public async Task<TemplateClausesResponse> CreateTemplateClausesAsync(CreateTemplateClausesRequest request)
    {
        var templateClause = _mapper.Map<TemplateClauses>(request);
        var createdTemplateClause = await _templateClausesRepository.AddAsync(templateClause);
        
        return _mapper.Map<TemplateClausesResponse>(createdTemplateClause);
    }

    public async Task<TemplateClausesResponse?> UpdateTemplateClausesAsync(int id, UpdateTemplateClausesRequest request)
    {
        var existingTemplateClause = await _templateClausesRepository.GetAsync(tc => tc.Id == id, "ContractTemplate");
        if (existingTemplateClause == null)
            return null;

        // Determine the clause number to use:
        // 1. First, try to extract from the request input (if user provided it)
        // 2. If not found in request, extract from existing clause
        // 3. If still not found, calculate from other clauses
        
        int? clauseNumber = null; // Use nullable to track if we found a number
        
        // Try to extract from request ClauseCode or Title first
        var requestClauseText = request.ClauseCode ?? request.Title ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(requestClauseText))
        {
            // Check if request text has the pattern
            var hasRequestPattern = Regex.IsMatch(requestClauseText, @"Điều\s+\d+\.", RegexOptions.IgnoreCase);
            if (hasRequestPattern)
            {
                var requestClauseNumber = ExtractClauseNumber(requestClauseText);
                if (requestClauseNumber > 0)
                {
                    clauseNumber = requestClauseNumber;
                }
            }
        }
        
        // If no number found in request, try existing clause
        if (!clauseNumber.HasValue)
        {
            var existingClauseText = existingTemplateClause.ClauseCode ?? existingTemplateClause.Title ?? string.Empty;
            
            // Check if the existing clause actually has a number pattern
            var hasClausePattern = !string.IsNullOrWhiteSpace(existingClauseText) && 
                                   Regex.IsMatch(existingClauseText, @"Điều\s+\d+\.", RegexOptions.IgnoreCase);
            
            if (hasClausePattern)
            {
                var existingClauseNumber = ExtractClauseNumber(existingClauseText);
                if (existingClauseNumber > 0)
                {
                    clauseNumber = existingClauseNumber;
                }
            }
        }
        
        // If still no pattern found, calculate from all other clauses (excluding current one)
        if (!clauseNumber.HasValue && existingTemplateClause.ContractTemplatesId.HasValue)
        {
            // Get all existing clauses for this contract template (excluding the current one)
            var allClauses = await _templateClausesRepository.GetTemplateClausesByContractTemplateIdAsync(existingTemplateClause.ContractTemplatesId.Value);
            var otherClauses = allClauses.Where(c => c.Id != id);
            
            // Calculate max + 1 from other clauses
            clauseNumber = GetNextClauseNumber(otherClauses);
        }
        
        // Default to 1 if still no number found
        int finalClauseNumber = clauseNumber ?? 1;
        
        // Format ClauseCode and Title with the same logic as create function
        // Use the request input if provided, otherwise use existing values
        string inputClauseCode = request.ClauseCode ?? existingTemplateClause.ClauseCode ?? string.Empty;
        string inputTitle = request.Title ?? existingTemplateClause.Title ?? string.Empty;
        
        string formattedClauseCode = FormatClauseCode(inputClauseCode, finalClauseNumber);
        string formattedTitle = FormatClauseTitle(inputTitle, finalClauseNumber);

        // Check if content is being changed (Title or Body)
        bool contentChanged = existingTemplateClause.Title != formattedTitle || 
                             existingTemplateClause.Body != request.Body;

        // Store original values before mapping (needed for BodyJson update)
        var originalTitle = existingTemplateClause.Title;
        var originalBody = existingTemplateClause.Body;
        var contractTemplate = existingTemplateClause.ContractTemplate;
        
        // Preserve the original ContractTemplatesId - it cannot be changed
        var originalContractTemplatesId = existingTemplateClause.ContractTemplatesId;

        // Update only the allowed fields manually (to avoid mapping ContractTemplatesId)
        existingTemplateClause.ClauseCode = formattedClauseCode;
        existingTemplateClause.Title = formattedTitle;
        existingTemplateClause.Body = request.Body ?? existingTemplateClause.Body;
        existingTemplateClause.IsMandatory = request.IsMandatory ?? existingTemplateClause.IsMandatory;
        existingTemplateClause.IsEditable = request.IsEditable ?? existingTemplateClause.IsEditable;
        
        // Ensure ContractTemplatesId remains unchanged
        existingTemplateClause.ContractTemplatesId = originalContractTemplatesId;
        
        // Update the clause in database
        var updatedTemplateClause = await _templateClausesRepository.UpdateAsync(existingTemplateClause);
        
        // If content changed and we have a contract template, update the BodyJson
        if (contentChanged && contractTemplate != null)
        {
            await UpdateClauseInContractTemplateBodyJsonAsync(contractTemplate, updatedTemplateClause, originalTitle, originalBody);
        }
        
        return _mapper.Map<TemplateClausesResponse>(updatedTemplateClause);
    }

    public async Task<bool> DeleteTemplateClausesAsync(int id)
    {
        var templateClause = await _templateClausesRepository.GetAsync(tc => tc.Id == id, "ContractTemplate");
        if (templateClause == null)
            return false;

        // Store contract template and clause info before deletion
        var contractTemplate = templateClause.ContractTemplate;
        var clauseTitle = templateClause.Title;
        var clauseId = templateClause.Id;
        var deletedClauseNumber = ExtractClauseNumber(templateClause.ClauseCode ?? templateClause.Title ?? string.Empty);
        var contractTemplateId = templateClause.ContractTemplatesId;

        // Delete the clause from database
        await _templateClausesRepository.DeleteAsync(templateClause);

        // Remove the clause from ContractTemplate BodyJson if it exists
        if (contractTemplate != null)
        {
            await RemoveClauseFromContractTemplateBodyJsonAsync(contractTemplate, clauseId, clauseTitle);
        }

        // Renumber remaining clauses after the deleted one
        if (contractTemplateId.HasValue && deletedClauseNumber > 0)
        {
            await RenumberClausesAfterDeletionAsync(contractTemplateId.Value, deletedClauseNumber, contractTemplate);
        }

        return true;
    }

    private async Task RenumberClausesAfterDeletionAsync(int contractTemplateId, int deletedClauseNumber, ContractTemplates? contractTemplate)
    {
        // Get all remaining clauses for this contract template
        var remainingClauses = await _templateClausesRepository.GetTemplateClausesByContractTemplateIdAsync(contractTemplateId);
        
        // Find clauses with numbers greater than the deleted clause number
        var clausesToRenumber = remainingClauses
            .Where(c => 
            {
                var clauseNum = ExtractClauseNumber(c.ClauseCode ?? c.Title ?? string.Empty);
                return clauseNum > deletedClauseNumber;
            })
            .OrderBy(c => ExtractClauseNumber(c.ClauseCode ?? c.Title ?? string.Empty))
            .ToList();

        if (!clausesToRenumber.Any())
            return;

        // Reload contract template if not provided
        if (contractTemplate == null)
        {
            contractTemplate = await _contractTemplatesRepository.GetAsync(ct => ct.Id == contractTemplateId);
            if (contractTemplate == null)
                return;
        }

        // First, update all clauses in database
        var renumberMappings = new List<(TemplateClauses clause, int oldNumber, int newNumber, string oldTitle, string newTitle)>();
        
        foreach (var clause in clausesToRenumber)
        {
            var oldNumber = ExtractClauseNumber(clause.ClauseCode ?? clause.Title ?? string.Empty);
            var newNumber = oldNumber - 1;

            // Update ClauseCode and Title with new number
            var oldClauseCode = clause.ClauseCode ?? string.Empty;
            var oldTitle = clause.Title ?? string.Empty;

            clause.ClauseCode = FormatClauseCode(oldClauseCode, newNumber);
            clause.Title = FormatClauseTitle(oldTitle, newNumber);

            // Update in database
            await _templateClausesRepository.UpdateAsync(clause);
            
            // Store mapping for BodyJson update
            renumberMappings.Add((clause, oldNumber, newNumber, oldTitle, clause.Title));
        }

        // Then update BodyJson once with all renumberings
        if (!string.IsNullOrWhiteSpace(contractTemplate.BodyJson) && renumberMappings.Any())
        {
            await RenumberClausesInBodyJsonAsync(contractTemplate, renumberMappings);
        }
    }

    private async Task RenumberClausesInBodyJsonAsync(ContractTemplates contractTemplate, List<(TemplateClauses clause, int oldNumber, int newNumber, string oldTitle, string newTitle)> renumberMappings)
    {
        if (contractTemplate == null || string.IsNullOrWhiteSpace(contractTemplate.BodyJson) || !renumberMappings.Any())
            return;

        var bodyJson = contractTemplate.BodyJson;

        // Process renumberings in reverse order of clause numbers (from highest to lowest)
        // This prevents conflicts when replacing (e.g., replacing "Điều 5" before "Điều 6" prevents "Điều 6" becoming "Điều 5" accidentally)
        var sortedMappings = renumberMappings.OrderByDescending(m => m.oldNumber).ToList();

        foreach (var mapping in sortedMappings)
        {
            var clauseId = mapping.clause.Id;
            var oldNumber = mapping.oldNumber;
            var newNumber = mapping.newNumber;
            var oldTitle = mapping.oldTitle;
            var newTitle = mapping.newTitle;

            // Find the clause by data-clause-id attribute
            var clauseIdPattern = $@"data-clause-id=[""']?{clauseId}[""']?";
            var clauseIdMatch = Regex.Match(bodyJson, clauseIdPattern, RegexOptions.IgnoreCase);

            if (clauseIdMatch.Success)
            {
                // Find the clause title section and update it
                // Pattern to match: <p><strong data-clause-id="{clauseId}">Điều {oldNumber}...</strong></p>
                var titlePattern = $@"(<p>\s*<strong\s+data-clause-id=[""']?{clauseId}[""']?[^>]*>)(Điều\s+{oldNumber}\.)([^<]*)(</strong>\s*</p>)";
                var titleMatch = Regex.Match(bodyJson, titlePattern, RegexOptions.IgnoreCase);

                if (titleMatch.Success)
                {
                    // Replace the clause number in the title
                    var replacement = $"{titleMatch.Groups[1].Value}Điều {newNumber}.{titleMatch.Groups[3].Value}{titleMatch.Groups[4].Value}";
                    bodyJson = bodyJson.Substring(0, titleMatch.Index) + replacement + bodyJson.Substring(titleMatch.Index + titleMatch.Length);
                }
                else
                {
                    // Fallback: replace any occurrence of old title with new title
                    var escapedOldTitle = Regex.Escape(oldTitle);
                    var newTitleEscaped = Regex.Escape(newTitle);
                    bodyJson = Regex.Replace(bodyJson, escapedOldTitle, newTitleEscaped, RegexOptions.IgnoreCase);
                }
            }
            else
            {
                // Fallback: try to find by old title and replace
                var escapedOldTitle = Regex.Escape(oldTitle);
                var newTitleEscaped = Regex.Escape(newTitle);
                bodyJson = Regex.Replace(bodyJson, escapedOldTitle, newTitleEscaped, RegexOptions.IgnoreCase);
            }
        }

        // Update the contract template once with all changes
        contractTemplate.BodyJson = bodyJson;
        contractTemplate.UpdatedAt = DateTime.UtcNow;
        await _contractTemplatesRepository.UpdateAsync(contractTemplate);
    }

    public async Task<TemplateClausesResponse> CreateTemplateClauseAsync(int contractTemplateId, string titleOrCode, string? body = null, bool? isMandatory = false, bool? isEditable = false)
    {
        // Validate contract template exists
        var contractTemplate = await _contractTemplatesRepository.GetAsync(ct => ct.Id == contractTemplateId);
        if (contractTemplate == null)
        {
            throw new InvalidOperationException($"Contract template with ID {contractTemplateId} not found.");
        }

        // Get all existing clauses for this contract template
        var existingClauses = await _templateClausesRepository.GetTemplateClausesByContractTemplateIdAsync(contractTemplateId);
        
        // Find the maximum clause number
        int nextClauseNumber = GetNextClauseNumber(existingClauses);
        
        // Format the ClauseCode and Title
        string formattedClauseCode = FormatClauseCode(titleOrCode, nextClauseNumber);
        string formattedTitle = FormatClauseTitle(titleOrCode, nextClauseNumber);
        
        // Create the new template clause
        var newClause = new TemplateClauses
        {
            ClauseCode = formattedClauseCode,
            Title = formattedTitle,
            Body = body ?? string.Empty,
            IsMandatory = isMandatory ?? false,
            IsEditable = isEditable ?? false,
            ContractTemplatesId = contractTemplateId,
            CreatedAt = DateTime.UtcNow
        };
        
        // Save the clause to get its ID
        var createdClause = await _templateClausesRepository.AddAsync(newClause);
        
        // Update the ContractTemplate BodyJson to include the new clause
        await UpdateContractTemplateBodyJsonAsync(contractTemplate, createdClause);
        
        return _mapper.Map<TemplateClausesResponse>(createdClause);
    }

    private int GetNextClauseNumber(IEnumerable<TemplateClauses> existingClauses)
    {
        // Regex pattern to match "Điều [number]."
        var regex = new Regex(@"Điều\s+(\d+)\.", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        
        int maxNumber = 0;
        
        foreach (var clause in existingClauses)
        {
            if (string.IsNullOrWhiteSpace(clause.ClauseCode))
                continue;
                
            var match = regex.Match(clause.ClauseCode);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int clauseNumber))
            {
                if (clauseNumber > maxNumber)
                {
                    maxNumber = clauseNumber;
                }
            }
        }
        
        // Return next number (max + 1), or 1 if no clauses exist
        return maxNumber + 1;
    }

    private string FormatClauseCode(string input, int clauseNumber)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return $"Điều {clauseNumber}.";
        }
        
        var trimmedInput = input.Trim();
        
        // Check if input already starts with "Điều [number]." pattern
        var regex = new Regex(@"^Điều\s+(\d+)\.", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        var match = regex.Match(trimmedInput);
        
        if (match.Success)
        {
            // Input already has the pattern, check if it matches the expected number
            if (int.TryParse(match.Groups[1].Value, out int existingNumber))
            {
                if (existingNumber == clauseNumber)
                {
                    // Already has the correct number, return as is
                    return trimmedInput;
                }
                else
                {
                    // Has a different number, replace it with the correct one
                    return regex.Replace(trimmedInput, $"Điều {clauseNumber}.", 1);
                }
            }
        }
        
        // Input doesn't have the pattern, prepend it
        return $"Điều {clauseNumber}. {trimmedInput}";
    }

    private string FormatClauseTitle(string input, int clauseNumber)
    {
        // Title should be formatted the same way as ClauseCode
        return FormatClauseCode(input, clauseNumber);
    }

    private int ExtractClauseNumber(string clauseCodeOrTitle)
    {
        if (string.IsNullOrWhiteSpace(clauseCodeOrTitle))
            return 1; // Default to 1 if no clause number found
        
        // Regex pattern to match "Điều [number]."
        var regex = new Regex(@"Điều\s+(\d+)\.", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        var match = regex.Match(clauseCodeOrTitle);
        
        if (match.Success && int.TryParse(match.Groups[1].Value, out int clauseNumber))
        {
            return clauseNumber;
        }
        
        // If no number found, default to 1
        return 1;
    }

    private async Task UpdateContractTemplateBodyJsonAsync(ContractTemplates contractTemplate, TemplateClauses newClause)
    {
        if (contractTemplate == null || newClause == null)
            return;
        
        // Get current BodyJson (HTML content)
        var currentBody = contractTemplate.BodyJson ?? string.Empty;
        
        // Create HTML for the new clause
        // Format: <p><strong data-clause-id="{id}">Điều X. Title</strong></p> followed by body content
        // Include the clause ID as a data attribute so the template knows about the new clause
        var clauseHtml = $"\n<p><strong data-clause-id=\"{newClause.Id}\">{newClause.Title}</strong></p>\n";
        
        if (!string.IsNullOrWhiteSpace(newClause.Body))
        {
            clauseHtml += $"{newClause.Body}\n";
        }
        
        // Insert the new clause before the signature section, not after
        // If BodyJson is empty, initialize it with a div wrapper
        if (string.IsNullOrWhiteSpace(currentBody))
        {
            contractTemplate.BodyJson = $"<div>\n{clauseHtml}</div>";
        }
        else
        {
            // Find signature section markers (signature table, signature section div, or contract-signatures-section)
            // Look for common signature section patterns
            var signaturePatterns = new[]
            {
                @"<table[^>]*>\s*<tbody>\s*<tr>\s*<td>\s*<p><strong>ĐẠI DIỆN",
                @"contract-signatures-section",
                @"<!-- End Signature Section -->",
                @"<div[^>]*id=[""']?contract-signatures-section"
            };
            
            int insertPosition = -1;
            string signatureMarker = string.Empty;
            
            // Find the first occurrence of any signature marker
            foreach (var pattern in signaturePatterns)
            {
                var match = Regex.Match(currentBody, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    insertPosition = match.Index;
                    signatureMarker = match.Value;
                    break;
                }
            }
            
            // Also check for signature table by looking for "ĐẠI DIỆN" pattern
            if (insertPosition == -1)
            {
                var daiDienMatch = Regex.Match(currentBody, @"ĐẠI DIỆN", RegexOptions.IgnoreCase);
                if (daiDienMatch.Success)
                {
                    // Find the start of the table containing "ĐẠI DIỆN"
                    var beforeDaiDien = currentBody.Substring(0, daiDienMatch.Index);
                    var tableStart = beforeDaiDien.LastIndexOf("<table", StringComparison.OrdinalIgnoreCase);
                    if (tableStart != -1)
                    {
                        insertPosition = tableStart;
                    }
                }
            }
            
            if (insertPosition != -1)
            {
                // Insert clause before signature section
                var beforeSignature = currentBody.Substring(0, insertPosition).TrimEnd();
                var afterSignature = currentBody.Substring(insertPosition);
                
                // Ensure there's proper spacing before signature
                if (!beforeSignature.EndsWith("\n") && !beforeSignature.EndsWith("</p>") && !beforeSignature.EndsWith("</ol>"))
                {
                    beforeSignature += "\n";
                }
                
                contractTemplate.BodyJson = beforeSignature + clauseHtml + afterSignature;
            }
            else
            {
                // No signature section found, insert before the last closing </div> or at the end
                var lastDivIndex = currentBody.LastIndexOf("</div>", StringComparison.OrdinalIgnoreCase);
                if (lastDivIndex != -1)
                {
                    var beforeLastDiv = currentBody.Substring(0, lastDivIndex).TrimEnd();
                    var afterLastDiv = currentBody.Substring(lastDivIndex);
                    contractTemplate.BodyJson = beforeLastDiv + clauseHtml + afterLastDiv;
                }
                else
                {
                    contractTemplate.BodyJson = currentBody + clauseHtml;
                }
            }
        }
        
        // Update the contract template
        contractTemplate.UpdatedAt = DateTime.UtcNow;
        await _contractTemplatesRepository.UpdateAsync(contractTemplate);
    }

    private async Task UpdateClauseInContractTemplateBodyJsonAsync(ContractTemplates contractTemplate, TemplateClauses updatedClause, string? originalTitle, string? originalBody)
    {
        if (contractTemplate == null || updatedClause == null || string.IsNullOrWhiteSpace(contractTemplate.BodyJson))
            return;

        var bodyJson = contractTemplate.BodyJson;
        var clauseId = updatedClause.Id;

        // Pattern to match the clause section in HTML
        // We need to find: <p><strong data-clause-id="{clauseId}">...</strong></p> followed by body content until next clause or end
        
        int clauseStartIndex = -1;
        int clauseEndIndex = -1;
        
        // First, try to find clause with data-clause-id attribute
        var clauseIdPattern = $@"data-clause-id=[""']?{clauseId}[""']?";
        var clauseIdMatch = Regex.Match(bodyJson, clauseIdPattern, RegexOptions.IgnoreCase);
        
        if (clauseIdMatch.Success)
        {
            // Found by data-clause-id, find the surrounding <p><strong> tag
            var beforeClauseId = bodyJson.Substring(0, clauseIdMatch.Index);
            var paragraphStart = beforeClauseId.LastIndexOf("<p>", StringComparison.OrdinalIgnoreCase);
            if (paragraphStart == -1)
            {
                paragraphStart = beforeClauseId.LastIndexOf("<p ", StringComparison.OrdinalIgnoreCase);
            }
            
            if (paragraphStart != -1)
            {
                clauseStartIndex = paragraphStart;
            }
        }
        else
        {
            // If data-clause-id not found, try to find by title match (for backward compatibility)
            if (!string.IsNullOrWhiteSpace(originalTitle))
            {
                // Escape special regex characters in the title
                var escapedTitle = Regex.Escape(originalTitle);
                var titlePattern = $@"<p>\s*<strong[^>]*>\s*{escapedTitle}\s*</strong>\s*</p>";
                var titleMatch = Regex.Match(bodyJson, titlePattern, RegexOptions.IgnoreCase);
                if (titleMatch.Success)
                {
                    clauseStartIndex = titleMatch.Index;
                }
            }
        }
        
        if (clauseStartIndex == -1)
        {
            // Clause not found in BodyJson, cannot update
            return;
        }

        // Find the end of the current clause header (</strong></p>)
        var headerEndPattern = @"</strong>\s*</p>";
        var headerEndMatch = Regex.Match(bodyJson.Substring(clauseStartIndex), headerEndPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (!headerEndMatch.Success)
        {
            return; // Cannot find header end
        }

        var headerEndIndex = clauseStartIndex + headerEndMatch.Index + headerEndMatch.Length;
        
        // Find the end of the clause body (next clause header or end of document)
        // Look for next <p><strong>Điều pattern after current clause
        var nextClausePattern = @"<p>\s*<strong[^>]*>\s*Điều";
        var nextClauseMatch = Regex.Match(bodyJson.Substring(headerEndIndex), nextClausePattern, RegexOptions.IgnoreCase);
        
        if (nextClauseMatch.Success)
        {
            // Found next clause, end before it
            clauseEndIndex = headerEndIndex + nextClauseMatch.Index;
        }
        else
        {
            // No next clause, find the end of the document (before closing </div> if exists)
            var remainingText = bodyJson.Substring(headerEndIndex);
            var closingDivIndex = remainingText.LastIndexOf("</div>", StringComparison.OrdinalIgnoreCase);
            if (closingDivIndex > 0)
            {
                clauseEndIndex = headerEndIndex + closingDivIndex;
            }
            else
            {
                clauseEndIndex = bodyJson.Length;
            }
        }

        // Build the new clause HTML
        var newClauseHtml = $"<p><strong data-clause-id=\"{clauseId}\">{updatedClause.Title}</strong></p>\n";
        if (!string.IsNullOrWhiteSpace(updatedClause.Body))
        {
            newClauseHtml += $"{updatedClause.Body}\n";
        }

        // Replace the old clause with the new one
        var beforeClause = bodyJson.Substring(0, clauseStartIndex);
        var afterClause = bodyJson.Substring(clauseEndIndex);
        contractTemplate.BodyJson = beforeClause + newClauseHtml + afterClause;

        // Update the contract template
        contractTemplate.UpdatedAt = DateTime.UtcNow;
        await _contractTemplatesRepository.UpdateAsync(contractTemplate);
    }

    private async Task RemoveClauseFromContractTemplateBodyJsonAsync(ContractTemplates contractTemplate, int clauseId, string? clauseTitle)
    {
        if (contractTemplate == null || string.IsNullOrWhiteSpace(contractTemplate.BodyJson))
            return;

        var bodyJson = contractTemplate.BodyJson;

        // Pattern to match the clause section in HTML
        // We need to find: <p><strong data-clause-id="{clauseId}">...</strong></p> followed by body content until next clause or end
        
        int clauseStartIndex = -1;
        int clauseEndIndex = -1;
        
        // First, try to find clause with data-clause-id attribute
        var clauseIdPattern = $@"data-clause-id=[""']?{clauseId}[""']?";
        var clauseIdMatch = Regex.Match(bodyJson, clauseIdPattern, RegexOptions.IgnoreCase);
        
        if (clauseIdMatch.Success)
        {
            // Found by data-clause-id, find the surrounding <p><strong> tag
            var beforeClauseId = bodyJson.Substring(0, clauseIdMatch.Index);
            var paragraphStart = beforeClauseId.LastIndexOf("<p>", StringComparison.OrdinalIgnoreCase);
            if (paragraphStart == -1)
            {
                paragraphStart = beforeClauseId.LastIndexOf("<p ", StringComparison.OrdinalIgnoreCase);
            }
            
            if (paragraphStart != -1)
            {
                clauseStartIndex = paragraphStart;
            }
        }
        else
        {
            // If data-clause-id not found, try to find by title match (for backward compatibility)
            if (!string.IsNullOrWhiteSpace(clauseTitle))
            {
                // Escape special regex characters in the title
                var escapedTitle = Regex.Escape(clauseTitle);
                var titlePattern = $@"<p>\s*<strong[^>]*>\s*{escapedTitle}\s*</strong>\s*</p>";
                var titleMatch = Regex.Match(bodyJson, titlePattern, RegexOptions.IgnoreCase);
                if (titleMatch.Success)
                {
                    clauseStartIndex = titleMatch.Index;
                }
            }
        }
        
        if (clauseStartIndex == -1)
        {
            // Clause not found in BodyJson, nothing to remove
            return;
        }

        // Find the end of the current clause header (</strong></p>)
        var headerEndPattern = @"</strong>\s*</p>";
        var headerEndMatch = Regex.Match(bodyJson.Substring(clauseStartIndex), headerEndPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (!headerEndMatch.Success)
        {
            return; // Cannot find header end
        }

        var headerEndIndex = clauseStartIndex + headerEndMatch.Index + headerEndMatch.Length;
        
        // Find the end of the clause body (next clause header or end of document)
        // Look for next <p><strong>Điều pattern after current clause
        var nextClausePattern = @"<p>\s*<strong[^>]*>\s*Điều";
        var nextClauseMatch = Regex.Match(bodyJson.Substring(headerEndIndex), nextClausePattern, RegexOptions.IgnoreCase);
        
        if (nextClauseMatch.Success)
        {
            // Found next clause, end before it
            clauseEndIndex = headerEndIndex + nextClauseMatch.Index;
        }
        else
        {
            // No next clause, find the end of the document (before closing </div> if exists)
            var remainingText = bodyJson.Substring(headerEndIndex);
            var closingDivIndex = remainingText.LastIndexOf("</div>", StringComparison.OrdinalIgnoreCase);
            if (closingDivIndex > 0)
            {
                clauseEndIndex = headerEndIndex + closingDivIndex;
            }
            else
            {
                clauseEndIndex = bodyJson.Length;
            }
        }

        // Remove the clause by combining before and after parts
        var beforeClause = bodyJson.Substring(0, clauseStartIndex);
        var afterClause = bodyJson.Substring(clauseEndIndex);
        contractTemplate.BodyJson = beforeClause + afterClause;

        // Update the contract template
        contractTemplate.UpdatedAt = DateTime.UtcNow;
        await _contractTemplatesRepository.UpdateAsync(contractTemplate);
    }
}

