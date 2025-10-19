using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository.Interfaces;

namespace RoboRent_BE.Repository.Repositories;

public class PriceQuoteRepository : GenericRepository<PriceQuote>, IPriceQuoteRepository
{
    public PriceQuoteRepository(AppDbContext context) : base(context)
    {
    }
}