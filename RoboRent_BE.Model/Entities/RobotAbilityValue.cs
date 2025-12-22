using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class RobotAbilityValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long RentalDetailId { get; set; }

        [Required]
        public long RobotAbilityId { get; set; }

        // Store value
        public string? ValueText { get; set; }

        [Column(TypeName = "jsonb")]
        public string? ValueJson { get; set; }

        [Required]
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        [ForeignKey("RentalDetailId")]
        public RentalDetail RentalDetail { get; set; } = null!;
        [ForeignKey("RobotAbilityId")]
        public RobotAbility RobotAbility { get; set; } = null!;
    }

