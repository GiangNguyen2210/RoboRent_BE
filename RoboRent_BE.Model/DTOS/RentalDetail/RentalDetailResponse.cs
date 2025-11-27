namespace RoboRent_BE.Model.DTOS.RentalDetail;

public class RentalDetailResponse
{
    public int Id { get; set; }
    
    public bool? IsDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    
    public int? RentalId { get; set; }
    
    public int? RoboTypeId { get; set; }
    
    public int? RobotAbilityId { get; set; }

    public string? Script { get; set; } =  string.Empty;

    public string? Branding { get; set; } = string.Empty;

    public string? Scenario { get; set; } =  string.Empty;
    
    public string? RobotTypeName { get; set; } =  string.Empty;
    
    public string? RobotTypeDescription { get; set; } =  string.Empty;
}


