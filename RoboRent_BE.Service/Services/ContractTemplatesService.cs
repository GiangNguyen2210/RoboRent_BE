using AutoMapper;
using RoboRent_BE.Model.DTOS.ContractTemplates;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;
using RoboRent_BE.Service.Interfaces;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace RoboRent_BE.Service.Services;

public class ContractTemplatesService : IContractTemplatesService
{
    private readonly IContractTemplatesRepository _contractTemplatesRepository;
    private readonly ITemplateClausesRepository _templateClausesRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public ContractTemplatesService(IContractTemplatesRepository contractTemplatesRepository, ITemplateClausesRepository templateClausesRepository, IMapper mapper, IConfiguration configuration, IHostEnvironment environment)
    {
        _contractTemplatesRepository = contractTemplatesRepository;
        _templateClausesRepository = templateClausesRepository;
        _mapper = mapper;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task<IEnumerable<ContractTemplatesResponse>> GetAllContractTemplatesAsync()
    {
        var contractTemplates = await _contractTemplatesRepository.GetAllWithIncludesAsync();
        return _mapper.Map<IEnumerable<ContractTemplatesResponse>>(contractTemplates);
    }

    public async Task<ContractTemplatesResponse?> GetContractTemplatesByIdAsync(int id)
    {
        var contractTemplate = await _contractTemplatesRepository.GetAsync(ct => ct.Id == id, "Created,Updated");
        if (contractTemplate == null)
            return null;

        return _mapper.Map<ContractTemplatesResponse>(contractTemplate);
    }

    public async Task<IEnumerable<ContractTemplatesResponse>> GetContractTemplatesByStatusAsync(string status)
    {
        var contractTemplates = await _contractTemplatesRepository.GetContractTemplatesByStatusAsync(status);
        return _mapper.Map<IEnumerable<ContractTemplatesResponse>>(contractTemplates);
    }

    public async Task<IEnumerable<ContractTemplatesResponse>> GetContractTemplatesByCreatedByAsync(int createdBy)
    {
        var contractTemplates = await _contractTemplatesRepository.GetContractTemplatesByCreatedByAsync(createdBy);
        return _mapper.Map<IEnumerable<ContractTemplatesResponse>>(contractTemplates);
    }

    public async Task<IEnumerable<ContractTemplatesResponse>> GetContractTemplatesByVersionAsync(string version)
    {
        var contractTemplates = await _contractTemplatesRepository.GetContractTemplatesByVersionAsync(version);
        return _mapper.Map<IEnumerable<ContractTemplatesResponse>>(contractTemplates);
    }

    public async Task<ContractTemplatesResponse> CreateContractTemplatesAsync(CreateContractTemplatesRequest request)
    {
        var contractTemplate = _mapper.Map<ContractTemplates>(request);
        
        // Capitalize status if provided, otherwise default to "Created"
        if (!string.IsNullOrWhiteSpace(contractTemplate.Status))
        {
            contractTemplate.Status = CapitalizeStatus(contractTemplate.Status);
        }
        else
        {
            contractTemplate.Status = "Created";
        }
        
        var createdContractTemplate = await _contractTemplatesRepository.AddAsync(contractTemplate);
        
        return _mapper.Map<ContractTemplatesResponse>(createdContractTemplate);
    }

    public async Task<ContractTemplatesResponse> CreateFromBodyAndGenerateClausesAsync(CreateTemplateWithParsedClausesRequest request, int createdByAccountId, string? createdByName = null)
    {
        var now = DateTime.UtcNow;
        var body = string.IsNullOrWhiteSpace(request.BodyJson)
            ? LoadDefaultBodyJson()
            : request.BodyJson;
        var contractTemplate = new ContractTemplates
        {
            TemplateCode = request.TemplateCode,
            Title = request.Title,
            Description = request.Description,
            BodyJson = body,
            Status = "Initiated",
            Version = request.Version,
            CreatedAt = now,
            CreatedBy = createdByAccountId,
            UpdatedAt = now,
            UpdatedBy = null
        };

        var created = await _contractTemplatesRepository.AddAsync(contractTemplate);

        if (!string.IsNullOrWhiteSpace(body))
        {
            foreach (var clause in ParseClausesFromBodyHtml(body))
            {
                clause.ContractTemplatesId = created.Id;
                clause.IsMandatory = true;
                clause.IsEditable = ShouldClauseBeEditable(clause.Title);
                clause.CreatedAt = now;
                await _templateClausesRepository.AddAsync(clause);
            }
        }

        return _mapper.Map<ContractTemplatesResponse>(created);
    }

    private static IEnumerable<TemplateClauses> ParseClausesFromBodyHtml(string html)
    {
        // Match headers like <p><strong>Điều ...</strong></p>
        var headerRegex = new Regex("<p>\\s*<strong>\\s*(Điều[^<]*)<\\/strong>\\s*<\\/p>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        var matches = headerRegex.Matches(html);
        if (matches.Count == 0)
        {
            yield break;
        }

        for (int i = 0; i < matches.Count; i++)
        {
            var current = matches[i];
            var startIndex = current.Index + current.Length;
            var endIndex = (i + 1 < matches.Count) ? matches[i + 1].Index : html.Length;
            if (endIndex < startIndex) endIndex = startIndex;
            var body = html.Substring(startIndex, endIndex - startIndex).Trim();
            var titleText = current.Groups[1].Value.Trim();

            yield return new TemplateClauses
            {
                ClauseCode = titleText,
                Title = titleText,
                Body = body
            };
        }
    }

    private static bool ShouldClauseBeEditable(string? title)
    {
        if (string.IsNullOrWhiteSpace(title)) return false;
        // Ensure only Điều 6 or Điều 7 are editable (avoid matching 60, 61, etc.)
        var match = Regex.Match(title, @"Điều\s*(6|7)(\D|$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return match.Success;
    }

    private string LoadDefaultBodyJson()
    {
        var configuredPath = _configuration["ContractTemplates:DefaultBodyPath"]; // optional
        var fallbackRelative = Path.Combine("RoboRent_BE.Service", "Defaults", "ContractTemplateDefaultBody.html");

        // Try configured path first
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            var resolved = ResolvePath(configuredPath);
            if (File.Exists(resolved))
            {
                return File.ReadAllText(resolved);
            }
        }

        // Try fallback relative paths
        var candidates = new List<string>
        {
            ResolvePath(fallbackRelative),
            Path.Combine(_environment.ContentRootPath, fallbackRelative),
            Path.Combine(Directory.GetParent(_environment.ContentRootPath!)?.FullName ?? _environment.ContentRootPath, fallbackRelative)
        };

        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }
        }

        return string.Empty;
    }

