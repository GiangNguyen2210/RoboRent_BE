using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.ContractDrafts;

public class VerifyCodeRequest
{
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string Code { get; set; } = string.Empty;
}
