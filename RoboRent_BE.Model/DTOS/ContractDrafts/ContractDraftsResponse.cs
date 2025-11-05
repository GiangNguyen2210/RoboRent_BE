namespace RoboRent_BE.Model.DTOS.ContractDrafts;

public class ContractDraftsResponse
{
    public int Id { get; set; }

    public string? Title { get; set; } = string.Empty;

    public string? BodyJson { get; set; } = string.Empty;

    public string? Comments { get; set; } = string.Empty;

    public string? Status { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? ContractTemplatesId { get; set; }

    public int? RentalId { get; set; }

    public int? StaffId { get; set; }

    public int? ManagerId { get; set; }

    // Navigation properties
    public string? ContractTemplateTitle { get; set; }
    public string? RentalEventName { get; set; }
    public string? StaffName { get; set; }
    public string? ManagerName { get; set; }
}
