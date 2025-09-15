using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ModifyIdentityUserRepository : GenericRepository<ModifyIdentityUser>, IModifyIdentityUserRepository
{
    public ModifyIdentityUserRepository(AppDbContext context) : base(context)
    {
    }
}