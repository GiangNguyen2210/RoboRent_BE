using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class RobotAbilityValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int RentalDetailId { get; set; }

        [Required]
        public int RobotAbilityId { get; set; }

        // Store value
        public string? ValueText { get; set; }

        [Column(TypeName = "jsonb")]
        public string? ValueJson { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool isUpdated { get; set; } = false;
        // Navigation
        [ForeignKey("RentalDetailId")]
        public RentalDetail RentalDetail { get; set; } = null!;
        [ForeignKey("RobotAbilityId")]
        public RobotAbility RobotAbility { get; set; } = null!;
    }

