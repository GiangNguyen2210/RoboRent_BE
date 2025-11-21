using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.ContractDrafts;

public class UpdateContractDraftsRequest
{
    [Required]
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? BodyJson { get; set; }

    public string? Comments { get; set; }
}
