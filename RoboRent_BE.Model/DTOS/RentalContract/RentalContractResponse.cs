namespace RoboRent_BE.Model.DTOS.RentalContract;

public class RentalContractResponse
{
    public int Id { get; set; }

    public string? Name { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public bool? IsDeleted { get; set; } = false;

    public string? Status { get; set; } = string.Empty;

    public int? RentalId { get; set; }

    public string? ContractUrl { get; set; } = string.Empty;

    // Navigation properties
    public string? RentalEventName { get; set; }
}
