using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOs.Chat;

public class UpdateMessageStatusRequest
{
    [Required]
    [RegularExpression("^(Accepted|Rejected)$")]
    public string Status { get; set; } = string.Empty;
}