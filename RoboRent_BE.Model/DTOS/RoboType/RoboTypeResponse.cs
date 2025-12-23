using RoboRent_BE.Model.DTOs.RobotAbility;

namespace RoboRent_BE.Model.DTOs.RoboType;

public class RoboTypeResponse
{
    public int Id { get; set; }
    
    public string? TypeName { get; set; } =  string.Empty;
    
    public string? Description { get; set; } =  string.Empty;
    
    public bool? IsDeleted { get; set; } =  false;
    
    public List<RobotAbilityResponse>? RobotAbilityResponses { get; set; } = new List<RobotAbilityResponse>();
}