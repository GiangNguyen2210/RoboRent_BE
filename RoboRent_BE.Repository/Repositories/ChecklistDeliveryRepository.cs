using RoboRent_BE.Model.DTOs.ChecklistDelivery;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class ChecklistDeliveryRepository : GenericRepository<ChecklistDelivery>, IChecklistDeliveryRepository
{
    private readonly AppDbContext _context;
    public ChecklistDeliveryRepository(AppDbContext db) : base(db)
    {
        _context = db;
    }
}