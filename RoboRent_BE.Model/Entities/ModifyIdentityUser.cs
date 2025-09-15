using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboRent_BE.Model.Entities;

public partial class ModifyIdentityUser : IdentityUser
{
    public string? Status { get; set; }
}