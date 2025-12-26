using System.ComponentModel;

namespace RoboRent_BE.Model.DTOs.RobotAbilityValue;

public class CreateRobotAbilityValueRequest
{
    public long RobotAbilityId { get; set; }

    public string? ValueText { get; set; }

    public string? ValueJson { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [DefaultValue(false)]
    public bool isUpdated { get; set; } = false;
}