using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

    public class RobotAbility
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int RobotTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = null!;

        [Required]
        public string Label { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string DataType { get; set; } = null!;

        public bool IsRequired { get; set; } = false;

        [MaxLength(50)]
        public string? AbilityGroup { get; set; }
        
        // Business Rules
        public bool LockAtCutoff { get; set; } = true;
        public bool IsPriceImpacting { get; set; } = false;
        public bool IsOnSiteAdjustable { get; set; } = false;

        // UI hints
        [MaxLength(30)]
        public string? UiControl { get; set; }

        public string? Placeholder { get; set; }

        // Validation
        [Column(TypeName = "numeric")]
        public decimal? Min { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Max { get; set; }

        public int? MaxLength { get; set; }

        public string? Regex { get; set; }

        // JSON
        [Column(TypeName = "jsonb")]
        public string? OptionsJson { get; set; }

        [Column(TypeName = "jsonb")]
        public string? JsonSchema { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        [ForeignKey("RobotTypeId")]
        public RoboType RoboType { get; set; } = null!;
    }

