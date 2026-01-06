using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ChecklistDeliveryEvidenceRepository : GenericRepository<ChecklistDeliveryEvidence>, IChecklistDeliveryEvidenceRepository
{
    public ChecklistDeliveryEvidenceRepository(AppDbContext db) : base(db)
    {
    }
}