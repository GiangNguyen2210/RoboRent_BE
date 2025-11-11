using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.RentalDetail;

public class UpdateRentalDetailRequest
{
    public int Id { get; set; }
    [DefaultValue(false)]
    public bool? IsDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    
    [Required] public int? RentalId { get; set; }
    
    [Required] public int? RoboTypeId { get; set; }
    
    public string? Script { get; set; } =  string.Empty;

    public string? Branding { get; set; } = string.Empty;

    public string? Scenario { get; set; } =  string.Empty;
}


