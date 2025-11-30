using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class FaceProfiles
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public int? AccountId { get; set; }

    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;
    
    // ------------------------------
    // CitizenId (NVARCHAR 20)
    // ------------------------------
    [Required]
    [MaxLength(20)]
    public string CitizenId { get; set; } = string.Empty;

    // ------------------------------
    // Embedding VARBINARY(MAX)
    // ------------------------------
    [Required]
    public string Embedding { get; set; } = string.Empty;

    // ------------------------------
    // Model NVARCHAR(50)
    // ------------------------------
    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = "dlib_resnet";

    // ------------------------------
    // Hash SHA256 - 64 CHAR
    // ------------------------------
    [Required]
    [StringLength(64)]
    public string HashSha256 { get; set; } = string.Empty;

    // ------------------------------
    // Image path (front side of ID)
    // ------------------------------
    [MaxLength(500)]
    public string? FrontIdImagePath { get; set; }

    // ------------------------------
    // Timestamps
    // ------------------------------
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastUsedAt { get; set; }

    // ------------------------------
    // Active flag
    // ------------------------------
    public bool IsActive { get; set; } = true;
}