namespace RoboRent_BE.Model.DTOs.RobotAbilityValue;

public class RobotAbilityValueResponse
{
    public int Id { get; set; }

    public int RentalDetailId { get; set; }

    public int RobotAbilityId { get; set; }

    public string? ValueText { get; set; }

    public string? ValueJson { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool isUpdated { get; set; } = false;
}