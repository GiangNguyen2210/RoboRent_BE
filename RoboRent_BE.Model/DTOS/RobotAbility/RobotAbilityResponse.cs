namespace RoboRent_BE.Model.DTOs.RobotAbility;

public class RobotAbilityResponse
{
    public long Id { get; set; }

    public int RobotTypeId { get; set; }

    public string Key { get; set; } = null!;

    public string Label { get; set; } = null!;

    public string? Description { get; set; }

    public string DataType { get; set; } = null!;

    public bool IsRequired { get; set; } = false;

    public string? AbilityGroup { get; set; }
        
    public bool LockAtCutoff { get; set; } = true;
    public bool IsPriceImpacting { get; set; } = false;
    public bool IsOnSiteAdjustable { get; set; } = false;

    // UI hints
    public string? UiControl { get; set; }

    public string? Placeholder { get; set; }

    // Validation
    public decimal? Min { get; set; }

    public decimal? Max { get; set; }

    public int? MaxLength { get; set; }

    public string? Regex { get; set; }

    // JSON
    public string? OptionsJson { get; set; }

    public string? JsonSchema { get; set; }

    public bool IsActive { get; set; } = true;
}