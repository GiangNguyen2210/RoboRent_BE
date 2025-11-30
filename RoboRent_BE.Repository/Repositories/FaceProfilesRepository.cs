using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class FaceProfilesRepository : GenericRepository<FaceProfiles> ,IFaceProfilesRepository
{
    private readonly AppDbContext _dbContext;
    public FaceProfilesRepository(AppDbContext db) : base(db)
    {
        _dbContext = db;
    }
}