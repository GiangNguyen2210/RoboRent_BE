using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class TypesOfRoboRepository : GenericRepository<TypesOfRobo>, ITypesOfRoboRepository
{
    public TypesOfRoboRepository(AppDbContext context) : base(context)
    {
    }
}