namespace RoboRent_BE.Model.DTOs.FaceVerifications;

public class FaceVerificationsResponse
{
    public int? Id { get; set; }

    public int? AccountId { get; set; }

    public int? FaceProfileId { get; set; }
    
    public decimal? MatchScore { get; set; }     // DECIMAL(5,4)
    
    public decimal Threshold { get; set; } = 0.8000m;

    public string Result { get; set; } = string.Empty; // Success / Failed

    public int? RentalId { get; set; }

    public string? ImageEvidenceRef { get; set; }

    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
}