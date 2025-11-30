using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class FaceVerificationRepository : GenericRepository<FaceVerification>, IFaceVerificationRepository
{
    private readonly AppDbContext _db;
    public FaceVerificationRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
}