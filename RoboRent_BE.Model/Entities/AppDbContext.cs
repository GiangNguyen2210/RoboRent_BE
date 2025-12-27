using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RoboRent_BE.Model.Entities;

public partial class AppDbContext : IdentityDbContext<
    ModifyIdentityUser
    >
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<RentalContract> RentalContracts { get; set; } = null!;

    // public virtual DbSet<EventActivity> EventActivities { get; set; } = null!;
    
    public virtual DbSet<FaceProfiles>  FaceProfiles { get; set; } = null!;
    
    public virtual DbSet<FaceVerification>  FaceVerifications { get; set; } = null!;

    public virtual DbSet<Account> Accounts { get; set; } = null!;

    public virtual DbSet<Robot> Robots { get; set; } = null!;

    public virtual DbSet<RoboType> RoboTypes { get; set; } = null!;

    public virtual DbSet<TypesOfRobo> TypesOfRobos { get; set; } = null!;

    public virtual DbSet<ActivityType> ActivityTypes { get; set; } = null!;

    public virtual DbSet<Rental> Rentals { get; set; } = null!;

    public virtual DbSet<RentalDetail> RentalDetails { get; set; } = null!;

    public virtual DbSet<PriceQuote> PriceQuotes { get; set; } = null!;
    public virtual DbSet<ChatRoom> ChatRooms { get; set; } = null!;
    public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;

    public virtual DbSet<ContractDrafts> ContractDrafts { get; set; } = null!;

    public virtual DbSet<ContractReports> ContractReports { get; set; } = null!;

    public virtual DbSet<ContractTemplates> ContractTemplates { get; set; } = null!;

    public virtual DbSet<CustomerContracts> CustomerContracts { get; set; } = null!;

    public virtual DbSet<DraftApprovals> DraftApprovals { get; set; } = null!;

    public virtual DbSet<DraftClauses> DraftClauses { get; set; } = null!;

    public virtual DbSet<TemplateClauses> TemplateClauses { get; set; } = null!;

    public virtual DbSet<ActivityTypeGroup> ActivityTypeGroups { get; set; } = null!;

    public virtual DbSet<GroupSchedule> GroupSchedules { get; set; } = null!;

    public virtual DbSet<RobotInGroup> RobotInGroups { get; set; } = null!;

    public virtual DbSet<RobotTypeOfActivity> RobotTypeOfEvents { get; set; } = null!;

    public virtual DbSet<RobotAbility> RobotAbilities { get; set; } = null!;
    public virtual DbSet<ActualDelivery> ActualDeliveries { get; set; } = null!;
    public virtual DbSet<RobotAbilityValue> RobotAbilityValues { get; set; } = null!;
    public virtual DbSet<PaymentRecord> PaymentRecords { get; set; } = null!;
    public virtual DbSet<RentalChangeLog> RentalChangeLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "Staff", NormalizedName = "STAFF" },
            new IdentityRole { Id = "3", Name = "Customer", NormalizedName = "CUSTOMER" },
            new IdentityRole { Id = "4", Name = "Manager", NormalizedName = "MANAGER" }
        );

        modelBuilder.Entity<ModifyIdentityUser>().HasData(
            new ModifyIdentityUser
            {
                Id = "5373bf8f-51b2-4c2d-b832-be15aedd63bc",
                Status = "Active",
                UserName = "xuant0343@gmail.com",
                NormalizedUserName = "XUANT0343@GMAIL.COM",
                Email = "xuant0343@gmail.com",
                NormalizedEmail = "XUANT0343@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = "TO6KVHYLEHG2KLGZPJVPI6NSP5FJBT5V",
                ConcurrencyStamp = "0b266393-f928-4c62-8d31-b7f4e8b884f4",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            },
            new ModifyIdentityUser
            {
                Id = "47ebcce9-fd0c-4173-91f4-a25385622d21",
                Status = "Active",
                UserName = "giangntse183662@fpt.edu.vn",
                NormalizedUserName = "GIANGNTSE183662@FPT.EDU.VN",
                Email = "giangntse183662@fpt.edu.vn",
                NormalizedEmail = "GIANGNTSE183662@FPT.EDU.VN",
                EmailConfirmed = true,
                SecurityStamp = "FRUURT5J22RQ24DHPTJOV27KBO6YSUUG",
                ConcurrencyStamp = "06625a65-14d1-4822-aaf1-2643d61d246b",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            },
            new ModifyIdentityUser
            {
                Id = "fa56f53b-f406-42c4-afdc-f12a1a210b4b",
                Status = "Active",
                UserName = "giangnguyen102004@gmail.com",
                NormalizedUserName = "GIANGNGUYEN102004@GMAIL.COM",
                Email = "giangnguyen102004@gmail.com",
                NormalizedEmail = "GIANGNGUYEN102004@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = "VPYTFD22TZD5DQECWLL62UFQVBZM6T4C",
                ConcurrencyStamp = "e57cf66c-36b0-47b3-a7bc-26bb276298a4",
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            }
            );

        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                Id = 1,
                FullName = "xuan truong",
                PhoneNumber = string.Empty,
                DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                gender = string.Empty,
                IdentificationIsValidated = false,
                isDeleted = false,
                Status = "Active",
                UserId = "5373bf8f-51b2-4c2d-b832-be15aedd63bc"
            },
            new Account
            {
                Id = 2,
                FullName = "Nguyen Truong Giang (K18 HCM)",
                PhoneNumber = string.Empty,
                DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                gender = string.Empty,
                IdentificationIsValidated = false,
                isDeleted = false,
                Status = "Active",
                UserId = "47ebcce9-fd0c-4173-91f4-a25385622d21"
            },
            new Account
            {
                Id = 3,
                FullName = "Trường Giang Nguyễn",
                PhoneNumber = string.Empty,
                DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                gender = string.Empty,
                IdentificationIsValidated = false,
                isDeleted = false,
                Status = "Active",
                UserId = "fa56f53b-f406-42c4-afdc-f12a1a210b4b"
            }
            );

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = "5373bf8f-51b2-4c2d-b832-be15aedd63bc",
                RoleId = "3"
            },
            new IdentityUserRole<string>
            {
                UserId = "fa56f53b-f406-42c4-afdc-f12a1a210b4b",
                RoleId = "4"
            },
            new IdentityUserRole<string>
            {
                UserId = "47ebcce9-fd0c-4173-91f4-a25385622d21",
                RoleId = "2"
            }
            );

        modelBuilder.Entity<IdentityUserLogin<string>>().HasData(
            new IdentityUserLogin<string>
            {
                UserId = "5373bf8f-51b2-4c2d-b832-be15aedd63bc",
                LoginProvider = "Google",
                ProviderKey = "101135697127238020611",
                ProviderDisplayName = "Google"
            },
            new IdentityUserLogin<string>
            {
                UserId = "47ebcce9-fd0c-4173-91f4-a25385622d21",
                LoginProvider = "Google",
                ProviderKey = "111410206604030881459",
                ProviderDisplayName = "Google"
            },
            new IdentityUserLogin<string>
            {
                UserId = "fa56f53b-f406-42c4-afdc-f12a1a210b4b",
                LoginProvider = "Google",
                ProviderKey = "116621369845429820359",
                ProviderDisplayName = "Google"
            }
            );

        

        // modelBuilder.Entity<EventActivity>().HasData(
        //     new EventActivity
        //         { Id = 1, Name = "Performance", Description = "Robot biểu diễn trước khán giả", IsDeleted = false },
        //     new EventActivity
        //     {
        //         Id = 2, Name = "Presentation & Hosting", Description = "Robot giao tiếp, nói, dẫn chương trình",
        //         IsDeleted = false
        //     },
        //     new EventActivity
        //     {
        //         Id = 3, Name = "Storytelling & Interaction", Description = "Robot tương tác nội dung với khán giả",
        //         IsDeleted = false
        //     },
        //     new EventActivity
        //     {
        //         Id = 4, Name = "Entertainment Support",
        //         Description = "Robot hỗ trợ event nhưng không phải “tiết mục biểu diễn chính”", IsDeleted = false
        //     },
        //     new EventActivity
        //     {
        //         Id = 5, Name = "Promotion & Marketing", Description = "Hoạt động marketing, bán hàng, PR",
        //         IsDeleted = false
        //     }
        // );

        modelBuilder.Entity<RoboType>().HasData(
            new RoboType
            {
                Id = 1,
                TypeName = "Reception Robot",
                Description = "Robot lễ tân, chào đón khách, hỗ trợ check-in, hướng dẫn khu vực và hiển thị nội dung thương hiệu",
                IsDeleted = false
            },
            new RoboType
            {
                Id = 2,
                TypeName = "Performance Robot",
                Description = "Robot biểu diễn sân khấu, nhảy múa, trình diễn theo show set và kịch bản được cấu hình",
                IsDeleted = false
            },
            new RoboType
            {
                Id = 3,
                TypeName = "Host Robot",
                Description = "Robot dẫn chương trình (MC), storytelling, đọc kịch bản theo timeline và tương tác với khán giả",
                IsDeleted = false
            },
            new RoboType
            {
                Id = 4,
                TypeName = "Promotion Robot",
                Description = "Robot quảng bá thương hiệu, hỗ trợ booth, chạy nội dung marketing, CTA và thu hút khách tham dự",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Robot>().HasData(

    // =====================================================
    // PACKAGE: BASIC (16 robots)
    // =====================================================

    // RECEPTION (4) - RoboTypeId = 11
    new Robot { Id = 1,  RoboTypeId = 1, RobotName = "REC-BASIC-01", ModelName = "REC-BASIC-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 2,  RoboTypeId = 1, RobotName = "REC-BASIC-02", ModelName = "REC-BASIC-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 3,  RoboTypeId = 1, RobotName = "REC-BASIC-03", ModelName = "REC-BASIC-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 4,  RoboTypeId = 1, RobotName = "REC-BASIC-04", ModelName = "REC-BASIC-04", RobotStatus = "Stored", IsDeleted = false },

    // PERFORMANCE (4) - RoboTypeId = 1
    new Robot { Id = 5,  RoboTypeId = 2, RobotName = "PERF-BASIC-01", ModelName = "PERF-BASIC-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 6,  RoboTypeId = 2, RobotName = "PERF-BASIC-02", ModelName = "PERF-BASIC-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 7,  RoboTypeId = 2, RobotName = "PERF-BASIC-03", ModelName = "PERF-BASIC-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 8,  RoboTypeId = 2, RobotName = "PERF-BASIC-04", ModelName = "PERF-BASIC-04", RobotStatus = "Stored", IsDeleted = false },

    // HOST (4) - RoboTypeId = 5
    new Robot { Id = 9,  RoboTypeId = 3, RobotName = "HOST-BASIC-01", ModelName = "HOST-BASIC-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 10, RoboTypeId = 3, RobotName = "HOST-BASIC-02", ModelName = "HOST-BASIC-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 11, RoboTypeId = 3, RobotName = "HOST-BASIC-03", ModelName = "HOST-BASIC-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 12, RoboTypeId = 3, RobotName = "HOST-BASIC-04", ModelName = "HOST-BASIC-04", RobotStatus = "Stored", IsDeleted = false },

    // PROMOTION (4) - RoboTypeId = 14
    new Robot { Id = 13, RoboTypeId = 4, RobotName = "PROMO-BASIC-01", ModelName = "PROMO-BASIC-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 14, RoboTypeId = 4, RobotName = "PROMO-BASIC-02", ModelName = "PROMO-BASIC-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 15, RoboTypeId = 4, RobotName = "PROMO-BASIC-03", ModelName = "PROMO-BASIC-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 16, RoboTypeId = 4, RobotName = "PROMO-BASIC-04", ModelName = "PROMO-BASIC-04", RobotStatus = "Stored", IsDeleted = false },

    // =====================================================
    // PACKAGE: STANDARD (24 robots)
    // =====================================================

    // RECEPTION (4)
    new Robot { Id = 17, RoboTypeId = 1, RobotName = "REC-STANDARD-01", ModelName = "REC-STANDARD-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 18, RoboTypeId = 1, RobotName = "REC-STANDARD-02", ModelName = "REC-STANDARD-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 19, RoboTypeId = 1, RobotName = "REC-STANDARD-03", ModelName = "REC-STANDARD-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 20, RoboTypeId = 1, RobotName = "REC-STANDARD-04", ModelName = "REC-STANDARD-04", RobotStatus = "Stored", IsDeleted = false },

    // PERFORMANCE (8)
    new Robot { Id = 21, RoboTypeId = 2, RobotName = "PERF-STANDARD-01", ModelName = "PERF-STANDARD-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 22, RoboTypeId = 2, RobotName = "PERF-STANDARD-02", ModelName = "PERF-STANDARD-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 23, RoboTypeId = 2, RobotName = "PERF-STANDARD-03", ModelName = "PERF-STANDARD-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 24, RoboTypeId = 2, RobotName = "PERF-STANDARD-04", ModelName = "PERF-STANDARD-04", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 25, RoboTypeId = 2, RobotName = "PERF-STANDARD-05", ModelName = "PERF-STANDARD-05", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 26, RoboTypeId = 2, RobotName = "PERF-STANDARD-06", ModelName = "PERF-STANDARD-06", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 27, RoboTypeId = 2, RobotName = "PERF-STANDARD-07", ModelName = "PERF-STANDARD-07", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 28, RoboTypeId = 2, RobotName = "PERF-STANDARD-08", ModelName = "PERF-STANDARD-08", RobotStatus = "Stored", IsDeleted = false },

    // HOST (4)
    new Robot { Id = 29, RoboTypeId = 3, RobotName = "HOST-STANDARD-01", ModelName = "HOST-STANDARD-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 30, RoboTypeId = 3, RobotName = "HOST-STANDARD-02", ModelName = "HOST-STANDARD-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 31, RoboTypeId = 3, RobotName = "HOST-STANDARD-03", ModelName = "HOST-STANDARD-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 32, RoboTypeId = 3, RobotName = "HOST-STANDARD-04", ModelName = "HOST-STANDARD-04", RobotStatus = "Stored", IsDeleted = false },

    // PROMOTION (8)
    new Robot { Id = 33, RoboTypeId = 4, RobotName = "PROMO-STANDARD-01", ModelName = "PROMO-STANDARD-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 34, RoboTypeId = 4, RobotName = "PROMO-STANDARD-02", ModelName = "PROMO-STANDARD-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 35, RoboTypeId = 4, RobotName = "PROMO-STANDARD-03", ModelName = "PROMO-STANDARD-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 36, RoboTypeId = 4, RobotName = "PROMO-STANDARD-04", ModelName = "PROMO-STANDARD-04", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 37, RoboTypeId = 4, RobotName = "PROMO-STANDARD-05", ModelName = "PROMO-STANDARD-05", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 38, RoboTypeId = 4, RobotName = "PROMO-STANDARD-06", ModelName = "PROMO-STANDARD-06", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 39, RoboTypeId = 4, RobotName = "PROMO-STANDARD-07", ModelName = "PROMO-STANDARD-07", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 40, RoboTypeId = 4, RobotName = "PROMO-STANDARD-08", ModelName = "PROMO-STANDARD-08", RobotStatus = "Stored", IsDeleted = false },

    // =====================================================
    // PACKAGE: PREMIUM (40 robots)
    // =====================================================

    // RECEPTION (8)
    new Robot { Id = 41, RoboTypeId = 1, RobotName = "REC-PREMIUM-01", ModelName = "REC-PREMIUM-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 42, RoboTypeId = 1, RobotName = "REC-PREMIUM-02", ModelName = "REC-PREMIUM-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 43, RoboTypeId = 1, RobotName = "REC-PREMIUM-03", ModelName = "REC-PREMIUM-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 44, RoboTypeId = 1, RobotName = "REC-PREMIUM-04", ModelName = "REC-PREMIUM-04", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 45, RoboTypeId = 1, RobotName = "REC-PREMIUM-05", ModelName = "REC-PREMIUM-05", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 46, RoboTypeId = 1, RobotName = "REC-PREMIUM-06", ModelName = "REC-PREMIUM-06", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 47, RoboTypeId = 1, RobotName = "REC-PREMIUM-07", ModelName = "REC-PREMIUM-07", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 48, RoboTypeId = 1, RobotName = "REC-PREMIUM-08", ModelName = "REC-PREMIUM-08", RobotStatus = "Stored", IsDeleted = false },

    // PERFORMANCE (12)
    new Robot { Id = 49, RoboTypeId = 2, RobotName = "PERF-PREMIUM-01", ModelName = "PERF-PREMIUM-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 50, RoboTypeId = 2, RobotName = "PERF-PREMIUM-02", ModelName = "PERF-PREMIUM-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 51, RoboTypeId = 2, RobotName = "PERF-PREMIUM-03", ModelName = "PERF-PREMIUM-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 52, RoboTypeId = 2, RobotName = "PERF-PREMIUM-04", ModelName = "PERF-PREMIUM-04", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 53, RoboTypeId = 2, RobotName = "PERF-PREMIUM-05", ModelName = "PERF-PREMIUM-05", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 54, RoboTypeId = 2, RobotName = "PERF-PREMIUM-06", ModelName = "PERF-PREMIUM-06", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 55, RoboTypeId = 2, RobotName = "PERF-PREMIUM-07", ModelName = "PERF-PREMIUM-07", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 56, RoboTypeId = 2, RobotName = "PERF-PREMIUM-08", ModelName = "PERF-PREMIUM-08", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 57, RoboTypeId = 2, RobotName = "PERF-PREMIUM-09", ModelName = "PERF-PREMIUM-09", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 58, RoboTypeId = 2, RobotName = "PERF-PREMIUM-10", ModelName = "PERF-PREMIUM-10", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 59, RoboTypeId = 2, RobotName = "PERF-PREMIUM-11", ModelName = "PERF-PREMIUM-11", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 60, RoboTypeId = 2, RobotName = "PERF-PREMIUM-12", ModelName = "PERF-PREMIUM-12", RobotStatus = "Stored", IsDeleted = false },

    // HOST (8)
    new Robot { Id = 61, RoboTypeId = 3, RobotName = "HOST-PREMIUM-01", ModelName = "HOST-PREMIUM-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 62, RoboTypeId = 3, RobotName = "HOST-PREMIUM-02", ModelName = "HOST-PREMIUM-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 63, RoboTypeId = 3, RobotName = "HOST-PREMIUM-03", ModelName = "HOST-PREMIUM-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 64, RoboTypeId = 3, RobotName = "HOST-PREMIUM-04", ModelName = "HOST-PREMIUM-04", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 65, RoboTypeId = 3, RobotName = "HOST-PREMIUM-05", ModelName = "HOST-PREMIUM-05", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 66, RoboTypeId = 3, RobotName = "HOST-PREMIUM-06", ModelName = "HOST-PREMIUM-06", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 67, RoboTypeId = 3, RobotName = "HOST-PREMIUM-07", ModelName = "HOST-PREMIUM-07", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 68, RoboTypeId = 3, RobotName = "HOST-PREMIUM-08", ModelName = "HOST-PREMIUM-08", RobotStatus = "Stored", IsDeleted = false },

    // PROMOTION (12)
    new Robot { Id = 69, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-01", ModelName = "PROMO-PREMIUM-01", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 70, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-02", ModelName = "PROMO-PREMIUM-02", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 71, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-03", ModelName = "PROMO-PREMIUM-03", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 72, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-04", ModelName = "PROMO-PREMIUM-04", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 73, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-05", ModelName = "PROMO-PREMIUM-05", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 74, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-06", ModelName = "PROMO-PREMIUM-06", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 75, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-07", ModelName = "PROMO-PREMIUM-07", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 76, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-08", ModelName = "PROMO-PREMIUM-08", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 77, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-09", ModelName = "PROMO-PREMIUM-09", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 78, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-10", ModelName = "PROMO-PREMIUM-10", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 79, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-11", ModelName = "PROMO-PREMIUM-11", RobotStatus = "Stored", IsDeleted = false },
    new Robot { Id = 80, RoboTypeId = 4, RobotName = "PROMO-PREMIUM-12", ModelName = "PROMO-PREMIUM-12", RobotStatus = "Stored", IsDeleted = false }
);

        modelBuilder.Entity<ActivityType>().HasData(

    new ActivityType
    {
        Id = 1,
        Code = "BASIC",
        Name = "Basic Event Package",
        ShortDescription = "Gói cơ bản với đầy đủ 4 nhóm robot cho sự kiện nhỏ",
        Description = "Gói BASIC cho sự kiện nhỏ. Khách chọn giờ linh hoạt, hệ thống tính tiền theo đơn giá/giờ. "
                    + "Thời lượng tối thiểu 2 tiếng, làm tròn theo bước 30 phút. Bao gồm 1 kỹ thuật viên đi kèm.",
        Currency = "VND",

        IncludesOperator = true,
        OperatorCount = 1,

        MinimumMinutes = 120,
        BillingIncrementMinutes = 30,

        HourlyRate = 3500000m,
        TechnicalStaffFeePerHour = 150000m,

        Price = 7300000m, // 2h minimum

        IsActive = true,
        IsDeleted = false,
        DamageDeposit = 5000000m
    },

    new ActivityType
    {
        Id = 2,
        Code = "STANDARD",
        Name = "Standard Event Package",
        ShortDescription = "Gói tiêu chuẩn cho sự kiện vừa, tăng cường trình diễn và quảng bá",
        Description = "Gói STANDARD cho sự kiện vừa. Khách chọn giờ linh hoạt, hệ thống tính tiền theo đơn giá/giờ. "
                    + "Thời lượng tối thiểu 2 tiếng, làm tròn theo bước 30 phút. Bao gồm 2 kỹ thuật viên hỗ trợ.",
        Currency = "VND",

        IncludesOperator = true,
        OperatorCount = 2,

        MinimumMinutes = 120,
        BillingIncrementMinutes = 30,

        HourlyRate = 5500000m,
        TechnicalStaffFeePerHour = 200000m,

        Price = 11800000m, // 2h minimum

        IsActive = true,
        IsDeleted = false,
        DamageDeposit = 10000000m
    },

    new ActivityType
    {
        Id = 3,
        Code = "PREMIUM",
        Name = "Premium Event Package",
        ShortDescription = "Gói cao cấp cho sự kiện lớn, trải nghiệm robot toàn diện",
        Description = "Gói PREMIUM cho sự kiện lớn. Khách chọn giờ linh hoạt, hệ thống tính tiền theo đơn giá/giờ. "
                    + "Thời lượng tối thiểu 2 tiếng, làm tròn theo bước 30 phút. Bao gồm 3 kỹ thuật viên cao cấp.",
        Currency = "VND",

        IncludesOperator = true,
        OperatorCount = 3,

        MinimumMinutes = 120,
        BillingIncrementMinutes = 30,

        HourlyRate = 8500000m,
        TechnicalStaffFeePerHour = 300000m,

        Price = 18800000m, // 2h minimum

        IsActive = true,
        IsDeleted = false,
        DamageDeposit = 20000000m
    }
);

        // modelBuilder.Entity<RobotTypeOfActivity>().HasData(
        //     // CATEGORY 1: PERFORMANCE
        //
        //     // Solo Singing
        //     new RobotTypeOfActivity { ActivityTypeId = 1, RoboTypeId = 1, Amount = 1 },
        //
        //     // Duet Singing
        //     new RobotTypeOfActivity { ActivityTypeId = 2, RoboTypeId = 1, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 2, RoboTypeId = 2, Amount = 1 },
        //
        //     // Birthday Song Performance
        //     new RobotTypeOfActivity { ActivityTypeId = 3, RoboTypeId = 1, Amount = 1 },
        //
        //     // Instrument Simulation Performance
        //     new RobotTypeOfActivity { ActivityTypeId = 4, RoboTypeId = 1, Amount = 1 },
        //
        //     // Solo Dance
        //     new RobotTypeOfActivity { ActivityTypeId = 5, RoboTypeId = 2, Amount = 1 },
        //
        //     // Group Robot Dance
        //     new RobotTypeOfActivity { ActivityTypeId = 6, RoboTypeId = 2, Amount = 3 },
        //     new RobotTypeOfActivity { ActivityTypeId = 6, RoboTypeId = 1, Amount = 1 },
        //
        //     // Kids Dance Performance
        //     new RobotTypeOfActivity { ActivityTypeId = 7, RoboTypeId = 2, Amount = 2 },
        //     new RobotTypeOfActivity { ActivityTypeId = 7, RoboTypeId = 1, Amount = 1 },
        //
        //     // Drama Acting
        //     new RobotTypeOfActivity { ActivityTypeId = 8, RoboTypeId = 3, Amount = 2 },
        //     new RobotTypeOfActivity { ActivityTypeId = 8, RoboTypeId = 1, Amount = 1 },
        //
        //     // Comedy Performance
        //     new RobotTypeOfActivity { ActivityTypeId = 9, RoboTypeId = 3, Amount = 2 },
        //
        //     // Theme Performance (Holiday/Seasonal)
        //     new RobotTypeOfActivity { ActivityTypeId = 10, RoboTypeId = 1, Amount = 2 },
        //     new RobotTypeOfActivity { ActivityTypeId = 10, RoboTypeId = 2, Amount = 1 }
        // );
        //
        // modelBuilder.Entity<RobotTypeOfActivity>().HasData(
        //     // CATEGORY 2: PRESENTATION & HOSTING
        //
        //     // Main Event Hosting (MC)
        //     new RobotTypeOfActivity { ActivityTypeId = 11, RoboTypeId = 5, Amount = 1 },
        //
        //     // Supporting MC
        //     new RobotTypeOfActivity { ActivityTypeId = 12, RoboTypeId = 5, Amount = 1 },
        //
        //     // Product Presentation
        //     new RobotTypeOfActivity { ActivityTypeId = 13, RoboTypeId = 6, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 13, RoboTypeId = 7, Amount = 1 },
        //
        //     // Corporate Introduction
        //     new RobotTypeOfActivity { ActivityTypeId = 14, RoboTypeId = 6, Amount = 1 },
        //
        //     // Educational Presentation
        //     new RobotTypeOfActivity { ActivityTypeId = 15, RoboTypeId = 6, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 15, RoboTypeId = 7, Amount = 1 },
        //
        //     // Ceremony Opening Speech
        //     new RobotTypeOfActivity { ActivityTypeId = 16, RoboTypeId = 5, Amount = 1 },
        //
        //     // Guest Introduction
        //     new RobotTypeOfActivity { ActivityTypeId = 17, RoboTypeId = 5, Amount = 1 },
        //
        //     // Event Flow Guidance
        //     new RobotTypeOfActivity { ActivityTypeId = 18, RoboTypeId = 5, Amount = 1 }
        // );
        //
        // modelBuilder.Entity<RobotTypeOfActivity>().HasData(
        //     // CATEGORY 3: STORYTELLING & INTERACTION
        //
        //     // Fairy Tale Storytelling
        //     new RobotTypeOfActivity { ActivityTypeId = 19, RoboTypeId = 8, Amount = 1 },
        //
        //     // Inspirational Storytelling
        //     new RobotTypeOfActivity { ActivityTypeId = 20, RoboTypeId = 8, Amount = 1 },
        //
        //     // Historical Storytelling
        //     new RobotTypeOfActivity { ActivityTypeId = 21, RoboTypeId = 8, Amount = 1 },
        //
        //     // Poetry Reading
        //     new RobotTypeOfActivity { ActivityTypeId = 22, RoboTypeId = 8, Amount = 1 },
        //
        //     // Q&A Interaction
        //     new RobotTypeOfActivity { ActivityTypeId = 23, RoboTypeId = 9, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 23, RoboTypeId = 8, Amount = 1 },
        //
        //     // AI Scripted Conversation
        //     new RobotTypeOfActivity { ActivityTypeId = 24, RoboTypeId = 9, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 24, RoboTypeId = 8, Amount = 1 },
        //
        //     // Audience Interview
        //     new RobotTypeOfActivity { ActivityTypeId = 25, RoboTypeId = 9, Amount = 1 },
        //
        //     // Trivia Quiz Hosting
        //     new RobotTypeOfActivity { ActivityTypeId = 26, RoboTypeId = 10, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 26, RoboTypeId = 9, Amount = 1 },
        //
        //     // Mini Games with Audience
        //     new RobotTypeOfActivity { ActivityTypeId = 27, RoboTypeId = 10, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 27, RoboTypeId = 9, Amount = 1 }
        // );
        //
        // modelBuilder.Entity<RobotTypeOfActivity>().HasData(
        //     // CATEGORY 4: ENTERTAINMENT SUPPORT
        //
        //     // Greeting Guests
        //     new RobotTypeOfActivity { ActivityTypeId = 28, RoboTypeId = 11, Amount = 1 },
        //
        //     // Selfie with Guests
        //     new RobotTypeOfActivity { ActivityTypeId = 29, RoboTypeId = 12, Amount = 1 },
        //
        //     // Guest Check-in Support
        //     new RobotTypeOfActivity { ActivityTypeId = 30, RoboTypeId = 11, Amount = 1 },
        //
        //     // Event Area Tour Guidance
        //     new RobotTypeOfActivity { ActivityTypeId = 31, RoboTypeId = 11, Amount = 1 },
        //
        //     // Crowd Engagement
        //     new RobotTypeOfActivity { ActivityTypeId = 32, RoboTypeId = 11, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 32, RoboTypeId = 12, Amount = 1 },
        //
        //     // Short Dance Break
        //     new RobotTypeOfActivity { ActivityTypeId = 33, RoboTypeId = 13, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 33, RoboTypeId = 11, Amount = 1 },
        //
        //     // Birthday Celebration Support
        //     new RobotTypeOfActivity { ActivityTypeId = 34, RoboTypeId = 11, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 34, RoboTypeId = 12, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 34, RoboTypeId = 13, Amount = 1 },
        //
        //     // Children Entertainment Interaction
        //     new RobotTypeOfActivity { ActivityTypeId = 35, RoboTypeId = 13, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 35, RoboTypeId = 12, Amount = 1 }
        // );
        //
        // modelBuilder.Entity<RobotTypeOfActivity>().HasData(
        //     // CATEGORY 5: PROMOTION & MARKETING
        //
        //     // Product Review
        //     new RobotTypeOfActivity { ActivityTypeId = 36, RoboTypeId = 15, Amount = 1 },
        //
        //     // Promotional Announcement
        //     new RobotTypeOfActivity { ActivityTypeId = 37, RoboTypeId = 16, Amount = 1 },
        //
        //     // Sampling Distribution
        //     new RobotTypeOfActivity { ActivityTypeId = 38, RoboTypeId = 16, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 38, RoboTypeId = 14, Amount = 1 },
        //
        //     // Brand Activation Speech
        //     new RobotTypeOfActivity { ActivityTypeId = 39, RoboTypeId = 14, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 39, RoboTypeId = 16, Amount = 1 },
        //
        //     // Customer Product Consultation
        //     new RobotTypeOfActivity { ActivityTypeId = 40, RoboTypeId = 15, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 40, RoboTypeId = 16, Amount = 1 },
        //
        //     // Collect Customer Information
        //     new RobotTypeOfActivity { ActivityTypeId = 41, RoboTypeId = 16, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 41, RoboTypeId = 14, Amount = 1 },
        //
        //     // Feature Demonstration
        //     new RobotTypeOfActivity { ActivityTypeId = 42, RoboTypeId = 15, Amount = 1 },
        //     new RobotTypeOfActivity { ActivityTypeId = 42, RoboTypeId = 14, Amount = 1 }
        // );
        
        modelBuilder.Entity<RobotTypeOfActivity>().HasData(

            // =========================
            // PACKAGE: BASIC (Id = 1)
            // =========================
            new RobotTypeOfActivity
            {
                ActivityTypeId = 1,
                RoboTypeId = 1, // Reception Robot
                Amount = 1
            },
            new RobotTypeOfActivity
            {
                ActivityTypeId = 1,
                RoboTypeId = 2, // Performance Robot
                Amount = 1
            },
            new RobotTypeOfActivity
            {
                ActivityTypeId = 1,
                RoboTypeId = 3, // Host Robot
                Amount = 1
            },
            new RobotTypeOfActivity
            {
                ActivityTypeId = 1,
                RoboTypeId = 4, // Promotion Robot
                Amount = 1
            },

            // =========================
            // PACKAGE: STANDARD (Id = 2)
            // =========================
            new RobotTypeOfActivity
            {
                ActivityTypeId = 2,
                RoboTypeId = 1, // Reception Robot
                Amount = 1
            },
            new RobotTypeOfActivity
            {
                ActivityTypeId = 2,
                RoboTypeId = 2, // Performance Robot
                Amount = 2
            },
            new RobotTypeOfActivity
            {
                ActivityTypeId = 2,
                RoboTypeId = 3, // Host Robot
                Amount = 1
            },
            new RobotTypeOfActivity
            {
                ActivityTypeId = 2,
                RoboTypeId = 4, // Promotion Robot
                Amount = 2
            },

            // =========================
            // PACKAGE: PREMIUM (Id = 3)
            // =========================
            new RobotTypeOfActivity
            {
                ActivityTypeId = 3,
                RoboTypeId = 1, // Reception Robot
                Amount = 2
            },
            new RobotTypeOfActivity
            {
                ActivityTypeId = 3,
                RoboTypeId = 2, // Performance Robot
                Amount = 3
            },
            new RobotTypeOfActivity
            {
                ActivityTypeId = 3,
                RoboTypeId = 3, // Host Robot
                Amount = 2
            },
            new RobotTypeOfActivity
            {
                ActivityTypeId = 3,
                RoboTypeId = 4, // Promotion Robot
                Amount = 3
            }
        );

        // modelBuilder.Entity<ActivityTypeGroup>().HasData(
        //     // CATEGORY 1: PERFORMANCE
        //     new ActivityTypeGroup { Id = 1, ActivityTypeId = 1, IsDeleted = false }, // AT01 - Solo Singing
        //     new ActivityTypeGroup { Id = 2, ActivityTypeId = 2, IsDeleted = false }, // AT02 - Duet Singing
        //     new ActivityTypeGroup { Id = 3, ActivityTypeId = 3, IsDeleted = false }, // AT03 - Birthday Song Performance
        //     new ActivityTypeGroup
        //         { Id = 4, ActivityTypeId = 4, IsDeleted = false }, // AT04 - Instrument Simulation Performance
        //     new ActivityTypeGroup { Id = 5, ActivityTypeId = 5, IsDeleted = false }, // AT05 - Solo Dance
        //     new ActivityTypeGroup { Id = 6, ActivityTypeId = 6, IsDeleted = false }, // AT06 - Group Robot Dance
        //     new ActivityTypeGroup { Id = 7, ActivityTypeId = 7, IsDeleted = false }, // AT07 - Kids Dance Performance
        //     new ActivityTypeGroup { Id = 8, ActivityTypeId = 8, IsDeleted = false }, // AT08 - Drama Acting
        //     new ActivityTypeGroup { Id = 9, ActivityTypeId = 9, IsDeleted = false }, // AT09 - Comedy Performance
        //     new ActivityTypeGroup
        //         { Id = 10, ActivityTypeId = 10, IsDeleted = false }, // AT10 - Theme Performance (Holiday/Seasonal)
        //
        //     // CATEGORY 2: PRESENTATION & HOSTING
        //     new ActivityTypeGroup { Id = 11, ActivityTypeId = 11, IsDeleted = false }, // AT11 - Main Event Hosting (MC)
        //     new ActivityTypeGroup { Id = 12, ActivityTypeId = 12, IsDeleted = false }, // AT12 - Supporting MC
        //     new ActivityTypeGroup { Id = 13, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
        //     new ActivityTypeGroup { Id = 14, ActivityTypeId = 14, IsDeleted = false }, // AT14 - Corporate Introduction
        //     new ActivityTypeGroup
        //         { Id = 15, ActivityTypeId = 15, IsDeleted = false }, // AT15 - Educational Presentation
        //     new ActivityTypeGroup { Id = 16, ActivityTypeId = 16, IsDeleted = false }, // AT16 - Ceremony Opening Speech
        //     new ActivityTypeGroup { Id = 17, ActivityTypeId = 17, IsDeleted = false }, // AT17 - Guest Introduction
        //     new ActivityTypeGroup { Id = 18, ActivityTypeId = 18, IsDeleted = false }, // AT18 - Event Flow Guidance
        //
        //     // CATEGORY 3: STORYTELLING & INTERACTION
        //     new ActivityTypeGroup { Id = 19, ActivityTypeId = 19, IsDeleted = false }, // AT19 - Fairy Tale Storytelling
        //     new ActivityTypeGroup
        //         { Id = 20, ActivityTypeId = 20, IsDeleted = false }, // AT20 - Inspirational Storytelling
        //     new ActivityTypeGroup { Id = 21, ActivityTypeId = 21, IsDeleted = false }, // AT21 - Historical Storytelling
        //     new ActivityTypeGroup { Id = 22, ActivityTypeId = 22, IsDeleted = false }, // AT22 - Poetry Reading
        //     new ActivityTypeGroup { Id = 23, ActivityTypeId = 23, IsDeleted = false }, // AT23 - Q&A Interaction
        //     new ActivityTypeGroup
        //         { Id = 24, ActivityTypeId = 24, IsDeleted = false }, // AT24 - AI Scripted Conversation
        //     new ActivityTypeGroup { Id = 25, ActivityTypeId = 25, IsDeleted = false }, // AT25 - Audience Interview
        //     new ActivityTypeGroup { Id = 26, ActivityTypeId = 26, IsDeleted = false }, // AT26 - Trivia Quiz Hosting
        //     new ActivityTypeGroup
        //         { Id = 27, ActivityTypeId = 27, IsDeleted = false }, // AT27 - Mini Games with Audience
        //
        //     // CATEGORY 4: ENTERTAINMENT SUPPORT
        //     new ActivityTypeGroup { Id = 28, ActivityTypeId = 28, IsDeleted = false }, // AT28 - Greeting Guests
        //     new ActivityTypeGroup { Id = 29, ActivityTypeId = 29, IsDeleted = false }, // AT29 - Selfie with Guests
        //     new ActivityTypeGroup { Id = 30, ActivityTypeId = 30, IsDeleted = false }, // AT30 - Guest Check-in Support
        //     new ActivityTypeGroup
        //         { Id = 31, ActivityTypeId = 31, IsDeleted = false }, // AT31 - Event Area Tour Guidance
        //     new ActivityTypeGroup { Id = 32, ActivityTypeId = 32, IsDeleted = false }, // AT32 - Crowd Engagement
        //     new ActivityTypeGroup { Id = 33, ActivityTypeId = 33, IsDeleted = false }, // AT33 - Short Dance Break
        //     new ActivityTypeGroup
        //         { Id = 34, ActivityTypeId = 34, IsDeleted = false }, // AT34 - Birthday Celebration Support
        //     new ActivityTypeGroup
        //         { Id = 35, ActivityTypeId = 35, IsDeleted = false }, // AT35 - Children Entertainment Interaction
        //
        //     // CATEGORY 5: PROMOTION & MARKETING
        //     new ActivityTypeGroup { Id = 36, ActivityTypeId = 36, IsDeleted = false }, // AT36 - Product Review
        //     new ActivityTypeGroup
        //         { Id = 37, ActivityTypeId = 37, IsDeleted = false }, // AT37 - Promotional Announcement
        //     new ActivityTypeGroup { Id = 38, ActivityTypeId = 38, IsDeleted = false }, // AT38 - Sampling Distribution
        //     new ActivityTypeGroup { Id = 39, ActivityTypeId = 39, IsDeleted = false }, // AT39 - Brand Activation Speech
        //     new ActivityTypeGroup
        //         { Id = 40, ActivityTypeId = 40, IsDeleted = false }, // AT40 - Customer Product Consultation
        //     new ActivityTypeGroup
        //         { Id = 41, ActivityTypeId = 41, IsDeleted = false }, // AT41 - Collect Customer Information
        //     new ActivityTypeGroup { Id = 42, ActivityTypeId = 42, IsDeleted = false } // AT42 - Feature Demonstration
        // );
        //
        // modelBuilder.Entity<ActivityTypeGroup>().HasData(
        //     // CATEGORY 1 — PERFORMANCE
        //     new ActivityTypeGroup { Id = 43, ActivityTypeId = 6, IsDeleted = false }, // AT06 - Group Robot Dance
        //     new ActivityTypeGroup { Id = 44, ActivityTypeId = 9, IsDeleted = false }, // AT09 - Comedy Performance
        //     new ActivityTypeGroup { Id = 45, ActivityTypeId = 9, IsDeleted = false }, // AT09 - Comedy Performance
        //     new ActivityTypeGroup { Id = 46, ActivityTypeId = 9, IsDeleted = false }, // AT09 - Comedy Performance
        //
        //     // CATEGORY 2 — PRESENTATION & HOSTING
        //     new ActivityTypeGroup { Id = 47, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
        //     new ActivityTypeGroup { Id = 48, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
        //     new ActivityTypeGroup { Id = 49, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
        //     new ActivityTypeGroup { Id = 50, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
        //     new ActivityTypeGroup
        //         { Id = 51, ActivityTypeId = 15, IsDeleted = false }, // AT15 - Educational Presentation
        //     new ActivityTypeGroup
        //         { Id = 52, ActivityTypeId = 15, IsDeleted = false }, // AT15 - Educational Presentation
        //     new ActivityTypeGroup { Id = 53, ActivityTypeId = 11, IsDeleted = false }, // AT11 - Main Event Hosting (MC)
        //     new ActivityTypeGroup { Id = 54, ActivityTypeId = 11, IsDeleted = false }, // AT11 - Main Event Hosting (MC)
        //     new ActivityTypeGroup { Id = 55, ActivityTypeId = 12, IsDeleted = false }, // AT12 - Supporting MC
        //     new ActivityTypeGroup { Id = 56, ActivityTypeId = 16, IsDeleted = false }, // AT16 - Ceremony Opening Speech
        //     new ActivityTypeGroup { Id = 57, ActivityTypeId = 16, IsDeleted = false }, // AT16 - Ceremony Opening Speech
        //     new ActivityTypeGroup { Id = 58, ActivityTypeId = 17, IsDeleted = false }, // AT17 - Guest Introduction
        //
        //     // CATEGORY 3 — STORYTELLING & INTERACTION
        //     new ActivityTypeGroup { Id = 59, ActivityTypeId = 19, IsDeleted = false }, // AT19 - Fairy Tale Storytelling
        //     new ActivityTypeGroup
        //         { Id = 60, ActivityTypeId = 20, IsDeleted = false }, // AT20 - Inspirational Storytelling
        //     new ActivityTypeGroup { Id = 61, ActivityTypeId = 21, IsDeleted = false }, // AT21 - Historical Storytelling
        //     new ActivityTypeGroup { Id = 62, ActivityTypeId = 22, IsDeleted = false }, // AT22 - Poetry Reading
        //     new ActivityTypeGroup { Id = 63, ActivityTypeId = 26, IsDeleted = false }, // AT26 - Trivia Quiz Hosting
        //     new ActivityTypeGroup { Id = 64, ActivityTypeId = 26, IsDeleted = false }, // AT26 - Trivia Quiz Hosting
        //     new ActivityTypeGroup
        //         { Id = 65, ActivityTypeId = 27, IsDeleted = false }, // AT27 - Mini Games with Audience
        //     new ActivityTypeGroup
        //         { Id = 66, ActivityTypeId = 27, IsDeleted = false }, // AT27 - Mini Games with Audience
        //     new ActivityTypeGroup { Id = 67, ActivityTypeId = 25, IsDeleted = false }, // AT25 - Audience Interview
        //
        //     // CATEGORY 4 — ENTERTAINMENT SUPPORT
        //     new ActivityTypeGroup
        //         { Id = 68, ActivityTypeId = 34, IsDeleted = false }, // AT34 - Birthday Celebration Support
        //     new ActivityTypeGroup
        //         { Id = 69, ActivityTypeId = 34, IsDeleted = false }, // AT34 - Birthday Celebration Support
        //     new ActivityTypeGroup { Id = 70, ActivityTypeId = 32, IsDeleted = false }, // AT32 - Crowd Engagement
        //     new ActivityTypeGroup { Id = 71, ActivityTypeId = 33, IsDeleted = false }, // AT33 - Short Dance Break
        //     new ActivityTypeGroup
        //         { Id = 72, ActivityTypeId = 35, IsDeleted = false }, // AT35 - Children Entertainment Interaction
        //     new ActivityTypeGroup
        //         { Id = 73, ActivityTypeId = 35, IsDeleted = false }, // AT35 - Children Entertainment Interaction
        //     new ActivityTypeGroup
        //         { Id = 74, ActivityTypeId = 35, IsDeleted = false }, // AT35 - Children Entertainment Interaction
        //
        //     // CATEGORY 5 — PROMOTION & MARKETING
        //     new ActivityTypeGroup { Id = 75, ActivityTypeId = 39, IsDeleted = false }, // AT39 - Brand Activation Speech
        //     new ActivityTypeGroup { Id = 76, ActivityTypeId = 39, IsDeleted = false }, // AT39 - Brand Activation Speech
        //     new ActivityTypeGroup
        //         { Id = 77, ActivityTypeId = 41, IsDeleted = false }, // AT41 - Collect Customer Information
        //     new ActivityTypeGroup
        //         { Id = 78, ActivityTypeId = 40, IsDeleted = false }, // AT40 - Customer Product Consultation
        //     new ActivityTypeGroup { Id = 79, ActivityTypeId = 38, IsDeleted = false }, // AT38 - Sampling Distribution
        //     new ActivityTypeGroup { Id = 80, ActivityTypeId = 42, IsDeleted = false }, // AT42 - Feature Demonstration
        //     new ActivityTypeGroup { Id = 81, ActivityTypeId = 42, IsDeleted = false }, // AT42 - Feature Demonstration
        //     new ActivityTypeGroup { Id = 82, ActivityTypeId = 36, IsDeleted = false }, // AT36 - Product Review
        //     new ActivityTypeGroup { Id = 83, ActivityTypeId = 36, IsDeleted = false }, // AT36 - Product Review
        //     new ActivityTypeGroup { Id = 84, ActivityTypeId = 36, IsDeleted = false }, // AT36 - Product Review
        //     new ActivityTypeGroup { Id = 85, ActivityTypeId = 36, IsDeleted = false } // AT36 - Product Review
        // );
        //
        // modelBuilder.Entity<RobotInGroup>().HasData(
        //     // ===== CATEGORY 1: PERFORMANCE =====
        //     // AT01 - Solo Singing (HPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 1, RobotId = 1, IsDeleted = false }, // HPR-01
        //
        //     // AT02 - Duet Singing (HPR x1, DCR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 2, RobotId = 2, IsDeleted = false }, // HPR-02
        //     new RobotInGroup { ActivityTypeGroupId = 2, RobotId = 11, IsDeleted = false }, // DCR-11
        //
        //     // AT03 - Birthday Song Performance (HPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 3, RobotId = 3, IsDeleted = false }, // HPR-03
        //
        //     // AT04 - Instrument Simulation Performance (HPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 4, RobotId = 4, IsDeleted = false }, // HPR-04
        //
        //     // AT05 - Solo Dance (DCR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 5, RobotId = 12, IsDeleted = false }, // DCR-12
        //
        //     // AT06 - Group Robot Dance (DCR x3 + HPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 13, IsDeleted = false }, // DCR-13
        //     new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 14, IsDeleted = false }, // DCR-14
        //     new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 15, IsDeleted = false }, // DCR-15
        //     new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 5, IsDeleted = false }, // HPR-05
        //
        //     // AT07 - Kids Dance Performance (DCR x2 + HPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 16, IsDeleted = false }, // DCR-16
        //     new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 17, IsDeleted = false }, // DCR-17
        //     new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 6, IsDeleted = false }, // HPR-06
        //
        //     // AT08 - Drama Acting (ARR x2 + HPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 21, IsDeleted = false }, // ARR-21
        //     new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 22, IsDeleted = false }, // ARR-22
        //     new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 7, IsDeleted = false }, // HPR-07
        //
        //     // AT09 - Comedy Performance (ARR x2)
        //     new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 23, IsDeleted = false }, // ARR-23
        //     new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 24, IsDeleted = false }, // ARR-24
        //
        //     // AT10 - Theme Performance (HPR x2)
        //     new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 8, IsDeleted = false }, // HPR-08
        //     new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 9, IsDeleted = false }, // HPR-09
        //
        //     // ===== CATEGORY 2: PRESENTATION & HOSTING =====
        //     // AT11 - Main Event Hosting (HMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 41, IsDeleted = false }, // HMR-41
        //
        //     // AT12 - Supporting MC (HMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 42, IsDeleted = false }, // HMR-42
        //
        //     // AT13 - Product Presentation (PRR x1 + TPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 13, RobotId = 51, IsDeleted = false }, // PRR-51
        //     new RobotInGroup { ActivityTypeGroupId = 13, RobotId = 61, IsDeleted = false }, // TPR-61
        //
        //     // AT14 - Corporate Introduction (PRR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 14, RobotId = 52, IsDeleted = false }, // PRR-52
        //
        //     // AT15 - Educational Presentation (PRR x1 + TPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 15, RobotId = 59, IsDeleted = false }, // PRR-59
        //     new RobotInGroup { ActivityTypeGroupId = 15, RobotId = 62, IsDeleted = false }, // TPR-62
        //
        //     // AT16 - Ceremony Opening Speech (HMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 16, RobotId = 43, IsDeleted = false }, // HMR-43
        //
        //     // AT17 - Guest Introduction (HMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 17, RobotId = 44, IsDeleted = false }, // HMR-44
        //
        //     // AT18 - Event Flow Guidance (HMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 18, RobotId = 45, IsDeleted = false }, // HMR-45
        //     // (Nếu đã seed "Only New" dùng HMR-45, hãy dịch HMR của "Only New" sang 46..50 để tránh trùng)
        //
        //     // ===== CATEGORY 3: STORYTELLING & INTERACTION =====
        //     // AT19 - Fairy Tale Storytelling (STR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 19, RobotId = 71, IsDeleted = false }, // STR-71
        //
        //     // AT20 - Inspirational Storytelling (STR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 20, RobotId = 72, IsDeleted = false }, // STR-72
        //
        //     // AT21 - Historical Storytelling (STR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 21, RobotId = 73, IsDeleted = false }, // STR-73
        //
        //     // AT22 - Poetry Reading (STR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 22, RobotId = 74, IsDeleted = false }, // STR-74
        //
        //     // AT23 - Q&A Interaction (ISR x1 + STR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 23, RobotId = 81, IsDeleted = false }, // ISR-81
        //     new RobotInGroup { ActivityTypeGroupId = 23, RobotId = 75, IsDeleted = false }, // STR-75
        //
        //     // AT24 - AI Scripted Conversation (ISR x1 + STR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 24, RobotId = 82, IsDeleted = false }, // ISR-82
        //     new RobotInGroup { ActivityTypeGroupId = 24, RobotId = 76, IsDeleted = false }, // STR-76
        //
        //     // AT25 - Audience Interview (ISR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 25, RobotId = 83, IsDeleted = false }, // ISR-83
        //
        //     // AT26 - Trivia Quiz Hosting (GQR x1 + ISR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 26, RobotId = 91, IsDeleted = false }, // GQR-91
        //     new RobotInGroup { ActivityTypeGroupId = 26, RobotId = 84, IsDeleted = false }, // ISR-84
        //
        //     // AT27 - Mini Games with Audience (GQR x1 + ISR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 27, RobotId = 92, IsDeleted = false }, // GQR-92
        //     new RobotInGroup { ActivityTypeGroupId = 27, RobotId = 85, IsDeleted = false }, // ISR-85
        //
        //     // ===== CATEGORY 4: ENTERTAINMENT SUPPORT =====
        //     // AT28 - Greeting Guests (HSR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 28, RobotId = 101, IsDeleted = false }, // HSR-101
        //
        //     // AT29 - Selfie with Guests (SPB x1)
        //     new RobotInGroup { ActivityTypeGroupId = 29, RobotId = 111, IsDeleted = false }, // SPB-111
        //
        //     // AT30 - Guest Check-in Support (HSR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 30, RobotId = 102, IsDeleted = false }, // HSR-102
        //
        //     // AT31 - Event Area Tour Guidance (HSR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 31, RobotId = 103, IsDeleted = false }, // HSR-103
        //
        //     // AT32 - Crowd Engagement (HSR x1 + SPB x1)
        //     new RobotInGroup { ActivityTypeGroupId = 32, RobotId = 104, IsDeleted = false }, // HSR-104
        //     new RobotInGroup { ActivityTypeGroupId = 32, RobotId = 112, IsDeleted = false }, // SPB-112
        //
        //     // AT33 - Short Dance Break (ANR x1 + HSR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 33, RobotId = 121, IsDeleted = false }, // ANR-121
        //     new RobotInGroup { ActivityTypeGroupId = 33, RobotId = 105, IsDeleted = false }, // HSR-105
        //
        //     // AT34 - Birthday Celebration Support (HSR x1 + SPB x1 + ANR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 34, RobotId = 106, IsDeleted = false }, // HSR-106
        //     new RobotInGroup { ActivityTypeGroupId = 34, RobotId = 113, IsDeleted = false }, // SPB-113
        //     new RobotInGroup { ActivityTypeGroupId = 34, RobotId = 122, IsDeleted = false }, // ANR-122
        //
        //     // AT35 - Children Entertainment Interaction (ANR x1 + SPB x1)
        //     new RobotInGroup { ActivityTypeGroupId = 35, RobotId = 123, IsDeleted = false }, // ANR-123
        //     new RobotInGroup { ActivityTypeGroupId = 35, RobotId = 114, IsDeleted = false }, // SPB-114
        //
        //     // ===== CATEGORY 5: PROMOTION & MARKETING =====
        //     // AT36 - Product Review (PDR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 36, RobotId = 141, IsDeleted = false }, // PDR-141
        //
        //     // AT37 - Promotional Announcement (PMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 37, RobotId = 151, IsDeleted = false }, // PMR-151
        //
        //     // AT38 - Sampling Distribution (PMR x1 + BAR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 38, RobotId = 152, IsDeleted = false }, // PMR-152
        //     new RobotInGroup { ActivityTypeGroupId = 38, RobotId = 131, IsDeleted = false }, // BAR-131
        //
        //     // AT39 - Brand Activation Speech (BAR x1 + PMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 39, RobotId = 132, IsDeleted = false }, // BAR-132
        //     new RobotInGroup { ActivityTypeGroupId = 39, RobotId = 153, IsDeleted = false }, // PMR-153
        //
        //     // AT40 - Customer Product Consultation (PDR x1 + PMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 40, RobotId = 142, IsDeleted = false }, // PDR-142
        //     new RobotInGroup { ActivityTypeGroupId = 40, RobotId = 154, IsDeleted = false }, // PMR-154
        //
        //     // AT41 - Collect Customer Information (PMR x1 + BAR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 41, RobotId = 155, IsDeleted = false }, // PMR-155
        //     new RobotInGroup { ActivityTypeGroupId = 41, RobotId = 133, IsDeleted = false }, // BAR-133
        //
        //     // AT42 - Feature Demonstration (PDR x1 + BAR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 42, RobotId = 143, IsDeleted = false }, // PDR-143
        //     new RobotInGroup { ActivityTypeGroupId = 42, RobotId = 134, IsDeleted = false } // BAR-134
        // );
        //
        //
        // modelBuilder.Entity<RobotInGroup>().HasData(
        //     // ===== CATEGORY 1 =====
        //     // G43: AT06 - Group Robot Dance (DCR x3 + HPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 43, RobotId = 10, IsDeleted = false }, // HPR-10
        //     new RobotInGroup { ActivityTypeGroupId = 43, RobotId = 18, IsDeleted = false }, // DCR-18
        //     new RobotInGroup { ActivityTypeGroupId = 43, RobotId = 19, IsDeleted = false }, // DCR-19
        //     new RobotInGroup { ActivityTypeGroupId = 43, RobotId = 20, IsDeleted = false }, // DCR-20
        //
        //     // G44–46: AT09 - Comedy Performance (ARR x2 mỗi nhóm)
        //     new RobotInGroup { ActivityTypeGroupId = 44, RobotId = 25, IsDeleted = false }, // ARR-25
        //     new RobotInGroup { ActivityTypeGroupId = 44, RobotId = 26, IsDeleted = false }, // ARR-26
        //     new RobotInGroup { ActivityTypeGroupId = 45, RobotId = 27, IsDeleted = false }, // ARR-27
        //     new RobotInGroup { ActivityTypeGroupId = 45, RobotId = 28, IsDeleted = false }, // ARR-28
        //     new RobotInGroup { ActivityTypeGroupId = 46, RobotId = 29, IsDeleted = false }, // ARR-29
        //     new RobotInGroup { ActivityTypeGroupId = 46, RobotId = 30, IsDeleted = false }, // ARR-30
        //
        //     // ===== CATEGORY 2 =====
        //     // G47–50: AT13 - Product Presentation (PRR x1 + TPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 47, RobotId = 53, IsDeleted = false }, // PRR-53
        //     new RobotInGroup { ActivityTypeGroupId = 47, RobotId = 63, IsDeleted = false }, // TPR-63
        //     new RobotInGroup { ActivityTypeGroupId = 48, RobotId = 54, IsDeleted = false }, // PRR-54
        //     new RobotInGroup { ActivityTypeGroupId = 48, RobotId = 64, IsDeleted = false }, // TPR-64
        //     new RobotInGroup { ActivityTypeGroupId = 49, RobotId = 55, IsDeleted = false }, // PRR-55
        //     new RobotInGroup { ActivityTypeGroupId = 49, RobotId = 65, IsDeleted = false }, // TPR-65
        //     new RobotInGroup { ActivityTypeGroupId = 50, RobotId = 56, IsDeleted = false }, // PRR-56
        //     new RobotInGroup { ActivityTypeGroupId = 50, RobotId = 66, IsDeleted = false }, // TPR-66
        //
        //     // G51–52: AT15 - Educational Presentation (PRR x1 + TPR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 51, RobotId = 57, IsDeleted = false }, // PRR-57
        //     new RobotInGroup { ActivityTypeGroupId = 51, RobotId = 67, IsDeleted = false }, // TPR-67
        //     new RobotInGroup { ActivityTypeGroupId = 52, RobotId = 58, IsDeleted = false }, // PRR-58
        //     new RobotInGroup { ActivityTypeGroupId = 52, RobotId = 68, IsDeleted = false }, // TPR-68
        //
        //     // G53–58: HMR-only (AT11 x2, AT12 x1, AT16 x2, AT17 x1)
        //     new RobotInGroup { ActivityTypeGroupId = 53, RobotId = 45, IsDeleted = false }, // HMR-45
        //     new RobotInGroup { ActivityTypeGroupId = 54, RobotId = 46, IsDeleted = false }, // HMR-46
        //     new RobotInGroup { ActivityTypeGroupId = 55, RobotId = 47, IsDeleted = false }, // HMR-47
        //     new RobotInGroup { ActivityTypeGroupId = 56, RobotId = 48, IsDeleted = false }, // HMR-48
        //     new RobotInGroup { ActivityTypeGroupId = 57, RobotId = 49, IsDeleted = false }, // HMR-49
        //     new RobotInGroup { ActivityTypeGroupId = 58, RobotId = 50, IsDeleted = false }, // HMR-50
        //
        //     // ===== CATEGORY 3 =====
        //     // G59–62: STR-only (AT19..AT22)
        //     new RobotInGroup { ActivityTypeGroupId = 59, RobotId = 77, IsDeleted = false }, // STR-77
        //     new RobotInGroup { ActivityTypeGroupId = 60, RobotId = 78, IsDeleted = false }, // STR-78
        //     new RobotInGroup { ActivityTypeGroupId = 61, RobotId = 79, IsDeleted = false }, // STR-79
        //     new RobotInGroup { ActivityTypeGroupId = 62, RobotId = 80, IsDeleted = false }, // STR-80
        //
        //     // G63–66: AT26/AT27 (GQR x1 + ISR x1 mỗi nhóm)
        //     new RobotInGroup { ActivityTypeGroupId = 63, RobotId = 93, IsDeleted = false }, // GQR-93
        //     new RobotInGroup { ActivityTypeGroupId = 63, RobotId = 86, IsDeleted = false }, // ISR-86
        //     new RobotInGroup { ActivityTypeGroupId = 64, RobotId = 94, IsDeleted = false }, // GQR-94
        //     new RobotInGroup { ActivityTypeGroupId = 64, RobotId = 87, IsDeleted = false }, // ISR-87
        //     new RobotInGroup { ActivityTypeGroupId = 65, RobotId = 95, IsDeleted = false }, // GQR-95
        //     new RobotInGroup { ActivityTypeGroupId = 65, RobotId = 88, IsDeleted = false }, // ISR-88
        //     new RobotInGroup { ActivityTypeGroupId = 66, RobotId = 96, IsDeleted = false }, // GQR-96
        //     new RobotInGroup { ActivityTypeGroupId = 66, RobotId = 89, IsDeleted = false }, // ISR-89
        //
        //     // G67: AT25 - Audience Interview (ISR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 67, RobotId = 90, IsDeleted = false }, // ISR-90
        //
        //     // ===== CATEGORY 4 =====
        //     // G68–69: AT34 (HSR x1 + SPB x1 + ANR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 68, RobotId = 107, IsDeleted = false }, // HSR-107
        //     new RobotInGroup { ActivityTypeGroupId = 68, RobotId = 115, IsDeleted = false }, // SPB-115
        //     new RobotInGroup { ActivityTypeGroupId = 68, RobotId = 124, IsDeleted = false }, // ANR-124
        //     new RobotInGroup { ActivityTypeGroupId = 69, RobotId = 108, IsDeleted = false }, // HSR-108
        //     new RobotInGroup { ActivityTypeGroupId = 69, RobotId = 116, IsDeleted = false }, // SPB-116
        //     new RobotInGroup { ActivityTypeGroupId = 69, RobotId = 125, IsDeleted = false }, // ANR-125
        //
        //     // G70: AT32 (HSR x1 + SPB x1)
        //     new RobotInGroup { ActivityTypeGroupId = 70, RobotId = 109, IsDeleted = false }, // HSR-109
        //     new RobotInGroup { ActivityTypeGroupId = 70, RobotId = 117, IsDeleted = false }, // SPB-117
        //
        //     // G71: AT33 (ANR x1 + HSR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 71, RobotId = 126, IsDeleted = false }, // ANR-126
        //     new RobotInGroup { ActivityTypeGroupId = 71, RobotId = 110, IsDeleted = false }, // HSR-110
        //
        //     // G72–74: AT35 (ANR x1 + SPB x1 mỗi nhóm)
        //     new RobotInGroup { ActivityTypeGroupId = 72, RobotId = 127, IsDeleted = false }, // ANR-127
        //     new RobotInGroup { ActivityTypeGroupId = 72, RobotId = 118, IsDeleted = false }, // SPB-118
        //     new RobotInGroup { ActivityTypeGroupId = 73, RobotId = 128, IsDeleted = false }, // ANR-128
        //     new RobotInGroup { ActivityTypeGroupId = 73, RobotId = 119, IsDeleted = false }, // SPB-119
        //     new RobotInGroup { ActivityTypeGroupId = 74, RobotId = 129, IsDeleted = false }, // ANR-129
        //     new RobotInGroup { ActivityTypeGroupId = 74, RobotId = 120, IsDeleted = false }, // SPB-120
        //
        //     // ===== CATEGORY 5 =====
        //     // G75–76: AT39 (BAR x1 + PMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 75, RobotId = 135, IsDeleted = false }, // BAR-135
        //     new RobotInGroup { ActivityTypeGroupId = 75, RobotId = 156, IsDeleted = false }, // PMR-156
        //     new RobotInGroup { ActivityTypeGroupId = 76, RobotId = 136, IsDeleted = false }, // BAR-136
        //     new RobotInGroup { ActivityTypeGroupId = 76, RobotId = 157, IsDeleted = false }, // PMR-157
        //
        //     // G77: AT41 (PMR x1 + BAR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 77, RobotId = 158, IsDeleted = false }, // PMR-158
        //     new RobotInGroup { ActivityTypeGroupId = 77, RobotId = 137, IsDeleted = false }, // BAR-137
        //
        //     // G78: AT40 (PDR x1 + PMR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 78, RobotId = 144, IsDeleted = false }, // PDR-144
        //     new RobotInGroup { ActivityTypeGroupId = 78, RobotId = 159, IsDeleted = false }, // PMR-159
        //
        //     // G79: AT38 (PMR x1 + BAR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 79, RobotId = 160, IsDeleted = false }, // PMR-160
        //     new RobotInGroup { ActivityTypeGroupId = 79, RobotId = 138, IsDeleted = false }, // BAR-138
        //
        //     // G80–81: AT42 (PDR x1 + BAR x1)
        //     new RobotInGroup { ActivityTypeGroupId = 80, RobotId = 145, IsDeleted = false }, // PDR-145
        //     new RobotInGroup { ActivityTypeGroupId = 80, RobotId = 139, IsDeleted = false }, // BAR-139
        //     new RobotInGroup { ActivityTypeGroupId = 81, RobotId = 146, IsDeleted = false }, // PDR-146
        //     new RobotInGroup { ActivityTypeGroupId = 81, RobotId = 140, IsDeleted = false }, // BAR-140
        //
        //     // G82–85: AT36 (PDR x1 mỗi nhóm)
        //     new RobotInGroup { ActivityTypeGroupId = 82, RobotId = 147, IsDeleted = false }, // PDR-147
        //     new RobotInGroup { ActivityTypeGroupId = 83, RobotId = 148, IsDeleted = false }, // PDR-148
        //     new RobotInGroup { ActivityTypeGroupId = 84, RobotId = 149, IsDeleted = false }, // PDR-149
        //     new RobotInGroup { ActivityTypeGroupId = 85, RobotId = 150, IsDeleted = false } // PDR-150
        // );
        
        modelBuilder.Entity<ActivityTypeGroup>().HasData(

            // =========================
            // BASIC PACKAGE (ActivityTypeId = 1)
            // =========================
            new ActivityTypeGroup { Id = 1, ActivityTypeId = 1, IsDeleted = false },
            new ActivityTypeGroup { Id = 2, ActivityTypeId = 1, IsDeleted = false },
            new ActivityTypeGroup { Id = 3, ActivityTypeId = 1, IsDeleted = false },
            new ActivityTypeGroup { Id = 4, ActivityTypeId = 1, IsDeleted = false },

            // =========================
            // STANDARD PACKAGE (ActivityTypeId = 2)
            // =========================
            new ActivityTypeGroup { Id = 5, ActivityTypeId = 2, IsDeleted = false },
            new ActivityTypeGroup { Id = 6, ActivityTypeId = 2, IsDeleted = false },
            new ActivityTypeGroup { Id = 7, ActivityTypeId = 2, IsDeleted = false },
            new ActivityTypeGroup { Id = 8, ActivityTypeId = 2, IsDeleted = false },

            // =========================
            // PREMIUM PACKAGE (ActivityTypeId = 3)
            // =========================
            new ActivityTypeGroup { Id = 9, ActivityTypeId = 3, IsDeleted = false },
            new ActivityTypeGroup { Id = 10, ActivityTypeId = 3, IsDeleted = false },
            new ActivityTypeGroup { Id = 11, ActivityTypeId = 3, IsDeleted = false },
            new ActivityTypeGroup { Id = 12, ActivityTypeId = 3, IsDeleted = false }
        );

        modelBuilder.Entity<RobotInGroup>().HasData(

    // =====================================================
    // BASIC GROUPS (ActivityTypeGroupId: 1..4)
    // Each group: 4 robots
    // =====================================================

    // BASIC - Group 1: RobotId 1..4
    new RobotInGroup { ActivityTypeGroupId = 1, RobotId = 1, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 1, RobotId = 2, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 1, RobotId = 3, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 1, RobotId = 4, IsDeleted = false },

    // BASIC - Group 2: RobotId 5..8
    new RobotInGroup { ActivityTypeGroupId = 2, RobotId = 5, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 2, RobotId = 6, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 2, RobotId = 7, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 2, RobotId = 8, IsDeleted = false },

    // BASIC - Group 3: RobotId 9..12
    new RobotInGroup { ActivityTypeGroupId = 3, RobotId = 9, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 3, RobotId = 10, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 3, RobotId = 11, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 3, RobotId = 12, IsDeleted = false },

    // BASIC - Group 4: RobotId 13..16
    new RobotInGroup { ActivityTypeGroupId = 4, RobotId = 13, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 4, RobotId = 14, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 4, RobotId = 15, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 4, RobotId = 16, IsDeleted = false },


    // =====================================================
    // STANDARD GROUPS (ActivityTypeGroupId: 5..8)
    // Each group: 6 robots (1 Reception, 2 Performance, 1 Host, 2 Promotion)
    // =====================================================

    // STANDARD - Group 5: RobotId 17..22
    new RobotInGroup { ActivityTypeGroupId = 5, RobotId = 17, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 5, RobotId = 18, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 5, RobotId = 19, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 5, RobotId = 20, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 5, RobotId = 21, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 5, RobotId = 22, IsDeleted = false },

    // STANDARD - Group 6: RobotId 23..28
    new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 23, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 24, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 25, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 26, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 27, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 28, IsDeleted = false },

    // STANDARD - Group 7: RobotId 29..34
    new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 29, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 30, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 31, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 32, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 33, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 34, IsDeleted = false },

    // STANDARD - Group 8: RobotId 35..40
    new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 35, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 36, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 37, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 38, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 39, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 40, IsDeleted = false },


    // =====================================================
    // PREMIUM GROUPS (ActivityTypeGroupId: 9..12)
    // Each group: 10 robots (2 Reception, 3 Performance, 2 Host, 3 Promotion)
    // =====================================================

    // PREMIUM - Group 9: RobotId 41..50
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 41, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 42, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 43, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 44, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 45, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 46, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 47, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 48, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 49, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 50, IsDeleted = false },

    // PREMIUM - Group 10: RobotId 51..60
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 51, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 52, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 53, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 54, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 55, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 56, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 57, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 58, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 59, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 60, IsDeleted = false },

    // PREMIUM - Group 11: RobotId 61..70
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 61, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 62, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 63, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 64, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 65, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 66, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 67, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 68, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 69, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 70, IsDeleted = false },

    // PREMIUM - Group 12: RobotId 71..80
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 71, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 72, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 73, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 74, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 75, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 76, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 77, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 78, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 79, IsDeleted = false },
    new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 80, IsDeleted = false }
);
        modelBuilder.Entity<RobotAbility>().HasData(

    // =========================================================
    // RoboTypeId = 1 (Reception Robot)
    // =========================================================

    // --- Branding & UI (mostly LOCK) ---
    new RobotAbility {
        Id = 1, RobotTypeId = 1,
        Key = "brandName", Label = "Brand Name",
        Description = "Tên thương hiệu hiển thị trên màn hình robot.",
        DataType = "string", IsRequired = true, AbilityGroup = "Branding & UI",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "text", Placeholder = "VD: RoboRent",
        MaxLength = 100, IsActive = true
    },
    new RobotAbility {
        Id = 2, RobotTypeId = 1,
        Key = "logoUrl", Label = "Logo URL",
        Description = "Đường dẫn logo (PNG/SVG) hiển thị trên robot.",
        DataType = "string", IsRequired = true, AbilityGroup = "Branding & UI",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "url", Placeholder = "https://...",
        MaxLength = 500, IsActive = true
    },
    new RobotAbility {
        Id = 3, RobotTypeId = 1,
        Key = "themeAssets", Label = "Theme Assets",
        Description = "Cấu hình giao diện (banner/background/color).",
        DataType = "json", IsRequired = false, AbilityGroup = "Branding & UI",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""bannerUrl"":{""type"":""string""},
            ""backgroundUrl"":{""type"":""string""},
            ""primaryColor"":{""type"":""string""},
            ""secondaryColor"":{""type"":""string""}
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 4, RobotTypeId = 1,
        Key = "welcomeScreenText", Label = "Welcome Screen Text",
        Description = "Text chào mừng hiển thị trên màn hình.",
        DataType = "string", IsRequired = false, AbilityGroup = "Branding & UI",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true, // minor tweak allowed
        UiControl = "textarea", Placeholder = "Chào mừng bạn đến với...",
        MaxLength = 300, IsActive = true
    },
    new RobotAbility {
        Id = 5, RobotTypeId = 1,
        Key = "ctaQrUrl", Label = "CTA QR URL",
        Description = "Link/QR điều hướng khách (landing page, form...).",
        DataType = "string", IsRequired = false, AbilityGroup = "Branding & UI",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true, // minor swap allowed
        UiControl = "url", Placeholder = "https://...",
        MaxLength = 500, IsActive = true
    },
    new RobotAbility {
        Id = 6, RobotTypeId = 1,
        Key = "sponsorAssets", Label = "Sponsor Assets",
        Description = "Danh sách asset tài trợ hiển thị luân phiên.",
        DataType = "json", IsRequired = false, AbilityGroup = "Branding & UI",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{ ""type"":""array"", ""items"":{""type"":""string""} }",
        IsActive = true
    },

    // --- Greeting / Conversation Script ---
    new RobotAbility {
        Id = 7, RobotTypeId = 1,
        Key = "greetingScript", Label = "Greeting Script",
        Description = "Kịch bản chào hỏi / giới thiệu ngắn.",
        DataType = "string", IsRequired = false, AbilityGroup = "Greeting & Script",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "textarea", Placeholder = "Xin chào quý khách...",
        MaxLength = 2000, IsActive = true
    },
    new RobotAbility {
        Id = 8, RobotTypeId = 1,
        Key = "languages", Label = "Languages",
        Description = "Ngôn ngữ sử dụng khi chào hỏi / hướng dẫn.",
        DataType = "enum[]", IsRequired = true, AbilityGroup = "Greeting & Script",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "multiSelect",
        OptionsJson = @"[""VI"",""EN"",""JP"",""KR"",""CN""]",
        IsActive = true
    },
    new RobotAbility {
        Id = 9, RobotTypeId = 1,
        Key = "voiceProfile", Label = "Voice Profile",
        Description = "Cấu hình giọng nói (tốc độ/độ cao...).",
        DataType = "json", IsRequired = false, AbilityGroup = "Greeting & Script",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""voiceName"":{""type"":""string""},
            ""rate"":{""type"":""number"",""minimum"":0.5,""maximum"":2.0},
            ""pitch"":{""type"":""number"",""minimum"":-10,""maximum"":10},
            ""volume"":{""type"":""number"",""minimum"":0,""maximum"":100}
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 10, RobotTypeId = 1,
        Key = "faqItems", Label = "FAQ Items",
        Description = "Danh sách câu hỏi thường gặp (Q/A + keywords).",
        DataType = "json", IsRequired = false, AbilityGroup = "Greeting & Script",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""question"":{""type"":""string""},
              ""answer"":{""type"":""string""},
              ""keywords"":{""type"":""array"",""items"":{""type"":""string""}}
            },
            ""required"":[""question"",""answer""]
          }
        }",
        IsActive = true
    },

    // --- Check-in / Lead capture (optional) ---
    new RobotAbility {
        Id = 11, RobotTypeId = 1,
        Key = "checkinMode", Label = "Check-in Mode",
        Description = "Chế độ check-in tại sự kiện.",
        DataType = "enum", IsRequired = true, AbilityGroup = "Check-in & Lead",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "select",
        OptionsJson = @"[""None"",""QR"",""Form""]",
        IsActive = true
    },
    new RobotAbility {
        Id = 12, RobotTypeId = 1,
        Key = "leadFormFields", Label = "Lead Form Fields",
        Description = "Các trường thu thập thông tin khách.",
        DataType = "enum[]", IsRequired = false, AbilityGroup = "Check-in & Lead",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "multiSelect",
        OptionsJson = @"[""Name"",""Phone"",""Email"",""Company"",""Title""]",
        IsActive = true
    },
    new RobotAbility {
        Id = 13, RobotTypeId = 1,
        Key = "privacyNoticeText", Label = "Privacy Notice Text",
        Description = "Thông báo quyền riêng tư khi thu thập dữ liệu.",
        DataType = "string", IsRequired = false, AbilityGroup = "Check-in & Lead",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "textarea", Placeholder = "Thông tin của bạn sẽ được sử dụng để...",
        MaxLength = 2000, IsActive = true
    },

    // --- Navigation / POI (optional) ---
    new RobotAbility {
        Id = 14, RobotTypeId = 1,
        Key = "pois", Label = "Points of Interest (POI)",
        Description = "Danh sách điểm đến/booth để robot hướng dẫn.",
        DataType = "json", IsRequired = false, AbilityGroup = "Navigation",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false, // map phức tạp có thể ảnh hưởng giá
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""name"":{""type"":""string""},
              ""description"":{""type"":""string""},
              ""locationHint"":{""type"":""string""}
            },
            ""required"":[""name""]
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 15, RobotTypeId = 1,
        Key = "navigationRules", Label = "Navigation Rules",
        Description = "Quy tắc điều hướng (tốc độ, vùng cấm...).",
        DataType = "json", IsRequired = false, AbilityGroup = "Navigation",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = true,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""maxSpeed"":{""type"":""number"",""minimum"":0.1,""maximum"":2.0},
            ""noGoZones"":{""type"":""array"",""items"":{""type"":""string""}},
            ""preferredPaths"":{""type"":""array"",""items"":{""type"":""string""}}
          }
        }",
        IsActive = true
    },

    // =========================================================
    // RoboTypeId = 2 (Performance Robot)
    // =========================================================

    // --- Show Set (LOCK + PRICE IMPACTING) ---
    new RobotAbility {
        Id = 16, RobotTypeId = 2,
        Key = "showSets", Label = "Show Sets",
        Description = "Danh sách show set (nhạc + choreography + thời lượng + lặp).",
        DataType = "json", IsRequired = true, AbilityGroup = "Show Set",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""setName"":{""type"":""string""},
              ""musicTrackUrl"":{""type"":""string""},
              ""choreographyId"":{""type"":""string""},
              ""durationSec"":{""type"":""integer"",""minimum"":10},
              ""repeatCount"":{""type"":""integer"",""minimum"":1}
            },
            ""required"":[""setName"",""durationSec""]
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 17, RobotTypeId = 2,
        Key = "showOrder", Label = "Show Order",
        Description = "Thứ tự chạy các set (theo index).",
        DataType = "json", IsRequired = false, AbilityGroup = "Show Set",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{ ""type"":""array"", ""items"":{""type"":""integer"",""minimum"":0} }",
        IsActive = true
    },

    // --- Trigger / Cue (LOCK) ---
    new RobotAbility {
        Id = 18, RobotTypeId = 2,
        Key = "triggerMode", Label = "Trigger Mode",
        Description = "Cách kích hoạt biểu diễn.",
        DataType = "enum", IsRequired = true, AbilityGroup = "Cues & Triggers",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "select",
        OptionsJson = @"[""Manual"",""Scheduled"",""RemoteSignal""]",
        IsActive = true
    },
    new RobotAbility {
        Id = 19, RobotTypeId = 2,
        Key = "cuePoints", Label = "Cue Points",
        Description = "Các cue/timecode điều khiển hành động trong show.",
        DataType = "json", IsRequired = false, AbilityGroup = "Cues & Triggers",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false, // tích hợp cue phức tạp có thể tăng giá
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""timecodeSec"":{""type"":""integer"",""minimum"":0},
              ""action"":{""type"":""string""},
              ""note"":{""type"":""string""}
            },
            ""required"":[""timecodeSec"",""action""]
          }
        }",
        IsActive = true
    },

    // --- Stage & Safety ---
    new RobotAbility {
        Id = 20, RobotTypeId = 2,
        Key = "stageZone", Label = "Stage Zone",
        Description = "Khu vực sân khấu và khoảng cách an toàn.",
        DataType = "json", IsRequired = true, AbilityGroup = "Stage & Safety",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""widthM"":{""type"":""number"",""minimum"":1},
            ""depthM"":{""type"":""number"",""minimum"":1},
            ""safeDistanceM"":{""type"":""number"",""minimum"":0.5}
          },
          ""required"":[""widthM"",""depthM"",""safeDistanceM""]
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 21, RobotTypeId = 2,
        Key = "safetyLimits", Label = "Safety Limits",
        Description = "Giới hạn an toàn (tốc độ khớp, góc tay chân...).",
        DataType = "json", IsRequired = false, AbilityGroup = "Stage & Safety",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""maxJointSpeed"":{""type"":""number"",""minimum"":0.1,""maximum"":2.0},
            ""maxLimbAngle"":{""type"":""number"",""minimum"":10,""maximum"":180},
            ""emergencyStopRequired"":{""type"":""boolean""}
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 22, RobotTypeId = 2,
        Key = "rehearsalRequired", Label = "Rehearsal Required",
        Description = "Có yêu cầu rehearsal trước giờ chạy show hay không.",
        DataType = "bool", IsRequired = true, AbilityGroup = "Stage & Safety",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false,
        UiControl = "switch",
        IsActive = true
    },
    new RobotAbility {
        Id = 23, RobotTypeId = 2,
        Key = "riskLevel", Label = "Risk Level",
        Description = "Mức độ rủi ro (do staff set) để phục vụ quản trị an toàn.",
        DataType = "enum", IsRequired = true, AbilityGroup = "Stage & Safety",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "select",
        OptionsJson = @"[""Low"",""Medium"",""High""]",
        IsActive = true
    },

    // --- Visual Style ---
    new RobotAbility {
        Id = 24, RobotTypeId = 2,
        Key = "costumeOrLedTheme", Label = "Costume/LED Theme",
        Description = "Theme trang phục/LED cho robot (nếu có).",
        DataType = "string", IsRequired = false, AbilityGroup = "Visual Style",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false,
        UiControl = "text", Placeholder = "VD: Neon / Tết / Christmas...",
        MaxLength = 200, IsActive = true
    },
    new RobotAbility {
        Id = 25, RobotTypeId = 2,
        Key = "introOutroLines", Label = "Intro/Outro Lines",
        Description = "1-2 câu chào mở đầu/kết thúc show.",
        DataType = "string", IsRequired = false, AbilityGroup = "Visual Style",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "textarea", Placeholder = "Xin chào quý khách...",
        MaxLength = 500, IsActive = true
    },

    // =========================================================
    // RoboTypeId = 3 (Host Robot)
    // =========================================================

    // --- Script timeline (LOCK, price impact if complex) ---
    new RobotAbility {
        Id = 26, RobotTypeId = 3,
        Key = "scriptBlocks", Label = "Script Blocks",
        Description = "Kịch bản theo timeline (blockTitle/text/language/duration).",
        DataType = "json", IsRequired = true, AbilityGroup = "Script Timeline",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""blockTitle"":{""type"":""string""},
              ""timecode"":{""type"":""string""},
              ""text"":{""type"":""string""},
              ""language"":{""type"":""string""},
              ""estimatedDurationSec"":{""type"":""integer"",""minimum"":5},
              ""interactionPrompts"":{""type"":""array"",""items"":{""type"":""string""}}
            },
            ""required"":[""blockTitle"",""text""]
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 27, RobotTypeId = 3,
        Key = "pronunciationDict", Label = "Pronunciation Dictionary",
        Description = "Từ điển phát âm (term/phonetic) cho tên riêng, thương hiệu.",
        DataType = "json", IsRequired = false, AbilityGroup = "Voice & Pronunciation",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""term"":{""type"":""string""},
              ""phonetic"":{""type"":""string""}
            },
            ""required"":[""term"",""phonetic""]
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 28, RobotTypeId = 3,
        Key = "voiceProfile", Label = "Voice Profile",
        Description = "Cấu hình giọng MC (rate/pitch/volume).",
        DataType = "json", IsRequired = false, AbilityGroup = "Voice & Pronunciation",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""voiceName"":{""type"":""string""},
            ""rate"":{""type"":""number"",""minimum"":0.5,""maximum"":2.0},
            ""pitch"":{""type"":""number"",""minimum"":-10,""maximum"":10},
            ""volume"":{""type"":""number"",""minimum"":0,""maximum"":100}
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 29, RobotTypeId = 3,
        Key = "volumeRules", Label = "Volume Rules",
        Description = "Quy tắc âm lượng theo ngữ cảnh (quiet hours...).",
        DataType = "json", IsRequired = false, AbilityGroup = "Voice & Pronunciation",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""defaultVolume"":{""type"":""integer"",""minimum"":0,""maximum"":100},
            ""quietHoursVolume"":{""type"":""integer"",""minimum"":0,""maximum"":100}
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 30, RobotTypeId = 3,
        Key = "screenAssets", Label = "On-screen Assets",
        Description = "Asset hiển thị (QR/Image/Slide + thời lượng).",
        DataType = "json", IsRequired = false, AbilityGroup = "On-screen Assets",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""type"":{""type"":""string""},
              ""url"":{""type"":""string""},
              ""displayDurationSec"":{""type"":""integer"",""minimum"":1}
            },
            ""required"":[""type"",""url""]
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 31, RobotTypeId = 3,
        Key = "countdownSettings", Label = "Countdown Settings",
        Description = "Cấu hình countdown (nếu dùng).",
        DataType = "json", IsRequired = false, AbilityGroup = "On-screen Assets",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""enabled"":{""type"":""boolean""},
            ""targetTime"":{""type"":""string""}
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 32, RobotTypeId = 3,
        Key = "humanMcPresent", Label = "Human MC Present",
        Description = "Có MC người phối hợp hay không.",
        DataType = "bool", IsRequired = true, AbilityGroup = "Co-host Mode",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "switch",
        IsActive = true
    },
    new RobotAbility {
        Id = 33, RobotTypeId = 3,
        Key = "handoffCues", Label = "Handoff Cues",
        Description = "Cue chuyển giao giữa robot và MC người.",
        DataType = "json", IsRequired = false, AbilityGroup = "Co-host Mode",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""cue"":{""type"":""string""},
              ""who"":{""type"":""string""}
            },
            ""required"":[""cue"",""who""]
          }
        }",
        OptionsJson = @"[""Robot"",""Human""]",
        IsActive = true
    },

    // =========================================================
    // RoboTypeId = 4 (Promotion Robot)
    // =========================================================

    // --- Ad Playlist (LOCK, price impact if content production) ---
    new RobotAbility {
        Id = 34, RobotTypeId = 4,
        Key = "adPlaylist", Label = "Ad Playlist",
        Description = "Playlist quảng cáo (image/video + duration + order).",
        DataType = "json", IsRequired = true, AbilityGroup = "Ad Playlist",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""assetUrl"":{""type"":""string""},
              ""assetType"":{""type"":""string""},
              ""durationSec"":{""type"":""integer"",""minimum"":1},
              ""order"":{""type"":""integer"",""minimum"":0}
            },
            ""required"":[""assetUrl"",""assetType"",""durationSec""]
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 35, RobotTypeId = 4,
        Key = "scheduleRules", Label = "Schedule Rules",
        Description = "Quy tắc chạy playlist (khung giờ, interval, peak mode).",
        DataType = "json", IsRequired = false, AbilityGroup = "Ad Playlist",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""start"":{""type"":""string""},
            ""end"":{""type"":""string""},
            ""peakMode"":{""type"":""boolean""},
            ""intervalSec"":{""type"":""integer"",""minimum"":5}
          }
        }",
        IsActive = true
    },

    // --- Audio & Announcement ---
    new RobotAbility {
        Id = 36, RobotTypeId = 4,
        Key = "audioPlaylist", Label = "Audio Playlist",
        Description = "Danh sách audio (nhạc nền) nếu sử dụng.",
        DataType = "json", IsRequired = false, AbilityGroup = "Audio & Announcement",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{ ""type"":""array"", ""items"":{""type"":""string""} }",
        IsActive = true
    },
    new RobotAbility {
        Id = 37, RobotTypeId = 4,
        Key = "announcementScript", Label = "Announcement Script",
        Description = "Script thông báo/mời chào tại booth.",
        DataType = "string", IsRequired = false, AbilityGroup = "Audio & Announcement",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "textarea", Placeholder = "Mời quý khách ghé booth...",
        MaxLength = 2000, IsActive = true
    },
    new RobotAbility {
        Id = 38, RobotTypeId = 4,
        Key = "volumeRules", Label = "Volume Rules",
        Description = "Cấu hình âm lượng phát tại booth.",
        DataType = "json", IsRequired = false, AbilityGroup = "Audio & Announcement",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""object"",
          ""properties"":{
            ""defaultVolume"":{""type"":""integer"",""minimum"":0,""maximum"":100}
          }
        }",
        IsActive = true
    },

    // --- CTA / Lead ---
    new RobotAbility {
        Id = 39, RobotTypeId = 4,
        Key = "ctaQrUrl", Label = "CTA QR URL",
        Description = "Link/QR cho CTA (landing page, đăng ký...).",
        DataType = "string", IsRequired = false, AbilityGroup = "CTA & Lead",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "url", Placeholder = "https://...",
        MaxLength = 500, IsActive = true
    },
    new RobotAbility {
        Id = 40, RobotTypeId = 4,
        Key = "ctaText", Label = "CTA Text",
        Description = "Text CTA hiển thị trên màn hình/booth.",
        DataType = "string", IsRequired = false, AbilityGroup = "CTA & Lead",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "text", Placeholder = "Quét QR để nhận ưu đãi!",
        MaxLength = 200, IsActive = true
    },
    new RobotAbility {
        Id = 41, RobotTypeId = 4,
        Key = "voucherRule", Label = "Voucher Rule",
        Description = "Luật voucher/ưu đãi (nếu có).",
        DataType = "string", IsRequired = false, AbilityGroup = "CTA & Lead",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false,
        UiControl = "textarea", Placeholder = "VD: Giảm 10% cho 100 khách đầu tiên...",
        MaxLength = 2000, IsActive = true
    },

    // --- Booth Route / Movement ---
    new RobotAbility {
        Id = 42, RobotTypeId = 4,
        Key = "routeMode", Label = "Route Mode",
        Description = "Chế độ di chuyển tại booth.",
        DataType = "enum", IsRequired = true, AbilityGroup = "Booth Route",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = false,
        UiControl = "select",
        OptionsJson = @"[""Static"",""Patrol""]",
        IsActive = true
    },
    new RobotAbility {
        Id = 43, RobotTypeId = 4,
        Key = "routePoints", Label = "Route Points",
        Description = "Các điểm dừng khi robot patrol.",
        DataType = "json", IsRequired = false, AbilityGroup = "Booth Route",
        LockAtCutoff = true, IsPriceImpacting = true, IsOnSiteAdjustable = false,
        UiControl = "jsonEditor",
        JsonSchema = @"{
          ""type"":""array"",
          ""items"":{
            ""type"":""object"",
            ""properties"":{
              ""name"":{""type"":""string""},
              ""stopDurationSec"":{""type"":""integer"",""minimum"":1}
            },
            ""required"":[""name"",""stopDurationSec""]
          }
        }",
        IsActive = true
    },
    new RobotAbility {
        Id = 44, RobotTypeId = 4,
        Key = "avoidZones", Label = "Avoid Zones",
        Description = "Khu vực tránh (nếu có).",
        DataType = "json", IsRequired = false, AbilityGroup = "Booth Route",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "jsonEditor",
        JsonSchema = @"{ ""type"":""array"", ""items"":{""type"":""string""} }",
        IsActive = true
    },
    new RobotAbility {
        Id = 45, RobotTypeId = 4,
        Key = "maxSpeed", Label = "Max Speed",
        Description = "Tốc độ tối đa (m/s) khi di chuyển.",
        DataType = "number", IsRequired = false, AbilityGroup = "Booth Route",
        LockAtCutoff = true, IsPriceImpacting = false, IsOnSiteAdjustable = true,
        UiControl = "number",
        Min = 0.1m, Max = 2.0m,
        IsActive = true
    }
);

        modelBuilder.Entity<RobotAbility>()
            .Property(x => x.Min).HasPrecision(18, 2);

        modelBuilder.Entity<RobotAbility>()
            .Property(x => x.Max).HasPrecision(18, 2);

        modelBuilder.Entity<TypesOfRobo>().HasKey(tor => new { tor.RobotId, tor.RoboTypeId });

        modelBuilder.Entity<RobotInGroup>().HasKey(rig => new { rig.ActivityTypeGroupId, rig.RobotId });

        modelBuilder.Entity<RobotTypeOfActivity>().HasKey(rtoe => new { rtoe.ActivityTypeId, rtoe.RoboTypeId });

        // modelBuilder.Entity<ActivityType>()
        //     .HasOne(a => a.EventActivity)
        //     .WithMany(e => e.ActivityTypes)
        //     .HasForeignKey(a => a.EventActivityId)
        //     .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ActivityTypeGroup>().Navigation(a => a.ActivityType).AutoInclude();
        // modelBuilder.Entity<ActivityType>().Navigation(a => a.EventActivity).AutoInclude();
        
        base.OnModelCreating(modelBuilder);
    }
}