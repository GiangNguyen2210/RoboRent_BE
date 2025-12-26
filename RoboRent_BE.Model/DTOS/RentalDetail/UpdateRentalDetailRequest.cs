using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RoboRent_BE.Model.DTOs.RobotAbilityValue;

namespace RoboRent_BE.Model.DTOS.RentalDetail;

public class UpdateRentalDetailRequest
{
    public int Id { get; set; }
    
    [DefaultValue(false)]
    public bool? IsDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    
    [Required] public int? RentalId { get; set; }
    
    [Required] public int? RoboTypeId { get; set; }
    
    public DateTime CreatedAd { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAd { get; set; } = DateTime.UtcNow;

    public bool? isLocked { get; set; }

    [DefaultValue(false)]
    public bool? IsUpdated { get; set; }

    public List<UpdateRobotAbilityValueRequest> UpdateRobotAbilityValueRequests { get; set; } =
        new List<UpdateRobotAbilityValueRequest>();
}


