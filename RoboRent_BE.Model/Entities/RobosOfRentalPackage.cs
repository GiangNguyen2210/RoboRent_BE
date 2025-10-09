using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class RobosOfRentalPackage
{
    public int RentalPackageId { get; set; }
    
    public int RoboTypeId { get; set; }
    
    public bool? IsDeleted { get; set; } =  false;
    
    [ForeignKey("RoboTypeId")] public virtual RoboType RoboType { get; set; } = null!;
    
    [ForeignKey("RentalPackageId")] public virtual RentalPackage RentalPackage { get; set; } = null!;
}