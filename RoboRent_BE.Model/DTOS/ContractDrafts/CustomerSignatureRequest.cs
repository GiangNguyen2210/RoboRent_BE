using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.ContractDrafts;

public class CustomerSignatureRequest
{
    [Required]
    public string Signature { get; set; } = string.Empty;
}

