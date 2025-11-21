using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.ContractDrafts;

public class CreateContractDraftsRequest
{
    public string? Title { get; set; } = string.Empty;

    public string? BodyJson { get; set; } = string.Empty;

    public string? Comments { get; set; } = string.Empty;

    public string? Status { get; set; } = string.Empty;

    public int? ContractTemplatesId { get; set; }

    [Required]
    public int? RentalId { get; set; }

    public int? StaffId { get; set; }

    public int? ManagerId { get; set; }
}
