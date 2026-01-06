using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ChecklistDeliveryItemRepository : GenericRepository<ChecklistDeliveryItem>, IChecklistDeliveryItemRepository
{
    private readonly AppDbContext _context;
    
    public ChecklistDeliveryItemRepository(AppDbContext db) : base(db)
    {
        _context = db;
    }
    
}