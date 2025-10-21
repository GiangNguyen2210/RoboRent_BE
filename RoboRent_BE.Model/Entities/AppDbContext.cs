using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RoboRent_BE.Model.Entities;

public partial class AppDbContext : IdentityDbContext<ModifyIdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}
    public virtual DbSet<RentalContract> RentalContracts { get; set; } = null!;
    
    public virtual DbSet<EventSchedule> EventSchedules { get; set; } = null!;
    
    public virtual DbSet<RentalPackage> RentalPackages { get; set; } = null!;
    
    public virtual DbSet<RobosOfRentalPackage> RobosOfRentalPackages { get; set; } = null!;
    
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
    public virtual DbSet<ChatRoom> ChatRooms { get; set; } = null!;
    public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;

    public virtual DbSet<ContractDrafts> ContractDrafts { get; set; } = null!;

    public virtual DbSet<ContractReports> ContractReports { get; set; } = null!;

    public virtual DbSet<ContractTemplates> ContractTemplates { get; set; } = null!;

    public virtual DbSet<CustomerContracts> CustomerContracts { get; set; } =  null!;

    public virtual DbSet<DraftApprovals> DraftApprovals { get; set; }  = null!;

    public virtual DbSet<DraftClauses> DraftClauses { get; set; } = null!;

    public virtual DbSet<TemplateClauses> TemplateClauses { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "Staff", NormalizedName = "STAFF" },
            new IdentityRole { Id = "3", Name = "Customer", NormalizedName = "CUSTOMER" },
            new IdentityRole { Id = "4", Name = "Manager", NormalizedName = "MANAGER" }
            );

        modelBuilder.Entity<Event>().HasData(
            new  Event {Id = 1, EventName = "conferences", IsDeleted = false},
            new  Event {Id = 2, EventName = "seminars", IsDeleted = false},
            new  Event {Id = 3, EventName = "workshops", IsDeleted = false},
            new  Event {Id = 4, EventName = "product launches", IsDeleted = false},
            new  Event {Id = 5, EventName = "weddings", IsDeleted = false},
            new  Event {Id = 6, EventName = "birthdays", IsDeleted = false},
            new  Event {Id = 7, EventName = "anniversaries", IsDeleted = false},
            new  Event {Id = 8, EventName = "festivals", IsDeleted = false},
            new  Event {Id = 9, EventName = "exhibitions", IsDeleted = false},
            new  Event {Id = 10, EventName = "concerts", IsDeleted = false}
            );

        modelBuilder.Entity<RentalPackage>().HasData(
            new RentalPackage {Id = 1, Name = "a", Description = "a", IsDeleted = false, Status = "Active"},
            new RentalPackage {Id = 2, Name = "b", Description = "b", IsDeleted = false, Status = "Active"},
            new RentalPackage {Id = 3, Name = "c", Description = "c", IsDeleted = false, Status = "Active"},
            new RentalPackage {Id = 4, Name = "d", Description = "d", IsDeleted = false, Status = "Active"},
            new RentalPackage {Id = 5, Name = "e", Description = "e", IsDeleted = false, Status = "Active"}
            );
        
        modelBuilder.Entity<EventRoboType>().HasKey(ert => new { ert.EventId, ert.RoboTypeId });
        
        modelBuilder.Entity<TypesOfRobo>().HasKey(tor => new { tor.RobotId, tor.RoboTypeId });

        modelBuilder.Entity<RobosOfRentalPackage>().HasKey(rorp => new { rorp.RentalPackageId, rorp.RoboTypeId });
        
        base.OnModelCreating(modelBuilder);
    }
}