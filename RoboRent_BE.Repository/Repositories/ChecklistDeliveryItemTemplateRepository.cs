using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ChecklistDeliveryItemTemplateRepository : GenericRepository<ChecklistDeliveryItemTemplate>, IChecklistDeliveryItemTemplateRepository 
{
    public ChecklistDeliveryItemTemplateRepository(AppDbContext db) : base(db)
    {
    }
}