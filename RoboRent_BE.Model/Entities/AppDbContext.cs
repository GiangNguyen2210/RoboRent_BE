using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RoboRent_BE.Model.Entities;

public partial class AppDbContext : IdentityDbContext<ModifyIdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<RentalContract> RentalContracts { get; set; } = null!;

    public virtual DbSet<EventActivity> EventActivities { get; set; } = null!;

    public virtual DbSet<Account> Accounts { get; set; } = null!;

    public virtual DbSet<Robot> Robots { get; set; } = null!;

    public virtual DbSet<RoboType> RoboTypes { get; set; } = null!;

    public virtual DbSet<TypesOfRobo> TypesOfRobos { get; set; } = null!;

    public virtual DbSet<ActivityType> ActivityTypes { get; set; } = null!;

    public virtual DbSet<Rental> Rentals { get; set; } = null!;

    public virtual DbSet<RentalDetail> RentalDetails { get; set; } = null!;

    public virtual DbSet<PriceQuote> PriceQuotes { get; set; } = null!;

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "Staff", NormalizedName = "STAFF" },
            new IdentityRole { Id = "3", Name = "Customer", NormalizedName = "CUSTOMER" },
            new IdentityRole { Id = "4", Name = "Manager", NormalizedName = "MANAGER" }
        );

        modelBuilder.Entity<ActivityType>().HasData(
            // CATEGORY 1: PERFORMANCE
            new ActivityType { Id = 1, EventActivityId = 1, Name = "Solo Singing", IsDeleted = false },
            new ActivityType { Id = 2, EventActivityId = 1, Name = "Duet Singing", IsDeleted = false },
            new ActivityType { Id = 3, EventActivityId = 1, Name = "Birthday Song Performance", IsDeleted = false },
            new ActivityType
                { Id = 4, EventActivityId = 1, Name = "Instrument Simulation Performance", IsDeleted = false },
            new ActivityType { Id = 5, EventActivityId = 1, Name = "Solo Dance", IsDeleted = false },
            new ActivityType { Id = 6, EventActivityId = 1, Name = "Group Robot Dance", IsDeleted = false },
            new ActivityType { Id = 7, EventActivityId = 1, Name = "Kids Dance Performance", IsDeleted = false },
            new ActivityType { Id = 8, EventActivityId = 1, Name = "Drama Acting", IsDeleted = false },
            new ActivityType { Id = 9, EventActivityId = 1, Name = "Comedy Performance", IsDeleted = false },
            new ActivityType
                { Id = 10, EventActivityId = 1, Name = "Theme Performance (Holiday/Seasonal)", IsDeleted = false },

            // CATEGORY 2: PRESENTATION & HOSTING
            new ActivityType { Id = 11, EventActivityId = 2, Name = "Main Event Hosting (MC)", IsDeleted = false },
            new ActivityType { Id = 12, EventActivityId = 2, Name = "Supporting MC", IsDeleted = false },
            new ActivityType { Id = 13, EventActivityId = 2, Name = "Product Presentation", IsDeleted = false },
            new ActivityType { Id = 14, EventActivityId = 2, Name = "Corporate Introduction", IsDeleted = false },
            new ActivityType { Id = 15, EventActivityId = 2, Name = "Educational Presentation", IsDeleted = false },
            new ActivityType { Id = 16, EventActivityId = 2, Name = "Ceremony Opening Speech", IsDeleted = false },
            new ActivityType { Id = 17, EventActivityId = 2, Name = "Guest Introduction", IsDeleted = false },
            new ActivityType { Id = 18, EventActivityId = 2, Name = "Event Flow Guidance", IsDeleted = false },

            // CATEGORY 3: STORYTELLING & INTERACTION
            new ActivityType { Id = 19, EventActivityId = 3, Name = "Fairy Tale Storytelling", IsDeleted = false },
            new ActivityType { Id = 20, EventActivityId = 3, Name = "Inspirational Storytelling", IsDeleted = false },
            new ActivityType { Id = 21, EventActivityId = 3, Name = "Historical Storytelling", IsDeleted = false },
            new ActivityType { Id = 22, EventActivityId = 3, Name = "Poetry Reading", IsDeleted = false },
            new ActivityType { Id = 23, EventActivityId = 3, Name = "Q&A Interaction", IsDeleted = false },
            new ActivityType { Id = 24, EventActivityId = 3, Name = "AI Scripted Conversation", IsDeleted = false },
            new ActivityType { Id = 25, EventActivityId = 3, Name = "Audience Interview", IsDeleted = false },
            new ActivityType { Id = 26, EventActivityId = 3, Name = "Trivia Quiz Hosting", IsDeleted = false },
            new ActivityType { Id = 27, EventActivityId = 3, Name = "Mini Games with Audience", IsDeleted = false },

            // CATEGORY 4: ENTERTAINMENT SUPPORT
            new ActivityType { Id = 28, EventActivityId = 4, Name = "Greeting Guests", IsDeleted = false },
            new ActivityType { Id = 29, EventActivityId = 4, Name = "Selfie with Guests", IsDeleted = false },
            new ActivityType { Id = 30, EventActivityId = 4, Name = "Guest Check-in Support", IsDeleted = false },
            new ActivityType { Id = 31, EventActivityId = 4, Name = "Event Area Tour Guidance", IsDeleted = false },
            new ActivityType { Id = 32, EventActivityId = 4, Name = "Crowd Engagement", IsDeleted = false },
            new ActivityType { Id = 33, EventActivityId = 4, Name = "Short Dance Break", IsDeleted = false },
            new ActivityType { Id = 34, EventActivityId = 4, Name = "Birthday Celebration Support", IsDeleted = false },
            new ActivityType
                { Id = 35, EventActivityId = 4, Name = "Children Entertainment Interaction", IsDeleted = false },

            // CATEGORY 5: PROMOTION & MARKETING
            new ActivityType { Id = 36, EventActivityId = 5, Name = "Product Review", IsDeleted = false },
            new ActivityType { Id = 37, EventActivityId = 5, Name = "Promotional Announcement", IsDeleted = false },
            new ActivityType { Id = 38, EventActivityId = 5, Name = "Sampling Distribution", IsDeleted = false },
            new ActivityType { Id = 39, EventActivityId = 5, Name = "Brand Activation Speech", IsDeleted = false },
            new ActivityType
                { Id = 40, EventActivityId = 5, Name = "Customer Product Consultation", IsDeleted = false },
            new ActivityType { Id = 41, EventActivityId = 5, Name = "Collect Customer Information", IsDeleted = false },
            new ActivityType { Id = 42, EventActivityId = 5, Name = "Feature Demonstration", IsDeleted = false }
        );


        modelBuilder.Entity<EventActivity>().HasData(
            new EventActivity
                { Id = 1, Name = "Performance", Description = "Robot biểu diễn trước khán giả", IsDeleted = false },
            new EventActivity
            {
                Id = 2, Name = "Presentation & Hosting", Description = "Robot giao tiếp, nói, dẫn chương trình",
                IsDeleted = false
            },
            new EventActivity
            {
                Id = 3, Name = "Storytelling & Interaction", Description = "Robot tương tác nội dung với khán giả",
                IsDeleted = false
            },
            new EventActivity
            {
                Id = 4, Name = "Entertainment Support",
                Description = "Robot hỗ trợ event nhưng không phải “tiết mục biểu diễn chính”", IsDeleted = false
            },
            new EventActivity
            {
                Id = 5, Name = "Promotion & Marketing", Description = "Hoạt động marketing, bán hàng, PR",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<RoboType>().HasData(
            // CATEGORY 1: PERFORMANCE
            new RoboType
            {
                Id = 1, TypeName = "Humanoid Performance Robot",
                Description = "Robot hình người, có thể nhảy múa, di chuyển linh hoạt", IsDeleted = false
            },
            new RoboType
            {
                Id = 2, TypeName = "Dance / Choreography Robot",
                Description = "Robot chuyên thực hiện nhảy múa, hành động đồng bộ theo nhóm", IsDeleted = false
            },
            new RoboType
            {
                Id = 3, TypeName = "Acting Robot / Drama Robot",
                Description = "Robot diễn xuất kịch, có khả năng cử chỉ, biểu cảm", IsDeleted = false
            },

            // CATEGORY 2: PRESENTATION & HOSTING
            new RoboType
            {
                Id = 5, TypeName = "Host / MC Robot",
                Description = "Robot dẫn chương trình, có thể giao tiếp và tương tác với khán giả", IsDeleted = false
            },
            new RoboType
            {
                Id = 6, TypeName = "Presentation Robot",
                Description = "Robot trình bày sản phẩm, nội dung, hỗ trợ thuyết minh tại sự kiện", IsDeleted = false
            },
            new RoboType
            {
                Id = 7, TypeName = "Telepresence Robot",
                Description = "Robot cho phép thuyết trình, giao tiếp từ xa thông qua điều khiển", IsDeleted = false
            },

            // CATEGORY 3: STORYTELLING & INTERACTION
            new RoboType
            {
                Id = 8, TypeName = "Storytelling Robot",
                Description = "Robot kể chuyện, tương tác với người nghe, phù hợp cho trẻ em và giáo dục",
                IsDeleted = false
            },
            new RoboType
            {
                Id = 9, TypeName = "Interaction / Social Robot",
                Description = "Robot giao tiếp, hỏi đáp, tương tác và tham gia trò chơi với khán giả", IsDeleted = false
            },
            new RoboType
            {
                Id = 10, TypeName = "Game / Quiz Robot",
                Description = "Robot dẫn các trò chơi nhỏ, quiz, minigame tương tác cùng khán giả", IsDeleted = false
            },

            // CATEGORY 4: ENTERTAINMENT SUPPORT
            new RoboType
            {
                Id = 11, TypeName = "Hospitality Robot",
                Description = "Robot chào đón khách, hướng dẫn khách, hỗ trợ check-in tại sự kiện", IsDeleted = false
            },
            new RoboType
            {
                Id = 12, TypeName = "Selfie / Photo Bot",
                Description = "Robot hỗ trợ chụp ảnh với khách tham dự, tạo trải nghiệm tương tác vui vẻ",
                IsDeleted = false
            },
            new RoboType
            {
                Id = 13, TypeName = "Animatronic Robot",
                Description = "Robot dạng mascot hoặc hoạt hình chuyển động tạo không khí sự kiện", IsDeleted = false
            },

            // CATEGORY 5: PROMOTION & MARKETING
            new RoboType
            {
                Id = 14, TypeName = "Brand Activation Robot",
                Description = "Robot thu hút khách tại booth, kích hoạt thương hiệu và tạo điểm nhấn sự kiện",
                IsDeleted = false
            },
            new RoboType
            {
                Id = 15, TypeName = "Product Demo Robot",
                Description = "Robot giới thiệu tính năng sản phẩm, hướng dẫn dùng thử, demo công nghệ",
                IsDeleted = false
            },
            new RoboType
            {
                Id = 16, TypeName = "Promotional Robot",
                Description = "Robot quảng bá, mời chào, dẫn traffic và hỗ trợ hoạt động marketing", IsDeleted = false
            }
        );

        modelBuilder.Entity<Robot>().HasData(
            // Humanoid Performance Robots (HPR) - RoboTypeId = 1
            new Robot
            {
                Id = 1, RoboTypeId = 1, RobotName = "HPR-01", ModelName = "HPR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 2, RoboTypeId = 1, RobotName = "HPR-02", ModelName = "HPR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 3, RoboTypeId = 1, RobotName = "HPR-03", ModelName = "HPR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 4, RoboTypeId = 1, RobotName = "HPR-04", ModelName = "HPR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 5, RoboTypeId = 1, RobotName = "HPR-05", ModelName = "HPR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 6, RoboTypeId = 1, RobotName = "HPR-06", ModelName = "HPR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 7, RoboTypeId = 1, RobotName = "HPR-07", ModelName = "HPR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 8, RoboTypeId = 1, RobotName = "HPR-08", ModelName = "HPR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 9, RoboTypeId = 1, RobotName = "HPR-09", ModelName = "HPR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 10, RoboTypeId = 1, RobotName = "HPR-10", ModelName = "HPR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Dance / Choreography Robots (DCR) - RoboTypeId = 2
            new Robot
            {
                Id = 11, RoboTypeId = 2, RobotName = "DCR-01", ModelName = "DCR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 12, RoboTypeId = 2, RobotName = "DCR-02", ModelName = "DCR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 13, RoboTypeId = 2, RobotName = "DCR-03", ModelName = "DCR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 14, RoboTypeId = 2, RobotName = "DCR-04", ModelName = "DCR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 15, RoboTypeId = 2, RobotName = "DCR-05", ModelName = "DCR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 16, RoboTypeId = 2, RobotName = "DCR-06", ModelName = "DCR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 17, RoboTypeId = 2, RobotName = "DCR-07", ModelName = "DCR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 18, RoboTypeId = 2, RobotName = "DCR-08", ModelName = "DCR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 19, RoboTypeId = 2, RobotName = "DCR-09", ModelName = "DCR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 20, RoboTypeId = 2, RobotName = "DCR-10", ModelName = "DCR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Acting / Drama Robots (ARR) - RoboTypeId = 3
            new Robot
            {
                Id = 21, RoboTypeId = 3, RobotName = "ARR-01", ModelName = "ARR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 22, RoboTypeId = 3, RobotName = "ARR-02", ModelName = "ARR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 23, RoboTypeId = 3, RobotName = "ARR-03", ModelName = "ARR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 24, RoboTypeId = 3, RobotName = "ARR-04", ModelName = "ARR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 25, RoboTypeId = 3, RobotName = "ARR-05", ModelName = "ARR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 26, RoboTypeId = 3, RobotName = "ARR-06", ModelName = "ARR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 27, RoboTypeId = 3, RobotName = "ARR-07", ModelName = "ARR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 28, RoboTypeId = 3, RobotName = "ARR-08", ModelName = "ARR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 29, RoboTypeId = 3, RobotName = "ARR-09", ModelName = "ARR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 30, RoboTypeId = 3, RobotName = "ARR-10", ModelName = "ARR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

            // Host / MC Robots (HMR) - RoboTypeId = 5
            new Robot
            {
                Id = 41, RoboTypeId = 5, RobotName = "HMR-01", ModelName = "HMR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 42, RoboTypeId = 5, RobotName = "HMR-02", ModelName = "HMR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 43, RoboTypeId = 5, RobotName = "HMR-03", ModelName = "HMR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 44, RoboTypeId = 5, RobotName = "HMR-04", ModelName = "HMR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 45, RoboTypeId = 5, RobotName = "HMR-05", ModelName = "HMR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 46, RoboTypeId = 5, RobotName = "HMR-06", ModelName = "HMR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 47, RoboTypeId = 5, RobotName = "HMR-07", ModelName = "HMR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 48, RoboTypeId = 5, RobotName = "HMR-08", ModelName = "HMR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 49, RoboTypeId = 5, RobotName = "HMR-09", ModelName = "HMR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 50, RoboTypeId = 5, RobotName = "HMR-10", ModelName = "HMR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Presentation Robots (PRR) - RoboTypeId = 6
            new Robot
            {
                Id = 51, RoboTypeId = 6, RobotName = "PRR-01", ModelName = "PRR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 52, RoboTypeId = 6, RobotName = "PRR-02", ModelName = "PRR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 53, RoboTypeId = 6, RobotName = "PRR-03", ModelName = "PRR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 54, RoboTypeId = 6, RobotName = "PRR-04", ModelName = "PRR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 55, RoboTypeId = 6, RobotName = "PRR-05", ModelName = "PRR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 56, RoboTypeId = 6, RobotName = "PRR-06", ModelName = "PRR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 57, RoboTypeId = 6, RobotName = "PRR-07", ModelName = "PRR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 58, RoboTypeId = 6, RobotName = "PRR-08", ModelName = "PRR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 59, RoboTypeId = 6, RobotName = "PRR-09", ModelName = "PRR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 60, RoboTypeId = 6, RobotName = "PRR-10", ModelName = "PRR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Telepresence Robots (TPR) - RoboTypeId = 7
            new Robot
            {
                Id = 61, RoboTypeId = 7, RobotName = "TPR-01", ModelName = "TPR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 62, RoboTypeId = 7, RobotName = "TPR-02", ModelName = "TPR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 63, RoboTypeId = 7, RobotName = "TPR-03", ModelName = "TPR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 64, RoboTypeId = 7, RobotName = "TPR-04", ModelName = "TPR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 65, RoboTypeId = 7, RobotName = "TPR-05", ModelName = "TPR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 66, RoboTypeId = 7, RobotName = "TPR-06", ModelName = "TPR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 67, RoboTypeId = 7, RobotName = "TPR-07", ModelName = "TPR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 68, RoboTypeId = 7, RobotName = "TPR-08", ModelName = "TPR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 69, RoboTypeId = 7, RobotName = "TPR-09", ModelName = "TPR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 70, RoboTypeId = 7, RobotName = "TPR-10", ModelName = "TPR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Storytelling Robots (STR) - RoboTypeId = 8
            new Robot
            {
                Id = 71, RoboTypeId = 8, RobotName = "STR-01", ModelName = "STR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 72, RoboTypeId = 8, RobotName = "STR-02", ModelName = "STR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 73, RoboTypeId = 8, RobotName = "STR-03", ModelName = "STR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 74, RoboTypeId = 8, RobotName = "STR-04", ModelName = "STR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 75, RoboTypeId = 8, RobotName = "STR-05", ModelName = "STR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 76, RoboTypeId = 8, RobotName = "STR-06", ModelName = "STR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 77, RoboTypeId = 8, RobotName = "STR-07", ModelName = "STR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 78, RoboTypeId = 8, RobotName = "STR-08", ModelName = "STR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 79, RoboTypeId = 8, RobotName = "STR-09", ModelName = "STR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 80, RoboTypeId = 8, RobotName = "STR-10", ModelName = "STR-10", RobotStatus = "Stored",
                IsDeleted = false
            },
            // Interaction / Social Robots (ISR) - RoboTypeId = 9
            new Robot
            {
                Id = 81, RoboTypeId = 9, RobotName = "ISR-01", ModelName = "ISR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 82, RoboTypeId = 9, RobotName = "ISR-02", ModelName = "ISR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 83, RoboTypeId = 9, RobotName = "ISR-03", ModelName = "ISR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 84, RoboTypeId = 9, RobotName = "ISR-04", ModelName = "ISR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 85, RoboTypeId = 9, RobotName = "ISR-05", ModelName = "ISR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 86, RoboTypeId = 9, RobotName = "ISR-06", ModelName = "ISR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 87, RoboTypeId = 9, RobotName = "ISR-07", ModelName = "ISR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 88, RoboTypeId = 9, RobotName = "ISR-08", ModelName = "ISR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 89, RoboTypeId = 9, RobotName = "ISR-09", ModelName = "ISR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 90, RoboTypeId = 9, RobotName = "ISR-10", ModelName = "ISR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Game / Quiz Robots (GQR) - RoboTypeId = 10
            new Robot
            {
                Id = 91, RoboTypeId = 10, RobotName = "GQR-01", ModelName = "GQR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 92, RoboTypeId = 10, RobotName = "GQR-02", ModelName = "GQR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 93, RoboTypeId = 10, RobotName = "GQR-03", ModelName = "GQR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 94, RoboTypeId = 10, RobotName = "GQR-04", ModelName = "GQR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 95, RoboTypeId = 10, RobotName = "GQR-05", ModelName = "GQR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 96, RoboTypeId = 10, RobotName = "GQR-06", ModelName = "GQR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 97, RoboTypeId = 10, RobotName = "GQR-07", ModelName = "GQR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 98, RoboTypeId = 10, RobotName = "GQR-08", ModelName = "GQR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 99, RoboTypeId = 10, RobotName = "GQR-09", ModelName = "GQR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 100, RoboTypeId = 10, RobotName = "GQR-10", ModelName = "GQR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Hospitality Robots (HSR) - RoboTypeId = 11
            new Robot
            {
                Id = 101, RoboTypeId = 11, RobotName = "HSR-01", ModelName = "HSR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 102, RoboTypeId = 11, RobotName = "HSR-02", ModelName = "HSR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 103, RoboTypeId = 11, RobotName = "HSR-03", ModelName = "HSR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 104, RoboTypeId = 11, RobotName = "HSR-04", ModelName = "HSR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 105, RoboTypeId = 11, RobotName = "HSR-05", ModelName = "HSR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 106, RoboTypeId = 11, RobotName = "HSR-06", ModelName = "HSR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 107, RoboTypeId = 11, RobotName = "HSR-07", ModelName = "HSR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 108, RoboTypeId = 11, RobotName = "HSR-08", ModelName = "HSR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 109, RoboTypeId = 11, RobotName = "HSR-09", ModelName = "HSR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 110, RoboTypeId = 11, RobotName = "HSR-10", ModelName = "HSR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Selfie / Photo Bots (SPB) - RoboTypeId = 12
            new Robot
            {
                Id = 111, RoboTypeId = 12, RobotName = "SPB-01", ModelName = "SPB-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 112, RoboTypeId = 12, RobotName = "SPB-02", ModelName = "SPB-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 113, RoboTypeId = 12, RobotName = "SPB-03", ModelName = "SPB-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 114, RoboTypeId = 12, RobotName = "SPB-04", ModelName = "SPB-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 115, RoboTypeId = 12, RobotName = "SPB-05", ModelName = "SPB-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 116, RoboTypeId = 12, RobotName = "SPB-06", ModelName = "SPB-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 117, RoboTypeId = 12, RobotName = "SPB-07", ModelName = "SPB-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 118, RoboTypeId = 12, RobotName = "SPB-08", ModelName = "SPB-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 119, RoboTypeId = 12, RobotName = "SPB-09", ModelName = "SPB-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 120, RoboTypeId = 12, RobotName = "SPB-10", ModelName = "SPB-10", RobotStatus = "Stored",
                IsDeleted = false
            },
            // Animatronic Robots (ANR) - RoboTypeId = 13
            new Robot
            {
                Id = 121, RoboTypeId = 13, RobotName = "ANR-01", ModelName = "ANR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 122, RoboTypeId = 13, RobotName = "ANR-02", ModelName = "ANR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 123, RoboTypeId = 13, RobotName = "ANR-03", ModelName = "ANR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 124, RoboTypeId = 13, RobotName = "ANR-04", ModelName = "ANR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 125, RoboTypeId = 13, RobotName = "ANR-05", ModelName = "ANR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 126, RoboTypeId = 13, RobotName = "ANR-06", ModelName = "ANR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 127, RoboTypeId = 13, RobotName = "ANR-07", ModelName = "ANR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 128, RoboTypeId = 13, RobotName = "ANR-08", ModelName = "ANR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 129, RoboTypeId = 13, RobotName = "ANR-09", ModelName = "ANR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 130, RoboTypeId = 13, RobotName = "ANR-10", ModelName = "ANR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Brand Activation Robots (BAR) - RoboTypeId = 14
            new Robot
            {
                Id = 131, RoboTypeId = 14, RobotName = "BAR-01", ModelName = "BAR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 132, RoboTypeId = 14, RobotName = "BAR-02", ModelName = "BAR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 133, RoboTypeId = 14, RobotName = "BAR-03", ModelName = "BAR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 134, RoboTypeId = 14, RobotName = "BAR-04", ModelName = "BAR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 135, RoboTypeId = 14, RobotName = "BAR-05", ModelName = "BAR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 136, RoboTypeId = 14, RobotName = "BAR-06", ModelName = "BAR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 137, RoboTypeId = 14, RobotName = "BAR-07", ModelName = "BAR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 138, RoboTypeId = 14, RobotName = "BAR-08", ModelName = "BAR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 139, RoboTypeId = 14, RobotName = "BAR-09", ModelName = "BAR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 140, RoboTypeId = 14, RobotName = "BAR-10", ModelName = "BAR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Product Demo Robots (PDR) - RoboTypeId = 15
            new Robot
            {
                Id = 141, RoboTypeId = 15, RobotName = "PDR-01", ModelName = "PDR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 142, RoboTypeId = 15, RobotName = "PDR-02", ModelName = "PDR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 143, RoboTypeId = 15, RobotName = "PDR-03", ModelName = "PDR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 144, RoboTypeId = 15, RobotName = "PDR-04", ModelName = "PDR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 145, RoboTypeId = 15, RobotName = "PDR-05", ModelName = "PDR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 146, RoboTypeId = 15, RobotName = "PDR-06", ModelName = "PDR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 147, RoboTypeId = 15, RobotName = "PDR-07", ModelName = "PDR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 148, RoboTypeId = 15, RobotName = "PDR-08", ModelName = "PDR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 149, RoboTypeId = 15, RobotName = "PDR-09", ModelName = "PDR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 150, RoboTypeId = 15, RobotName = "PDR-10", ModelName = "PDR-10", RobotStatus = "Stored",
                IsDeleted = false
            },

// Promotional Robots (PMR) - RoboTypeId = 16
            new Robot
            {
                Id = 151, RoboTypeId = 16, RobotName = "PMR-01", ModelName = "PMR-01", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 152, RoboTypeId = 16, RobotName = "PMR-02", ModelName = "PMR-02", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 153, RoboTypeId = 16, RobotName = "PMR-03", ModelName = "PMR-03", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 154, RoboTypeId = 16, RobotName = "PMR-04", ModelName = "PMR-04", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 155, RoboTypeId = 16, RobotName = "PMR-05", ModelName = "PMR-05", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 156, RoboTypeId = 16, RobotName = "PMR-06", ModelName = "PMR-06", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 157, RoboTypeId = 16, RobotName = "PMR-07", ModelName = "PMR-07", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 158, RoboTypeId = 16, RobotName = "PMR-08", ModelName = "PMR-08", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 159, RoboTypeId = 16, RobotName = "PMR-09", ModelName = "PMR-09", RobotStatus = "Stored",
                IsDeleted = false
            },
            new Robot
            {
                Id = 160, RoboTypeId = 16, RobotName = "PMR-10", ModelName = "PMR-10", RobotStatus = "Stored",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<RobotTypeOfActivity>().HasData(
            // CATEGORY 1: PERFORMANCE

            // Solo Singing
            new RobotTypeOfActivity { ActivityTypeId = 1, RoboTypeId = 1, Amount = 1 },

            // Duet Singing
            new RobotTypeOfActivity { ActivityTypeId = 2, RoboTypeId = 1, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 2, RoboTypeId = 2, Amount = 1 },

            // Birthday Song Performance
            new RobotTypeOfActivity { ActivityTypeId = 3, RoboTypeId = 1, Amount = 1 },

            // Instrument Simulation Performance
            new RobotTypeOfActivity { ActivityTypeId = 4, RoboTypeId = 1, Amount = 1 },

            // Solo Dance
            new RobotTypeOfActivity { ActivityTypeId = 5, RoboTypeId = 2, Amount = 1 },

            // Group Robot Dance
            new RobotTypeOfActivity { ActivityTypeId = 6, RoboTypeId = 2, Amount = 3 },
            new RobotTypeOfActivity { ActivityTypeId = 6, RoboTypeId = 1, Amount = 1 },

            // Kids Dance Performance
            new RobotTypeOfActivity { ActivityTypeId = 7, RoboTypeId = 2, Amount = 2 },
            new RobotTypeOfActivity { ActivityTypeId = 7, RoboTypeId = 1, Amount = 1 },

            // Drama Acting
            new RobotTypeOfActivity { ActivityTypeId = 8, RoboTypeId = 3, Amount = 2 },
            new RobotTypeOfActivity { ActivityTypeId = 8, RoboTypeId = 1, Amount = 1 },

            // Comedy Performance
            new RobotTypeOfActivity { ActivityTypeId = 9, RoboTypeId = 3, Amount = 2 },

            // Theme Performance (Holiday/Seasonal)
            new RobotTypeOfActivity { ActivityTypeId = 10, RoboTypeId = 1, Amount = 2 },
            new RobotTypeOfActivity { ActivityTypeId = 10, RoboTypeId = 2, Amount = 1 }
        );

        modelBuilder.Entity<RobotTypeOfActivity>().HasData(
            // CATEGORY 2: PRESENTATION & HOSTING

            // Main Event Hosting (MC)
            new RobotTypeOfActivity { ActivityTypeId = 11, RoboTypeId = 5, Amount = 1 },

            // Supporting MC
            new RobotTypeOfActivity { ActivityTypeId = 12, RoboTypeId = 5, Amount = 1 },

            // Product Presentation
            new RobotTypeOfActivity { ActivityTypeId = 13, RoboTypeId = 6, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 13, RoboTypeId = 7, Amount = 1 },

            // Corporate Introduction
            new RobotTypeOfActivity { ActivityTypeId = 14, RoboTypeId = 6, Amount = 1 },

            // Educational Presentation
            new RobotTypeOfActivity { ActivityTypeId = 15, RoboTypeId = 6, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 15, RoboTypeId = 7, Amount = 1 },

            // Ceremony Opening Speech
            new RobotTypeOfActivity { ActivityTypeId = 16, RoboTypeId = 5, Amount = 1 },

            // Guest Introduction
            new RobotTypeOfActivity { ActivityTypeId = 17, RoboTypeId = 5, Amount = 1 },

            // Event Flow Guidance
            new RobotTypeOfActivity { ActivityTypeId = 18, RoboTypeId = 5, Amount = 1 }
        );

        modelBuilder.Entity<RobotTypeOfActivity>().HasData(
            // CATEGORY 3: STORYTELLING & INTERACTION

            // Fairy Tale Storytelling
            new RobotTypeOfActivity { ActivityTypeId = 19, RoboTypeId = 8, Amount = 1 },

            // Inspirational Storytelling
            new RobotTypeOfActivity { ActivityTypeId = 20, RoboTypeId = 8, Amount = 1 },

            // Historical Storytelling
            new RobotTypeOfActivity { ActivityTypeId = 21, RoboTypeId = 8, Amount = 1 },

            // Poetry Reading
            new RobotTypeOfActivity { ActivityTypeId = 22, RoboTypeId = 8, Amount = 1 },

            // Q&A Interaction
            new RobotTypeOfActivity { ActivityTypeId = 23, RoboTypeId = 9, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 23, RoboTypeId = 8, Amount = 1 },

            // AI Scripted Conversation
            new RobotTypeOfActivity { ActivityTypeId = 24, RoboTypeId = 9, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 24, RoboTypeId = 8, Amount = 1 },

            // Audience Interview
            new RobotTypeOfActivity { ActivityTypeId = 25, RoboTypeId = 9, Amount = 1 },

            // Trivia Quiz Hosting
            new RobotTypeOfActivity { ActivityTypeId = 26, RoboTypeId = 10, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 26, RoboTypeId = 9, Amount = 1 },

            // Mini Games with Audience
            new RobotTypeOfActivity { ActivityTypeId = 27, RoboTypeId = 10, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 27, RoboTypeId = 9, Amount = 1 }
        );

        modelBuilder.Entity<RobotTypeOfActivity>().HasData(
            // CATEGORY 4: ENTERTAINMENT SUPPORT

            // Greeting Guests
            new RobotTypeOfActivity { ActivityTypeId = 28, RoboTypeId = 11, Amount = 1 },

            // Selfie with Guests
            new RobotTypeOfActivity { ActivityTypeId = 29, RoboTypeId = 12, Amount = 1 },

            // Guest Check-in Support
            new RobotTypeOfActivity { ActivityTypeId = 30, RoboTypeId = 11, Amount = 1 },

            // Event Area Tour Guidance
            new RobotTypeOfActivity { ActivityTypeId = 31, RoboTypeId = 11, Amount = 1 },

            // Crowd Engagement
            new RobotTypeOfActivity { ActivityTypeId = 32, RoboTypeId = 11, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 32, RoboTypeId = 12, Amount = 1 },

            // Short Dance Break
            new RobotTypeOfActivity { ActivityTypeId = 33, RoboTypeId = 13, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 33, RoboTypeId = 11, Amount = 1 },

            // Birthday Celebration Support
            new RobotTypeOfActivity { ActivityTypeId = 34, RoboTypeId = 11, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 34, RoboTypeId = 12, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 34, RoboTypeId = 13, Amount = 1 },

            // Children Entertainment Interaction
            new RobotTypeOfActivity { ActivityTypeId = 35, RoboTypeId = 13, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 35, RoboTypeId = 12, Amount = 1 }
        );

        modelBuilder.Entity<RobotTypeOfActivity>().HasData(
            // CATEGORY 5: PROMOTION & MARKETING

            // Product Review
            new RobotTypeOfActivity { ActivityTypeId = 36, RoboTypeId = 15, Amount = 1 },

            // Promotional Announcement
            new RobotTypeOfActivity { ActivityTypeId = 37, RoboTypeId = 16, Amount = 1 },

            // Sampling Distribution
            new RobotTypeOfActivity { ActivityTypeId = 38, RoboTypeId = 16, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 38, RoboTypeId = 14, Amount = 1 },

            // Brand Activation Speech
            new RobotTypeOfActivity { ActivityTypeId = 39, RoboTypeId = 14, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 39, RoboTypeId = 16, Amount = 1 },

            // Customer Product Consultation
            new RobotTypeOfActivity { ActivityTypeId = 40, RoboTypeId = 15, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 40, RoboTypeId = 16, Amount = 1 },

            // Collect Customer Information
            new RobotTypeOfActivity { ActivityTypeId = 41, RoboTypeId = 16, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 41, RoboTypeId = 14, Amount = 1 },

            // Feature Demonstration
            new RobotTypeOfActivity { ActivityTypeId = 42, RoboTypeId = 15, Amount = 1 },
            new RobotTypeOfActivity { ActivityTypeId = 42, RoboTypeId = 14, Amount = 1 }
        );

        modelBuilder.Entity<ActivityTypeGroup>().HasData(
            // CATEGORY 1: PERFORMANCE
            new ActivityTypeGroup { Id = 1, ActivityTypeId = 1, IsDeleted = false }, // AT01 - Solo Singing
            new ActivityTypeGroup { Id = 2, ActivityTypeId = 2, IsDeleted = false }, // AT02 - Duet Singing
            new ActivityTypeGroup { Id = 3, ActivityTypeId = 3, IsDeleted = false }, // AT03 - Birthday Song Performance
            new ActivityTypeGroup
                { Id = 4, ActivityTypeId = 4, IsDeleted = false }, // AT04 - Instrument Simulation Performance
            new ActivityTypeGroup { Id = 5, ActivityTypeId = 5, IsDeleted = false }, // AT05 - Solo Dance
            new ActivityTypeGroup { Id = 6, ActivityTypeId = 6, IsDeleted = false }, // AT06 - Group Robot Dance
            new ActivityTypeGroup { Id = 7, ActivityTypeId = 7, IsDeleted = false }, // AT07 - Kids Dance Performance
            new ActivityTypeGroup { Id = 8, ActivityTypeId = 8, IsDeleted = false }, // AT08 - Drama Acting
            new ActivityTypeGroup { Id = 9, ActivityTypeId = 9, IsDeleted = false }, // AT09 - Comedy Performance
            new ActivityTypeGroup
                { Id = 10, ActivityTypeId = 10, IsDeleted = false }, // AT10 - Theme Performance (Holiday/Seasonal)

            // CATEGORY 2: PRESENTATION & HOSTING
            new ActivityTypeGroup { Id = 11, ActivityTypeId = 11, IsDeleted = false }, // AT11 - Main Event Hosting (MC)
            new ActivityTypeGroup { Id = 12, ActivityTypeId = 12, IsDeleted = false }, // AT12 - Supporting MC
            new ActivityTypeGroup { Id = 13, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
            new ActivityTypeGroup { Id = 14, ActivityTypeId = 14, IsDeleted = false }, // AT14 - Corporate Introduction
            new ActivityTypeGroup
                { Id = 15, ActivityTypeId = 15, IsDeleted = false }, // AT15 - Educational Presentation
            new ActivityTypeGroup { Id = 16, ActivityTypeId = 16, IsDeleted = false }, // AT16 - Ceremony Opening Speech
            new ActivityTypeGroup { Id = 17, ActivityTypeId = 17, IsDeleted = false }, // AT17 - Guest Introduction
            new ActivityTypeGroup { Id = 18, ActivityTypeId = 18, IsDeleted = false }, // AT18 - Event Flow Guidance

            // CATEGORY 3: STORYTELLING & INTERACTION
            new ActivityTypeGroup { Id = 19, ActivityTypeId = 19, IsDeleted = false }, // AT19 - Fairy Tale Storytelling
            new ActivityTypeGroup
                { Id = 20, ActivityTypeId = 20, IsDeleted = false }, // AT20 - Inspirational Storytelling
            new ActivityTypeGroup { Id = 21, ActivityTypeId = 21, IsDeleted = false }, // AT21 - Historical Storytelling
            new ActivityTypeGroup { Id = 22, ActivityTypeId = 22, IsDeleted = false }, // AT22 - Poetry Reading
            new ActivityTypeGroup { Id = 23, ActivityTypeId = 23, IsDeleted = false }, // AT23 - Q&A Interaction
            new ActivityTypeGroup
                { Id = 24, ActivityTypeId = 24, IsDeleted = false }, // AT24 - AI Scripted Conversation
            new ActivityTypeGroup { Id = 25, ActivityTypeId = 25, IsDeleted = false }, // AT25 - Audience Interview
            new ActivityTypeGroup { Id = 26, ActivityTypeId = 26, IsDeleted = false }, // AT26 - Trivia Quiz Hosting
            new ActivityTypeGroup
                { Id = 27, ActivityTypeId = 27, IsDeleted = false }, // AT27 - Mini Games with Audience

            // CATEGORY 4: ENTERTAINMENT SUPPORT
            new ActivityTypeGroup { Id = 28, ActivityTypeId = 28, IsDeleted = false }, // AT28 - Greeting Guests
            new ActivityTypeGroup { Id = 29, ActivityTypeId = 29, IsDeleted = false }, // AT29 - Selfie with Guests
            new ActivityTypeGroup { Id = 30, ActivityTypeId = 30, IsDeleted = false }, // AT30 - Guest Check-in Support
            new ActivityTypeGroup
                { Id = 31, ActivityTypeId = 31, IsDeleted = false }, // AT31 - Event Area Tour Guidance
            new ActivityTypeGroup { Id = 32, ActivityTypeId = 32, IsDeleted = false }, // AT32 - Crowd Engagement
            new ActivityTypeGroup { Id = 33, ActivityTypeId = 33, IsDeleted = false }, // AT33 - Short Dance Break
            new ActivityTypeGroup
                { Id = 34, ActivityTypeId = 34, IsDeleted = false }, // AT34 - Birthday Celebration Support
            new ActivityTypeGroup
                { Id = 35, ActivityTypeId = 35, IsDeleted = false }, // AT35 - Children Entertainment Interaction

            // CATEGORY 5: PROMOTION & MARKETING
            new ActivityTypeGroup { Id = 36, ActivityTypeId = 36, IsDeleted = false }, // AT36 - Product Review
            new ActivityTypeGroup
                { Id = 37, ActivityTypeId = 37, IsDeleted = false }, // AT37 - Promotional Announcement
            new ActivityTypeGroup { Id = 38, ActivityTypeId = 38, IsDeleted = false }, // AT38 - Sampling Distribution
            new ActivityTypeGroup { Id = 39, ActivityTypeId = 39, IsDeleted = false }, // AT39 - Brand Activation Speech
            new ActivityTypeGroup
                { Id = 40, ActivityTypeId = 40, IsDeleted = false }, // AT40 - Customer Product Consultation
            new ActivityTypeGroup
                { Id = 41, ActivityTypeId = 41, IsDeleted = false }, // AT41 - Collect Customer Information
            new ActivityTypeGroup { Id = 42, ActivityTypeId = 42, IsDeleted = false } // AT42 - Feature Demonstration
        );

        modelBuilder.Entity<ActivityTypeGroup>().HasData(
            // CATEGORY 1 — PERFORMANCE
            new ActivityTypeGroup { Id = 43, ActivityTypeId = 6, IsDeleted = false }, // AT06 - Group Robot Dance
            new ActivityTypeGroup { Id = 44, ActivityTypeId = 9, IsDeleted = false }, // AT09 - Comedy Performance
            new ActivityTypeGroup { Id = 45, ActivityTypeId = 9, IsDeleted = false }, // AT09 - Comedy Performance
            new ActivityTypeGroup { Id = 46, ActivityTypeId = 9, IsDeleted = false }, // AT09 - Comedy Performance

            // CATEGORY 2 — PRESENTATION & HOSTING
            new ActivityTypeGroup { Id = 47, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
            new ActivityTypeGroup { Id = 48, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
            new ActivityTypeGroup { Id = 49, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
            new ActivityTypeGroup { Id = 50, ActivityTypeId = 13, IsDeleted = false }, // AT13 - Product Presentation
            new ActivityTypeGroup
                { Id = 51, ActivityTypeId = 15, IsDeleted = false }, // AT15 - Educational Presentation
            new ActivityTypeGroup
                { Id = 52, ActivityTypeId = 15, IsDeleted = false }, // AT15 - Educational Presentation
            new ActivityTypeGroup { Id = 53, ActivityTypeId = 11, IsDeleted = false }, // AT11 - Main Event Hosting (MC)
            new ActivityTypeGroup { Id = 54, ActivityTypeId = 11, IsDeleted = false }, // AT11 - Main Event Hosting (MC)
            new ActivityTypeGroup { Id = 55, ActivityTypeId = 12, IsDeleted = false }, // AT12 - Supporting MC
            new ActivityTypeGroup { Id = 56, ActivityTypeId = 16, IsDeleted = false }, // AT16 - Ceremony Opening Speech
            new ActivityTypeGroup { Id = 57, ActivityTypeId = 16, IsDeleted = false }, // AT16 - Ceremony Opening Speech
            new ActivityTypeGroup { Id = 58, ActivityTypeId = 17, IsDeleted = false }, // AT17 - Guest Introduction

            // CATEGORY 3 — STORYTELLING & INTERACTION
            new ActivityTypeGroup { Id = 59, ActivityTypeId = 19, IsDeleted = false }, // AT19 - Fairy Tale Storytelling
            new ActivityTypeGroup
                { Id = 60, ActivityTypeId = 20, IsDeleted = false }, // AT20 - Inspirational Storytelling
            new ActivityTypeGroup { Id = 61, ActivityTypeId = 21, IsDeleted = false }, // AT21 - Historical Storytelling
            new ActivityTypeGroup { Id = 62, ActivityTypeId = 22, IsDeleted = false }, // AT22 - Poetry Reading
            new ActivityTypeGroup { Id = 63, ActivityTypeId = 26, IsDeleted = false }, // AT26 - Trivia Quiz Hosting
            new ActivityTypeGroup { Id = 64, ActivityTypeId = 26, IsDeleted = false }, // AT26 - Trivia Quiz Hosting
            new ActivityTypeGroup
                { Id = 65, ActivityTypeId = 27, IsDeleted = false }, // AT27 - Mini Games with Audience
            new ActivityTypeGroup
                { Id = 66, ActivityTypeId = 27, IsDeleted = false }, // AT27 - Mini Games with Audience
            new ActivityTypeGroup { Id = 67, ActivityTypeId = 25, IsDeleted = false }, // AT25 - Audience Interview

            // CATEGORY 4 — ENTERTAINMENT SUPPORT
            new ActivityTypeGroup
                { Id = 68, ActivityTypeId = 34, IsDeleted = false }, // AT34 - Birthday Celebration Support
            new ActivityTypeGroup
                { Id = 69, ActivityTypeId = 34, IsDeleted = false }, // AT34 - Birthday Celebration Support
            new ActivityTypeGroup { Id = 70, ActivityTypeId = 32, IsDeleted = false }, // AT32 - Crowd Engagement
            new ActivityTypeGroup { Id = 71, ActivityTypeId = 33, IsDeleted = false }, // AT33 - Short Dance Break
            new ActivityTypeGroup
                { Id = 72, ActivityTypeId = 35, IsDeleted = false }, // AT35 - Children Entertainment Interaction
            new ActivityTypeGroup
                { Id = 73, ActivityTypeId = 35, IsDeleted = false }, // AT35 - Children Entertainment Interaction
            new ActivityTypeGroup
                { Id = 74, ActivityTypeId = 35, IsDeleted = false }, // AT35 - Children Entertainment Interaction

            // CATEGORY 5 — PROMOTION & MARKETING
            new ActivityTypeGroup { Id = 75, ActivityTypeId = 39, IsDeleted = false }, // AT39 - Brand Activation Speech
            new ActivityTypeGroup { Id = 76, ActivityTypeId = 39, IsDeleted = false }, // AT39 - Brand Activation Speech
            new ActivityTypeGroup
                { Id = 77, ActivityTypeId = 41, IsDeleted = false }, // AT41 - Collect Customer Information
            new ActivityTypeGroup
                { Id = 78, ActivityTypeId = 40, IsDeleted = false }, // AT40 - Customer Product Consultation
            new ActivityTypeGroup { Id = 79, ActivityTypeId = 38, IsDeleted = false }, // AT38 - Sampling Distribution
            new ActivityTypeGroup { Id = 80, ActivityTypeId = 42, IsDeleted = false }, // AT42 - Feature Demonstration
            new ActivityTypeGroup { Id = 81, ActivityTypeId = 42, IsDeleted = false }, // AT42 - Feature Demonstration
            new ActivityTypeGroup { Id = 82, ActivityTypeId = 36, IsDeleted = false }, // AT36 - Product Review
            new ActivityTypeGroup { Id = 83, ActivityTypeId = 36, IsDeleted = false }, // AT36 - Product Review
            new ActivityTypeGroup { Id = 84, ActivityTypeId = 36, IsDeleted = false }, // AT36 - Product Review
            new ActivityTypeGroup { Id = 85, ActivityTypeId = 36, IsDeleted = false } // AT36 - Product Review
        );

        modelBuilder.Entity<RobotInGroup>().HasData(
            // ===== CATEGORY 1: PERFORMANCE =====
            // AT01 - Solo Singing (HPR x1)
            new RobotInGroup { ActivityTypeGroupId = 1, RobotId = 1, IsDeleted = false }, // HPR-01

            // AT02 - Duet Singing (HPR x1, DCR x1)
            new RobotInGroup { ActivityTypeGroupId = 2, RobotId = 2, IsDeleted = false }, // HPR-02
            new RobotInGroup { ActivityTypeGroupId = 2, RobotId = 11, IsDeleted = false }, // DCR-11

            // AT03 - Birthday Song Performance (HPR x1)
            new RobotInGroup { ActivityTypeGroupId = 3, RobotId = 3, IsDeleted = false }, // HPR-03

            // AT04 - Instrument Simulation Performance (HPR x1)
            new RobotInGroup { ActivityTypeGroupId = 4, RobotId = 4, IsDeleted = false }, // HPR-04

            // AT05 - Solo Dance (DCR x1)
            new RobotInGroup { ActivityTypeGroupId = 5, RobotId = 12, IsDeleted = false }, // DCR-12

            // AT06 - Group Robot Dance (DCR x3 + HPR x1)
            new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 13, IsDeleted = false }, // DCR-13
            new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 14, IsDeleted = false }, // DCR-14
            new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 15, IsDeleted = false }, // DCR-15
            new RobotInGroup { ActivityTypeGroupId = 6, RobotId = 5, IsDeleted = false }, // HPR-05

            // AT07 - Kids Dance Performance (DCR x2 + HPR x1)
            new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 16, IsDeleted = false }, // DCR-16
            new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 17, IsDeleted = false }, // DCR-17
            new RobotInGroup { ActivityTypeGroupId = 7, RobotId = 6, IsDeleted = false }, // HPR-06

            // AT08 - Drama Acting (ARR x2 + HPR x1)
            new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 21, IsDeleted = false }, // ARR-21
            new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 22, IsDeleted = false }, // ARR-22
            new RobotInGroup { ActivityTypeGroupId = 8, RobotId = 7, IsDeleted = false }, // HPR-07

            // AT09 - Comedy Performance (ARR x2)
            new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 23, IsDeleted = false }, // ARR-23
            new RobotInGroup { ActivityTypeGroupId = 9, RobotId = 24, IsDeleted = false }, // ARR-24

            // AT10 - Theme Performance (HPR x2)
            new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 8, IsDeleted = false }, // HPR-08
            new RobotInGroup { ActivityTypeGroupId = 10, RobotId = 9, IsDeleted = false }, // HPR-09

            // ===== CATEGORY 2: PRESENTATION & HOSTING =====
            // AT11 - Main Event Hosting (HMR x1)
            new RobotInGroup { ActivityTypeGroupId = 11, RobotId = 41, IsDeleted = false }, // HMR-41

            // AT12 - Supporting MC (HMR x1)
            new RobotInGroup { ActivityTypeGroupId = 12, RobotId = 42, IsDeleted = false }, // HMR-42

            // AT13 - Product Presentation (PRR x1 + TPR x1)
            new RobotInGroup { ActivityTypeGroupId = 13, RobotId = 51, IsDeleted = false }, // PRR-51
            new RobotInGroup { ActivityTypeGroupId = 13, RobotId = 61, IsDeleted = false }, // TPR-61

            // AT14 - Corporate Introduction (PRR x1)
            new RobotInGroup { ActivityTypeGroupId = 14, RobotId = 52, IsDeleted = false }, // PRR-52

            // AT15 - Educational Presentation (PRR x1 + TPR x1)
            new RobotInGroup { ActivityTypeGroupId = 15, RobotId = 59, IsDeleted = false }, // PRR-59
            new RobotInGroup { ActivityTypeGroupId = 15, RobotId = 62, IsDeleted = false }, // TPR-62

            // AT16 - Ceremony Opening Speech (HMR x1)
            new RobotInGroup { ActivityTypeGroupId = 16, RobotId = 43, IsDeleted = false }, // HMR-43

            // AT17 - Guest Introduction (HMR x1)
            new RobotInGroup { ActivityTypeGroupId = 17, RobotId = 44, IsDeleted = false }, // HMR-44

            // AT18 - Event Flow Guidance (HMR x1)
            new RobotInGroup { ActivityTypeGroupId = 18, RobotId = 45, IsDeleted = false }, // HMR-45
            // (Nếu đã seed "Only New" dùng HMR-45, hãy dịch HMR của "Only New" sang 46..50 để tránh trùng)

            // ===== CATEGORY 3: STORYTELLING & INTERACTION =====
            // AT19 - Fairy Tale Storytelling (STR x1)
            new RobotInGroup { ActivityTypeGroupId = 19, RobotId = 71, IsDeleted = false }, // STR-71

            // AT20 - Inspirational Storytelling (STR x1)
            new RobotInGroup { ActivityTypeGroupId = 20, RobotId = 72, IsDeleted = false }, // STR-72

            // AT21 - Historical Storytelling (STR x1)
            new RobotInGroup { ActivityTypeGroupId = 21, RobotId = 73, IsDeleted = false }, // STR-73

            // AT22 - Poetry Reading (STR x1)
            new RobotInGroup { ActivityTypeGroupId = 22, RobotId = 74, IsDeleted = false }, // STR-74

            // AT23 - Q&A Interaction (ISR x1 + STR x1)
            new RobotInGroup { ActivityTypeGroupId = 23, RobotId = 81, IsDeleted = false }, // ISR-81
            new RobotInGroup { ActivityTypeGroupId = 23, RobotId = 75, IsDeleted = false }, // STR-75

            // AT24 - AI Scripted Conversation (ISR x1 + STR x1)
            new RobotInGroup { ActivityTypeGroupId = 24, RobotId = 82, IsDeleted = false }, // ISR-82
            new RobotInGroup { ActivityTypeGroupId = 24, RobotId = 76, IsDeleted = false }, // STR-76

            // AT25 - Audience Interview (ISR x1)
            new RobotInGroup { ActivityTypeGroupId = 25, RobotId = 83, IsDeleted = false }, // ISR-83

            // AT26 - Trivia Quiz Hosting (GQR x1 + ISR x1)
            new RobotInGroup { ActivityTypeGroupId = 26, RobotId = 91, IsDeleted = false }, // GQR-91
            new RobotInGroup { ActivityTypeGroupId = 26, RobotId = 84, IsDeleted = false }, // ISR-84

            // AT27 - Mini Games with Audience (GQR x1 + ISR x1)
            new RobotInGroup { ActivityTypeGroupId = 27, RobotId = 92, IsDeleted = false }, // GQR-92
            new RobotInGroup { ActivityTypeGroupId = 27, RobotId = 85, IsDeleted = false }, // ISR-85

            // ===== CATEGORY 4: ENTERTAINMENT SUPPORT =====
            // AT28 - Greeting Guests (HSR x1)
            new RobotInGroup { ActivityTypeGroupId = 28, RobotId = 101, IsDeleted = false }, // HSR-101

            // AT29 - Selfie with Guests (SPB x1)
            new RobotInGroup { ActivityTypeGroupId = 29, RobotId = 111, IsDeleted = false }, // SPB-111

            // AT30 - Guest Check-in Support (HSR x1)
            new RobotInGroup { ActivityTypeGroupId = 30, RobotId = 102, IsDeleted = false }, // HSR-102

            // AT31 - Event Area Tour Guidance (HSR x1)
            new RobotInGroup { ActivityTypeGroupId = 31, RobotId = 103, IsDeleted = false }, // HSR-103

            // AT32 - Crowd Engagement (HSR x1 + SPB x1)
            new RobotInGroup { ActivityTypeGroupId = 32, RobotId = 104, IsDeleted = false }, // HSR-104
            new RobotInGroup { ActivityTypeGroupId = 32, RobotId = 112, IsDeleted = false }, // SPB-112

            // AT33 - Short Dance Break (ANR x1 + HSR x1)
            new RobotInGroup { ActivityTypeGroupId = 33, RobotId = 121, IsDeleted = false }, // ANR-121
            new RobotInGroup { ActivityTypeGroupId = 33, RobotId = 105, IsDeleted = false }, // HSR-105

            // AT34 - Birthday Celebration Support (HSR x1 + SPB x1 + ANR x1)
            new RobotInGroup { ActivityTypeGroupId = 34, RobotId = 106, IsDeleted = false }, // HSR-106
            new RobotInGroup { ActivityTypeGroupId = 34, RobotId = 113, IsDeleted = false }, // SPB-113
            new RobotInGroup { ActivityTypeGroupId = 34, RobotId = 122, IsDeleted = false }, // ANR-122

            // AT35 - Children Entertainment Interaction (ANR x1 + SPB x1)
            new RobotInGroup { ActivityTypeGroupId = 35, RobotId = 123, IsDeleted = false }, // ANR-123
            new RobotInGroup { ActivityTypeGroupId = 35, RobotId = 114, IsDeleted = false }, // SPB-114

            // ===== CATEGORY 5: PROMOTION & MARKETING =====
            // AT36 - Product Review (PDR x1)
            new RobotInGroup { ActivityTypeGroupId = 36, RobotId = 141, IsDeleted = false }, // PDR-141

            // AT37 - Promotional Announcement (PMR x1)
            new RobotInGroup { ActivityTypeGroupId = 37, RobotId = 151, IsDeleted = false }, // PMR-151

            // AT38 - Sampling Distribution (PMR x1 + BAR x1)
            new RobotInGroup { ActivityTypeGroupId = 38, RobotId = 152, IsDeleted = false }, // PMR-152
            new RobotInGroup { ActivityTypeGroupId = 38, RobotId = 131, IsDeleted = false }, // BAR-131

            // AT39 - Brand Activation Speech (BAR x1 + PMR x1)
            new RobotInGroup { ActivityTypeGroupId = 39, RobotId = 132, IsDeleted = false }, // BAR-132
            new RobotInGroup { ActivityTypeGroupId = 39, RobotId = 153, IsDeleted = false }, // PMR-153

            // AT40 - Customer Product Consultation (PDR x1 + PMR x1)
            new RobotInGroup { ActivityTypeGroupId = 40, RobotId = 142, IsDeleted = false }, // PDR-142
            new RobotInGroup { ActivityTypeGroupId = 40, RobotId = 154, IsDeleted = false }, // PMR-154

            // AT41 - Collect Customer Information (PMR x1 + BAR x1)
            new RobotInGroup { ActivityTypeGroupId = 41, RobotId = 155, IsDeleted = false }, // PMR-155
            new RobotInGroup { ActivityTypeGroupId = 41, RobotId = 133, IsDeleted = false }, // BAR-133

            // AT42 - Feature Demonstration (PDR x1 + BAR x1)
            new RobotInGroup { ActivityTypeGroupId = 42, RobotId = 143, IsDeleted = false }, // PDR-143
            new RobotInGroup { ActivityTypeGroupId = 42, RobotId = 134, IsDeleted = false } // BAR-134
        );


        modelBuilder.Entity<RobotInGroup>().HasData(
            // ===== CATEGORY 1 =====
            // G43: AT06 - Group Robot Dance (DCR x3 + HPR x1)
            new RobotInGroup { ActivityTypeGroupId = 43, RobotId = 10, IsDeleted = false }, // HPR-10
            new RobotInGroup { ActivityTypeGroupId = 43, RobotId = 18, IsDeleted = false }, // DCR-18
            new RobotInGroup { ActivityTypeGroupId = 43, RobotId = 19, IsDeleted = false }, // DCR-19
            new RobotInGroup { ActivityTypeGroupId = 43, RobotId = 20, IsDeleted = false }, // DCR-20

            // G44–46: AT09 - Comedy Performance (ARR x2 mỗi nhóm)
            new RobotInGroup { ActivityTypeGroupId = 44, RobotId = 25, IsDeleted = false }, // ARR-25
            new RobotInGroup { ActivityTypeGroupId = 44, RobotId = 26, IsDeleted = false }, // ARR-26
            new RobotInGroup { ActivityTypeGroupId = 45, RobotId = 27, IsDeleted = false }, // ARR-27
            new RobotInGroup { ActivityTypeGroupId = 45, RobotId = 28, IsDeleted = false }, // ARR-28
            new RobotInGroup { ActivityTypeGroupId = 46, RobotId = 29, IsDeleted = false }, // ARR-29
            new RobotInGroup { ActivityTypeGroupId = 46, RobotId = 30, IsDeleted = false }, // ARR-30

            // ===== CATEGORY 2 =====
            // G47–50: AT13 - Product Presentation (PRR x1 + TPR x1)
            new RobotInGroup { ActivityTypeGroupId = 47, RobotId = 53, IsDeleted = false }, // PRR-53
            new RobotInGroup { ActivityTypeGroupId = 47, RobotId = 63, IsDeleted = false }, // TPR-63
            new RobotInGroup { ActivityTypeGroupId = 48, RobotId = 54, IsDeleted = false }, // PRR-54
            new RobotInGroup { ActivityTypeGroupId = 48, RobotId = 64, IsDeleted = false }, // TPR-64
            new RobotInGroup { ActivityTypeGroupId = 49, RobotId = 55, IsDeleted = false }, // PRR-55
            new RobotInGroup { ActivityTypeGroupId = 49, RobotId = 65, IsDeleted = false }, // TPR-65
            new RobotInGroup { ActivityTypeGroupId = 50, RobotId = 56, IsDeleted = false }, // PRR-56
            new RobotInGroup { ActivityTypeGroupId = 50, RobotId = 66, IsDeleted = false }, // TPR-66

            // G51–52: AT15 - Educational Presentation (PRR x1 + TPR x1)
            new RobotInGroup { ActivityTypeGroupId = 51, RobotId = 57, IsDeleted = false }, // PRR-57
            new RobotInGroup { ActivityTypeGroupId = 51, RobotId = 67, IsDeleted = false }, // TPR-67
            new RobotInGroup { ActivityTypeGroupId = 52, RobotId = 58, IsDeleted = false }, // PRR-58
            new RobotInGroup { ActivityTypeGroupId = 52, RobotId = 68, IsDeleted = false }, // TPR-68

            // G53–58: HMR-only (AT11 x2, AT12 x1, AT16 x2, AT17 x1)
            new RobotInGroup { ActivityTypeGroupId = 53, RobotId = 45, IsDeleted = false }, // HMR-45
            new RobotInGroup { ActivityTypeGroupId = 54, RobotId = 46, IsDeleted = false }, // HMR-46
            new RobotInGroup { ActivityTypeGroupId = 55, RobotId = 47, IsDeleted = false }, // HMR-47
            new RobotInGroup { ActivityTypeGroupId = 56, RobotId = 48, IsDeleted = false }, // HMR-48
            new RobotInGroup { ActivityTypeGroupId = 57, RobotId = 49, IsDeleted = false }, // HMR-49
            new RobotInGroup { ActivityTypeGroupId = 58, RobotId = 50, IsDeleted = false }, // HMR-50

            // ===== CATEGORY 3 =====
            // G59–62: STR-only (AT19..AT22)
            new RobotInGroup { ActivityTypeGroupId = 59, RobotId = 77, IsDeleted = false }, // STR-77
            new RobotInGroup { ActivityTypeGroupId = 60, RobotId = 78, IsDeleted = false }, // STR-78
            new RobotInGroup { ActivityTypeGroupId = 61, RobotId = 79, IsDeleted = false }, // STR-79
            new RobotInGroup { ActivityTypeGroupId = 62, RobotId = 80, IsDeleted = false }, // STR-80

            // G63–66: AT26/AT27 (GQR x1 + ISR x1 mỗi nhóm)
            new RobotInGroup { ActivityTypeGroupId = 63, RobotId = 93, IsDeleted = false }, // GQR-93
            new RobotInGroup { ActivityTypeGroupId = 63, RobotId = 86, IsDeleted = false }, // ISR-86
            new RobotInGroup { ActivityTypeGroupId = 64, RobotId = 94, IsDeleted = false }, // GQR-94
            new RobotInGroup { ActivityTypeGroupId = 64, RobotId = 87, IsDeleted = false }, // ISR-87
            new RobotInGroup { ActivityTypeGroupId = 65, RobotId = 95, IsDeleted = false }, // GQR-95
            new RobotInGroup { ActivityTypeGroupId = 65, RobotId = 88, IsDeleted = false }, // ISR-88
            new RobotInGroup { ActivityTypeGroupId = 66, RobotId = 96, IsDeleted = false }, // GQR-96
            new RobotInGroup { ActivityTypeGroupId = 66, RobotId = 89, IsDeleted = false }, // ISR-89

            // G67: AT25 - Audience Interview (ISR x1)
            new RobotInGroup { ActivityTypeGroupId = 67, RobotId = 90, IsDeleted = false }, // ISR-90

            // ===== CATEGORY 4 =====
            // G68–69: AT34 (HSR x1 + SPB x1 + ANR x1)
            new RobotInGroup { ActivityTypeGroupId = 68, RobotId = 107, IsDeleted = false }, // HSR-107
            new RobotInGroup { ActivityTypeGroupId = 68, RobotId = 115, IsDeleted = false }, // SPB-115
            new RobotInGroup { ActivityTypeGroupId = 68, RobotId = 124, IsDeleted = false }, // ANR-124
            new RobotInGroup { ActivityTypeGroupId = 69, RobotId = 108, IsDeleted = false }, // HSR-108
            new RobotInGroup { ActivityTypeGroupId = 69, RobotId = 116, IsDeleted = false }, // SPB-116
            new RobotInGroup { ActivityTypeGroupId = 69, RobotId = 125, IsDeleted = false }, // ANR-125

            // G70: AT32 (HSR x1 + SPB x1)
            new RobotInGroup { ActivityTypeGroupId = 70, RobotId = 109, IsDeleted = false }, // HSR-109
            new RobotInGroup { ActivityTypeGroupId = 70, RobotId = 117, IsDeleted = false }, // SPB-117

            // G71: AT33 (ANR x1 + HSR x1)
            new RobotInGroup { ActivityTypeGroupId = 71, RobotId = 126, IsDeleted = false }, // ANR-126
            new RobotInGroup { ActivityTypeGroupId = 71, RobotId = 110, IsDeleted = false }, // HSR-110

            // G72–74: AT35 (ANR x1 + SPB x1 mỗi nhóm)
            new RobotInGroup { ActivityTypeGroupId = 72, RobotId = 127, IsDeleted = false }, // ANR-127
            new RobotInGroup { ActivityTypeGroupId = 72, RobotId = 118, IsDeleted = false }, // SPB-118
            new RobotInGroup { ActivityTypeGroupId = 73, RobotId = 128, IsDeleted = false }, // ANR-128
            new RobotInGroup { ActivityTypeGroupId = 73, RobotId = 119, IsDeleted = false }, // SPB-119
            new RobotInGroup { ActivityTypeGroupId = 74, RobotId = 129, IsDeleted = false }, // ANR-129
            new RobotInGroup { ActivityTypeGroupId = 74, RobotId = 120, IsDeleted = false }, // SPB-120

            // ===== CATEGORY 5 =====
            // G75–76: AT39 (BAR x1 + PMR x1)
            new RobotInGroup { ActivityTypeGroupId = 75, RobotId = 135, IsDeleted = false }, // BAR-135
            new RobotInGroup { ActivityTypeGroupId = 75, RobotId = 156, IsDeleted = false }, // PMR-156
            new RobotInGroup { ActivityTypeGroupId = 76, RobotId = 136, IsDeleted = false }, // BAR-136
            new RobotInGroup { ActivityTypeGroupId = 76, RobotId = 157, IsDeleted = false }, // PMR-157

            // G77: AT41 (PMR x1 + BAR x1)
            new RobotInGroup { ActivityTypeGroupId = 77, RobotId = 158, IsDeleted = false }, // PMR-158
            new RobotInGroup { ActivityTypeGroupId = 77, RobotId = 137, IsDeleted = false }, // BAR-137

            // G78: AT40 (PDR x1 + PMR x1)
            new RobotInGroup { ActivityTypeGroupId = 78, RobotId = 144, IsDeleted = false }, // PDR-144
            new RobotInGroup { ActivityTypeGroupId = 78, RobotId = 159, IsDeleted = false }, // PMR-159

            // G79: AT38 (PMR x1 + BAR x1)
            new RobotInGroup { ActivityTypeGroupId = 79, RobotId = 160, IsDeleted = false }, // PMR-160
            new RobotInGroup { ActivityTypeGroupId = 79, RobotId = 138, IsDeleted = false }, // BAR-138

            // G80–81: AT42 (PDR x1 + BAR x1)
            new RobotInGroup { ActivityTypeGroupId = 80, RobotId = 145, IsDeleted = false }, // PDR-145
            new RobotInGroup { ActivityTypeGroupId = 80, RobotId = 139, IsDeleted = false }, // BAR-139
            new RobotInGroup { ActivityTypeGroupId = 81, RobotId = 146, IsDeleted = false }, // PDR-146
            new RobotInGroup { ActivityTypeGroupId = 81, RobotId = 140, IsDeleted = false }, // BAR-140

            // G82–85: AT36 (PDR x1 mỗi nhóm)
            new RobotInGroup { ActivityTypeGroupId = 82, RobotId = 147, IsDeleted = false }, // PDR-147
            new RobotInGroup { ActivityTypeGroupId = 83, RobotId = 148, IsDeleted = false }, // PDR-148
            new RobotInGroup { ActivityTypeGroupId = 84, RobotId = 149, IsDeleted = false }, // PDR-149
            new RobotInGroup { ActivityTypeGroupId = 85, RobotId = 150, IsDeleted = false } // PDR-150
        );
        
        modelBuilder.Entity<TypesOfRobo>().HasKey(tor => new { tor.RobotId, tor.RoboTypeId });

        modelBuilder.Entity<RobotInGroup>().HasKey(rig => new { rig.ActivityTypeGroupId, rig.RobotId });

        modelBuilder.Entity<RobotTypeOfActivity>().HasKey(rtoe => new { rtoe.ActivityTypeId, rtoe.RoboTypeId });

        modelBuilder.Entity<ActivityType>()
            .HasOne(a => a.EventActivity)
            .WithMany(e => e.ActivityTypes)
            .HasForeignKey(a => a.EventActivityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ActivityTypeGroup>().Navigation(a => a.ActivityType).AutoInclude();
        modelBuilder.Entity<ActivityType>().Navigation(a => a.EventActivity).AutoInclude();
        
        base.OnModelCreating(modelBuilder);
    }
}