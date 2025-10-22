using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.RentalContract;

public class CreateRentalContractRequest
{
    [Required]
    public string? Name { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public bool? IsDeleted { get; set; } = false;

    public string? Status { get; set; } = string.Empty;

    [Required]
    public int? RentalId { get; set; }

    public string? ContractUrl { get; set; } = string.Empty;
}
