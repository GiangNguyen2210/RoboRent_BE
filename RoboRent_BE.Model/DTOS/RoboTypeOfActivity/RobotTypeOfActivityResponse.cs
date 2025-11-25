namespace RoboRent_BE.Model.DTOs.RoboTypeOfActivity;

public class RobotTypeOfActivityResponse
{
    public int? ActivityTypeId { get; set; }

    public int? RoboTypeId { get; set; }

    public int? Amount { get; set; }
    
    public string? RoboTypeName { get; set; }
}