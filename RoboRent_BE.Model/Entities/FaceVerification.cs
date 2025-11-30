using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class FaceVerification
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int? Id { get; set; }

    public int? AccountId { get; set; }
    
    [ForeignKey("AccountId")] public virtual Account Account { get; set; } =  null!;

    public int? FaceProfileId { get; set; }
    
    [ForeignKey("FaceProfileId")] public virtual FaceProfiles FaceProfiles { get; set; } =  null!;
    
    // ------------------------------
    // Matching
    // ------------------------------
    public decimal? MatchScore { get; set; }     // DECIMAL(5,4)
    public decimal Threshold { get; set; } = 0.8000m;

    [Required]
    [MaxLength(20)]
    public string Result { get; set; } = string.Empty; // Success / Failed

    // ------------------------------
    // Linked entities
    // ------------------------------
    public int? RentalId { get; set; }
    [ForeignKey("RentalId")] public virtual Rental Rental { get; set; } = null!;

    // ------------------------------
    // Evidence
    // ------------------------------
    [MaxLength(200)]
    public string? ImageEvidenceRef { get; set; }

    // ------------------------------
    // Verified time
    // ------------------------------
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
}