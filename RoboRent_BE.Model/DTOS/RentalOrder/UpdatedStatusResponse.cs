using RoboRent_BE.Model.DTOs.RentalChangeLog;
using RoboRent_BE.Model.DTOs.RobotAbilityValue;

namespace RoboRent_BE.Model.DTOS.RentalOrder;

public class UpdatedStatusResponse
{
    public bool? RentalIsUpdated { get; set; } = false;

    public bool? RentalDetailIsUpdated { get; set; } = false;
    
    public List<RentalChangeLogResponse> RentalChangeLogResponses { get; set; } =  new List<RentalChangeLogResponse>();

    public List<RobotAbilityValueResponse> RobotAbilityValueResponses { get; set; } =  new List<RobotAbilityValueResponse>();
}