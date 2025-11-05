using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public class CustomerContracts
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? ContractNumber { get; set; } = string.Empty;

    public string? Status { get; set; } =  string.Empty;

    public DateTime? SignedAt { get; set; } =  DateTime.UtcNow;

    public DateTime? SentAt { get; set; } =   DateTime.UtcNow;

    public string? ContractUrl { get; set; } = string.Empty;// URL to the store PDF contract

    public DateTime? CreatedAt { get; set; }  =  DateTime.UtcNow;

    public int? ContractDraftsId { get; set; }
    
    [ForeignKey("ContractDraftsId")] public virtual ContractDrafts ContractDrafts { get; set; } =  null!;

    public int? CustomerId { get; set; }
    
    [ForeignKey("CustomerId")] public virtual Account Customer { get; set; } =  null!;

    public int? ReviewerId { get; set; }
    
    [ForeignKey("ReviewerId")] public virtual Account Manager { get; set; } = null!;
}