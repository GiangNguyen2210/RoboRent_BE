using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RoboRent_BE.Model.DTOs.RobotAbilityValue;

namespace RoboRent_BE.Model.DTOS.RentalDetail;

public class CreateRentalDetailRequest
{
    [DefaultValue(false)]
    public bool? IsDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    
    [Required] public int? RentalId { get; set; }
    
    [Required] public int? RoboTypeId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool? isLocked { get; set; }

    public List<CreateRobotAbilityValueRequest> CreateRobotAbilityValueRequests { get; set; } =
        new List<CreateRobotAbilityValueRequest>();
}


