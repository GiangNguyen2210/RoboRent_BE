using System.ComponentModel.DataAnnotations;

namespace RoboRent_BE.Model.DTOS.RentalDetail;

public class UpdateRentalDetailRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int? Amount { get; set; } = 0;

    public string? Script { get; set; } = string.Empty;
    
    public string? Branding { get; set; } = string.Empty;

    public string? Dance { get; set; } = string.Empty;

    public string? Music { get; set; } = string.Empty;
    
    public bool? IsDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    
    [Required]
    public int? RentalId { get; set; }
    
    [Required]
    public int? RoboTypeId { get; set; }
}


