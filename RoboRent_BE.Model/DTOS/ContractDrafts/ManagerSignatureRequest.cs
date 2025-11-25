using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.ContractDrafts;

public class ManagerSignatureRequest
{
    [Required]
    public string Signature { get; set; } = string.Empty;
}

