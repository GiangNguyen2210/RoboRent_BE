using RoboRent_BE.Model.DTOs.RobotAbility;

namespace RoboRent_BE.Model.DTOs.RoboTypeOfActivity;

public class RobotTypeOfActivityResponse
{
    public int? ActivityTypeId { get; set; }

    public int? RoboTypeId { get; set; }

    public int? Amount { get; set; }
    
    public string? RoboTypeName { get; set; }

    public List<RobotAbilityResponse> RobotAbilityResponses { get; set; }
}