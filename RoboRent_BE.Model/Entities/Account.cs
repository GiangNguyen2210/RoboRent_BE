using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? FullName { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; } = string.Empty;
    
    public DateTime? DateOfBirth { get; set; } = DateTime.UtcNow;
    
    public string? gender { get; set; } = string.Empty;
    
    public bool? IdentificationIsValidated  { get; set; } = false;
    
    public bool? isDeleted { get; set; } = false;
    
    public string? Status { get; set; } = string.Empty;
    
    [Required] public string? UserId { get; set; } = string.Empty;
    
    [ForeignKey("UserId")] public virtual ModifyIdentityUser ModifyIdentityUser { get; set; } = null!;
}