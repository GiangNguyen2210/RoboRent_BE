namespace RoboRent_BE.Model.DTOs.FaceProfiles;

public class FaceProfilesResponse
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public string CitizenId { get; set; } = string.Empty;
    
    public string Model { get; set; } = "dlib_resnet";
    
    public string HashSha256 { get; set; } = string.Empty;
    
    public string? FrontIdImagePath { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastUsedAt { get; set; }
    
    public bool IsActive { get; set; } = true;
}