    private static string ResolvePath(string path)
    {
        if (Path.IsPathRooted(path)) return path;
        var baseDir = Directory.GetCurrentDirectory();
        return Path.Combine(baseDir, path);
    }

    public async Task<ContractTemplatesResponse?> UpdateContractTemplatesAsync(UpdateContractTemplatesRequest request)
    {
        var existingContractTemplate = await _contractTemplatesRepository.GetAsync(ct => ct.Id == request.Id, "Created,Updated");
        if (existingContractTemplate == null)
            return null;

        _mapper.Map(request, existingContractTemplate);
        
        // Capitalize status if provided
        if (!string.IsNullOrWhiteSpace(existingContractTemplate.Status))
        {
            existingContractTemplate.Status = CapitalizeStatus(existingContractTemplate.Status);
        }
        else
        {
            // If status is not provided in request but template exists, set to "Updated"
            existingContractTemplate.Status = "Updated";
        }
        
        existingContractTemplate.UpdatedAt = DateTime.UtcNow;
        var updatedContractTemplate = await _contractTemplatesRepository.UpdateAsync(existingContractTemplate);
        
        return _mapper.Map<ContractTemplatesResponse>(updatedContractTemplate);
    }

    private static string CapitalizeStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return status;
        
        // Capitalize first letter and make rest lowercase
        return char.ToUpperInvariant(status[0]) + (status.Length > 1 ? status.Substring(1).ToLowerInvariant() : string.Empty);
    }

    public async Task<bool> DeleteContractTemplatesAsync(int id)
    {
        var contractTemplate = await _contractTemplatesRepository.GetAsync(ct => ct.Id == id);
        if (contractTemplate == null)
            return false;

        // Instead of deleting, set status to "Disabled"
        contractTemplate.Status = "Disabled";
        contractTemplate.UpdatedAt = DateTime.UtcNow;
        await _contractTemplatesRepository.UpdateAsync(contractTemplate);
        
        return true;
    }

    public async Task<ContractTemplatesResponse?> ActivateContractTemplateAsync(int id)
    {
        var contractTemplate = await _contractTemplatesRepository.GetAsync(ct => ct.Id == id, "Created,Updated");
        if (contractTemplate == null)
            return null;

        // Check if contract template is disabled
        if (contractTemplate.Status != "Disabled" && contractTemplate.Status != "disabled")
        {
            throw new InvalidOperationException("Only disabled contract templates can be activated.");
        }

        // Set status to "Updated" when activating
        contractTemplate.Status = "Updated";
        contractTemplate.UpdatedAt = DateTime.UtcNow;
        var updatedContractTemplate = await _contractTemplatesRepository.UpdateAsync(contractTemplate);
        
        return _mapper.Map<ContractTemplatesResponse>(updatedContractTemplate);
    }
}

