using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RoboRent_BE.Model.Entities;

public partial class AppDbContext : IdentityDbContext<ModifyIdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}
    
    public virtual DbSet<Company>  Companies { get; set; } = null!;
    
    public virtual DbSet<Account> Accounts { get; set; } = null!;
    
    public virtual DbSet<Robot> Robots { get; set; } = null!;
    
    public virtual DbSet<RoboType> RoboTypes { get; set; } = null!;
    
    public virtual DbSet<TypesOfRobo> TypesOfRobos { get; set; } = null!;
    
    public virtual DbSet<Event> Events { get; set; } = null!;
    
    public virtual DbSet<EventRoboType>  EventRoboTypes { get; set; } = null!;
    
    public virtual DbSet<Rental> Rentals { get; set; } = null!;
    
    public virtual DbSet<RentalDetail>  RentalDetails { get; set; } = null!;
    
    public virtual DbSet<PriceQuote>  PriceQuotes { get; set; } = null!;
    
    public virtual DbSet<PaymentTransaction>  PaymentTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventRoboType>().HasKey(ert => new { ert.EventId, ert.RoboTypeId });
        
        modelBuilder.Entity<TypesOfRobo>().HasKey(tor => new { tor.RobotId, tor.RoboTypeId });
        
        base.OnModelCreating(modelBuilder);
    }
}