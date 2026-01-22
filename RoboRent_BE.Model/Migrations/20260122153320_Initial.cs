using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoboRent_BE.Model.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventActivity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoboTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoboTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    gender = table.Column<string>(type: "text", nullable: true),
                    IdentificationIsValidated = table.Column<bool>(type: "boolean", nullable: true),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    IncludesOperator = table.Column<bool>(type: "boolean", nullable: false),
                    OperatorCount = table.Column<int>(type: "integer", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumMinutes = table.Column<int>(type: "integer", nullable: false),
                    BillingIncrementMinutes = table.Column<int>(type: "integer", nullable: false),
                    TechnicalStaffFeePerHour = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DamageDeposit = table.Column<decimal>(type: "numeric", nullable: false),
                    EventActivityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityTypes_EventActivity_EventActivityId",
                        column: x => x.EventActivityId,
                        principalTable: "EventActivity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChecklistDeliveryItemTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoboTypeId = table.Column<int>(type: "integer", nullable: true),
                    Code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Group = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsCritical = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresMeasuredValue = table.Column<bool>(type: "boolean", nullable: false),
                    MeasuredValueLabel = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    EvidenceRequirement = table.Column<int>(type: "integer", nullable: false),
                    FailRequiresNote = table.Column<bool>(type: "boolean", nullable: false),
                    FailRequiresEvidence = table.Column<bool>(type: "boolean", nullable: false),
                    RobotTypeId = table.Column<int>(type: "integer", nullable: true),
                    ActivityTypeId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistDeliveryItemTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistDeliveryItemTemplates_RoboTypes_RoboTypeId",
                        column: x => x.RoboTypeId,
                        principalTable: "RoboTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RobotAbilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RobotTypeId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    AbilityGroup = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LockAtCutoff = table.Column<bool>(type: "boolean", nullable: false),
                    IsPriceImpacting = table.Column<bool>(type: "boolean", nullable: false),
                    IsOnSiteAdjustable = table.Column<bool>(type: "boolean", nullable: false),
                    UiControl = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Placeholder = table.Column<string>(type: "text", nullable: true),
                    Min = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Max = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxLength = table.Column<int>(type: "integer", nullable: true),
                    Regex = table.Column<string>(type: "text", nullable: true),
                    OptionsJson = table.Column<string>(type: "jsonb", nullable: true),
                    JsonSchema = table.Column<string>(type: "jsonb", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RobotAbilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RobotAbilities_RoboTypes_RobotTypeId",
                        column: x => x.RobotTypeId,
                        principalTable: "RoboTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Robots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoboTypeId = table.Column<int>(type: "integer", nullable: true),
                    RobotName = table.Column<string>(type: "text", nullable: true),
                    ModelName = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Specification = table.Column<string>(type: "text", nullable: true),
                    RobotStatus = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Robots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Robots_RoboTypes_RoboTypeId",
                        column: x => x.RoboTypeId,
                        principalTable: "RoboTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TemplateCode = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BodyJson = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractTemplates_Accounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractTemplates_Accounts_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FaceProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: true),
                    CitizenId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Embedding = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HashSha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FrontIdImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaceProfiles_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActivityTypeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivityTypeId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTypeGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityTypeGroups_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventName = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    IsUpdated = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    EventActivityId = table.Column<int>(type: "integer", nullable: true),
                    ActivityTypeId = table.Column<int>(type: "integer", nullable: true),
                    StaffId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentals_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rentals_Accounts_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rentals_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rentals_EventActivity_EventActivityId",
                        column: x => x.EventActivityId,
                        principalTable: "EventActivity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RobotTypeOfEvents",
                columns: table => new
                {
                    ActivityTypeId = table.Column<int>(type: "integer", nullable: false),
                    RoboTypeId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RobotTypeOfEvents", x => new { x.ActivityTypeId, x.RoboTypeId });
                    table.ForeignKey(
                        name: "FK_RobotTypeOfEvents_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RobotTypeOfEvents_RoboTypes_RoboTypeId",
                        column: x => x.RoboTypeId,
                        principalTable: "RoboTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TypesOfRobos",
                columns: table => new
                {
                    RobotId = table.Column<int>(type: "integer", nullable: false),
                    RoboTypeId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesOfRobos", x => new { x.RobotId, x.RoboTypeId });
                    table.ForeignKey(
                        name: "FK_TypesOfRobos_RoboTypes_RoboTypeId",
                        column: x => x.RoboTypeId,
                        principalTable: "RoboTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TypesOfRobos_Robots_RobotId",
                        column: x => x.RobotId,
                        principalTable: "Robots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateClauses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClauseCode = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    IsMandatory = table.Column<bool>(type: "boolean", nullable: true),
                    IsEditable = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ContractTemplatesId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateClauses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateClauses_ContractTemplates_ContractTemplatesId",
                        column: x => x.ContractTemplatesId,
                        principalTable: "ContractTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RobotInGroups",
                columns: table => new
                {
                    ActivityTypeGroupId = table.Column<int>(type: "integer", nullable: false),
                    RobotId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RobotInGroups", x => new { x.ActivityTypeGroupId, x.RobotId });
                    table.ForeignKey(
                        name: "FK_RobotInGroups_ActivityTypeGroups_ActivityTypeGroupId",
                        column: x => x.ActivityTypeGroupId,
                        principalTable: "ActivityTypeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RobotInGroups_Robots_RobotId",
                        column: x => x.RobotId,
                        principalTable: "Robots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalId = table.Column<int>(type: "integer", nullable: false),
                    StaffId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Accounts_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Accounts_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractDrafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    BodyJson = table.Column<string>(type: "text", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OriginalBodyJson = table.Column<string>(type: "text", nullable: true),
                    ContractTemplatesId = table.Column<int>(type: "integer", nullable: true),
                    RentalId = table.Column<int>(type: "integer", nullable: true),
                    StaffId = table.Column<int>(type: "integer", nullable: true),
                    ManagerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractDrafts_Accounts_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractDrafts_Accounts_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractDrafts_ContractTemplates_ContractTemplatesId",
                        column: x => x.ContractTemplatesId,
                        principalTable: "ContractTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractDrafts_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FaceVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: true),
                    FaceProfileId = table.Column<int>(type: "integer", nullable: true),
                    MatchScore = table.Column<decimal>(type: "numeric", nullable: true),
                    Threshold = table.Column<decimal>(type: "numeric", nullable: false),
                    Result = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RentalId = table.Column<int>(type: "integer", nullable: true),
                    ImageEvidenceRef = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaceVerifications_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FaceVerifications_FaceProfiles_FaceProfileId",
                        column: x => x.FaceProfileId,
                        principalTable: "FaceProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FaceVerifications_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GroupSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EventLocation = table.Column<string>(type: "text", nullable: true),
                    EventCity = table.Column<string>(type: "text", nullable: true),
                    SetupTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    FinishTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ActivityTypeGroupId = table.Column<int>(type: "integer", nullable: true),
                    RentalId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupSchedules_ActivityTypeGroups_ActivityTypeGroupId",
                        column: x => x.ActivityTypeGroupId,
                        principalTable: "ActivityTypeGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupSchedules_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipientId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    RentalId = table.Column<int>(type: "integer", nullable: true),
                    RelatedEntityId = table.Column<int>(type: "integer", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsRealTime = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Accounts_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PriceQuotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalId = table.Column<int>(type: "integer", nullable: false),
                    RentalFee = table.Column<decimal>(type: "numeric", nullable: false),
                    StaffFee = table.Column<decimal>(type: "numeric", nullable: false),
                    DamageDeposit = table.Column<decimal>(type: "numeric", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "numeric", nullable: true),
                    DeliveryDistance = table.Column<int>(type: "integer", nullable: true),
                    CustomizationFee = table.Column<decimal>(type: "numeric", nullable: false),
                    StaffDescription = table.Column<string>(type: "text", nullable: true),
                    ManagerFeedback = table.Column<string>(type: "text", nullable: true),
                    CustomerReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    ManagerId = table.Column<int>(type: "integer", nullable: true),
                    SubmittedToManagerAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ManagerApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceQuotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceQuotes_Accounts_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PriceQuotes_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalChangeLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalId = table.Column<int>(type: "integer", nullable: false),
                    FieldName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    ChangedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedByAccountId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalChangeLogs_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    RentalId = table.Column<int>(type: "integer", nullable: true),
                    ContractUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalContracts_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RentalDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    RentalId = table.Column<int>(type: "integer", nullable: false),
                    RoboTypeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isLocked = table.Column<bool>(type: "boolean", nullable: true),
                    IsUpdated = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalDetails_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalDetails_RoboTypes_RoboTypeId",
                        column: x => x.RoboTypeId,
                        principalTable: "RoboTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatRoomId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    MessageType = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    MediaUrls = table.Column<string>(type: "text", nullable: true),
                    PriceQuoteId = table.Column<int>(type: "integer", nullable: true),
                    ContractId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Accounts_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractNumber = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    SignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContractUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContractDraftsId = table.Column<int>(type: "integer", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    ReviewerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerContracts_Accounts_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerContracts_Accounts_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerContracts_ContractDrafts_ContractDraftsId",
                        column: x => x.ContractDraftsId,
                        principalTable: "ContractDrafts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DraftApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContractDraftsId = table.Column<int>(type: "integer", nullable: true),
                    RequestedBy = table.Column<int>(type: "integer", nullable: true),
                    ReviewerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraftApprovals_Accounts_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DraftApprovals_Accounts_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DraftApprovals_ContractDrafts_ContractDraftsId",
                        column: x => x.ContractDraftsId,
                        principalTable: "ContractDrafts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DraftClauses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    IsModified = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContractDraftsId = table.Column<int>(type: "integer", nullable: true),
                    TemplateClausesId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftClauses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraftClauses_ContractDrafts_ContractDraftsId",
                        column: x => x.ContractDraftsId,
                        principalTable: "ContractDrafts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DraftClauses_TemplateClauses_TemplateClausesId",
                        column: x => x.TemplateClausesId,
                        principalTable: "TemplateClauses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActualDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupScheduleId = table.Column<int>(type: "integer", nullable: false),
                    StaffId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ActualDeliveryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualPickupTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActualDeliveries_Accounts_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActualDeliveries_GroupSchedules_GroupScheduleId",
                        column: x => x.GroupScheduleId,
                        principalTable: "GroupSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalId = table.Column<int>(type: "integer", nullable: true),
                    PriceQuoteId = table.Column<int>(type: "integer", nullable: true),
                    PaymentType = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    OrderCode = table.Column<long>(type: "bigint", nullable: false),
                    PaymentLinkId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CheckoutUrl = table.Column<string>(type: "text", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRecords_PriceQuotes_PriceQuoteId",
                        column: x => x.PriceQuoteId,
                        principalTable: "PriceQuotes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentRecords_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RobotAbilityValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalDetailId = table.Column<int>(type: "integer", nullable: false),
                    RobotAbilityId = table.Column<int>(type: "integer", nullable: false),
                    ValueText = table.Column<string>(type: "text", nullable: true),
                    ValueJson = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isUpdated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RobotAbilityValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RobotAbilityValues_RentalDetails_RentalDetailId",
                        column: x => x.RentalDetailId,
                        principalTable: "RentalDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RobotAbilityValues_RobotAbilities_RobotAbilityId",
                        column: x => x.RobotAbilityId,
                        principalTable: "RobotAbilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActualDeliveryId = table.Column<int>(type: "integer", nullable: false),
                    ChecklistNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CheckedByStaffId = table.Column<int>(type: "integer", nullable: true),
                    CheckedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomerAcceptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomerAcceptedById = table.Column<int>(type: "integer", nullable: true),
                    CustomerSignatureUrl = table.Column<string>(type: "text", nullable: true),
                    CustomerNote = table.Column<string>(type: "text", nullable: true),
                    OverallResult = table.Column<int>(type: "integer", nullable: true),
                    OverallNote = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TotalItems = table.Column<int>(type: "integer", nullable: true),
                    PassItems = table.Column<int>(type: "integer", nullable: true),
                    FailItems = table.Column<int>(type: "integer", nullable: true),
                    MetaJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistDeliveries_Accounts_CheckedByStaffId",
                        column: x => x.CheckedByStaffId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChecklistDeliveries_Accounts_CustomerAcceptedById",
                        column: x => x.CustomerAcceptedById,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChecklistDeliveries_ActualDeliveries_ActualDeliveryId",
                        column: x => x.ActualDeliveryId,
                        principalTable: "ActualDeliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DraftClausesId = table.Column<int>(type: "integer", nullable: true),
                    ReporterId = table.Column<int>(type: "integer", nullable: true),
                    ReportRole = table.Column<string>(type: "text", nullable: true),
                    AccusedId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EvidencePath = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Resolution = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedBy = table.Column<int>(type: "integer", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractReports_Accounts_AccusedId",
                        column: x => x.AccusedId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractReports_Accounts_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractReports_Accounts_ReviewedBy",
                        column: x => x.ReviewedBy,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractReports_DraftClauses_DraftClausesId",
                        column: x => x.DraftClausesId,
                        principalTable: "DraftClauses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractReports_PaymentRecords_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "PaymentRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChecklistDeliveryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChecklistDeliveryId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    EvidenceRequiredOnFail = table.Column<bool>(type: "boolean", nullable: false),
                    MustPassToDispatch = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Expected = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ValueType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ValueBool = table.Column<bool>(type: "boolean", nullable: true),
                    ValueNumber = table.Column<decimal>(type: "numeric", nullable: true),
                    ValueText = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ValueJson = table.Column<string>(type: "jsonb", nullable: true),
                    Result = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistDeliveryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistDeliveryItems_ChecklistDeliveries_ChecklistDeliver~",
                        column: x => x.ChecklistDeliveryId,
                        principalTable: "ChecklistDeliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistDeliveryEvidences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChecklistDeliveryId = table.Column<int>(type: "integer", nullable: true),
                    ChecklistDeliveryItemId = table.Column<int>(type: "integer", nullable: true),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    CapturedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UploadedByStaffId = table.Column<int>(type: "integer", nullable: false),
                    MetaJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistDeliveryEvidences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistDeliveryEvidences_Accounts_UploadedByStaffId",
                        column: x => x.UploadedByStaffId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistDeliveryEvidences_ChecklistDeliveries_ChecklistDel~",
                        column: x => x.ChecklistDeliveryId,
                        principalTable: "ChecklistDeliveries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChecklistDeliveryEvidences_ChecklistDeliveryItems_Checklist~",
                        column: x => x.ChecklistDeliveryItemId,
                        principalTable: "ChecklistDeliveryItems",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ActivityTypes",
                columns: new[] { "Id", "BillingIncrementMinutes", "Code", "Currency", "DamageDeposit", "Description", "EventActivityId", "HourlyRate", "IncludesOperator", "IsActive", "IsDeleted", "MinimumMinutes", "Name", "OperatorCount", "Price", "ShortDescription", "TechnicalStaffFeePerHour" },
                values: new object[,]
                {
                    { 1, 30, "BASIC", "VND", 5000000m, "Gói BASIC cho sự kiện nhỏ. Khách chọn giờ linh hoạt, hệ thống tính tiền theo đơn giá/giờ. Thời lượng tối thiểu 2 tiếng, làm tròn theo bước 30 phút. Bao gồm 1 kỹ thuật viên đi kèm.", null, 3500000m, true, true, false, 120, "Basic Event Package", 1, 7300000m, "Gói cơ bản với đầy đủ 4 nhóm robot cho sự kiện nhỏ", 150000m },
                    { 2, 30, "STANDARD", "VND", 10000000m, "Gói STANDARD cho sự kiện vừa. Khách chọn giờ linh hoạt, hệ thống tính tiền theo đơn giá/giờ. Thời lượng tối thiểu 2 tiếng, làm tròn theo bước 30 phút. Bao gồm 2 kỹ thuật viên hỗ trợ.", null, 5500000m, true, true, false, 120, "Standard Event Package", 2, 11800000m, "Gói tiêu chuẩn cho sự kiện vừa, tăng cường trình diễn và quảng bá", 200000m },
                    { 3, 30, "PREMIUM", "VND", 20000000m, "Gói PREMIUM cho sự kiện lớn. Khách chọn giờ linh hoạt, hệ thống tính tiền theo đơn giá/giờ. Thời lượng tối thiểu 2 tiếng, làm tròn theo bước 30 phút. Bao gồm 3 kỹ thuật viên cao cấp.", null, 8500000m, true, true, false, 120, "Premium Event Package", 3, 18800000m, "Gói cao cấp cho sự kiện lớn, trải nghiệm robot toàn diện", 300000m }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "Staff", "STAFF" },
                    { "3", null, "Customer", "CUSTOMER" },
                    { "4", null, "Manager", "MANAGER" },
                    { "5", null, "TechnicalStaff", "TECHNICALSTAFF" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Status", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "47ebcce9-fd0c-4173-91f4-a25385622d21", 0, "06625a65-14d1-4822-aaf1-2643d61d246b", "giangntse183662@fpt.edu.vn", true, true, null, "GIANGNTSE183662@FPT.EDU.VN", "GIANGNTSE183662@FPT.EDU.VN", null, null, false, "FRUURT5J22RQ24DHPTJOV27KBO6YSUUG", "Active", false, "giangntse183662@fpt.edu.vn" },
                    { "5373bf8f-51b2-4c2d-b832-be15aedd63bc", 0, "0b266393-f928-4c62-8d31-b7f4e8b884f4", "xuant0343@gmail.com", true, true, null, "XUANT0343@GMAIL.COM", "XUANT0343@GMAIL.COM", null, null, false, "TO6KVHYLEHG2KLGZPJVPI6NSP5FJBT5V", "Active", false, "xuant0343@gmail.com" },
                    { "fa56f53b-f406-42c4-afdc-f12a1a210b4b", 0, "e57cf66c-36b0-47b3-a7bc-26bb276298a4", "giangnguyen102004@gmail.com", true, true, null, "GIANGNGUYEN102004@GMAIL.COM", "GIANGNGUYEN102004@GMAIL.COM", null, null, false, "VPYTFD22TZD5DQECWLL62UFQVBZM6T4C", "Active", false, "giangnguyen102004@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "ChecklistDeliveryItemTemplates",
                columns: new[] { "Id", "ActivityTypeId", "Code", "CreatedAt", "Description", "EvidenceRequirement", "FailRequiresEvidence", "FailRequiresNote", "Group", "IsActive", "IsCritical", "MeasuredValueLabel", "RequiresMeasuredValue", "RoboTypeId", "RobotTypeId", "SortOrder", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, "DOCS_RENTAL_BRIEF", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra có đầy đủ thông tin người phụ trách kỹ thuật, hotline, và hướng dẫn sử dụng nhanh (nếu có).", 0, false, true, 9, true, true, null, false, null, null, 10, "Tài liệu & thông tin bàn giao", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 2, null, "SAFETY_SERIAL_QR_MATCH", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Quét QR/đối chiếu serial trên robot với mã trong hệ thống trước khi giao.", 1, true, true, 8, true, true, "Serial/QR", true, null, null, 10, "Serial/QR đúng với hệ thống", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 3, null, "APPEARANCE_OVERALL_PHOTO", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Chụp ảnh toàn thân robot (ít nhất 2 góc) để làm bằng chứng tình trạng ban đầu.", 1, true, false, 1, true, true, null, false, null, null, 10, "Ảnh tổng quan robot trước bàn giao", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 4, null, "APPEARANCE_SCRATCH_DENT", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Quan sát thân vỏ, khớp nối, mặt trước/sau; ghi nhận trầy xước hoặc móp/nứt nếu có.", 1, true, true, 1, true, true, null, false, null, null, 20, "Trầy xước/móp/nứt vỏ", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 5, null, "POWER_BATTERY_LEVEL", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Bật robot và kiểm tra % pin (khuyến nghị ≥ 70% trước khi vận chuyển/giao).", 1, true, true, 2, true, true, "Battery (%)", true, null, null, 10, "Mức pin trước khi giao", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 6, null, "POWER_CHARGER_CABLES", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Đảm bảo có đủ sạc/adapter và dây nguồn đúng chuẩn, không đứt gãy.", 1, true, true, 7, true, true, null, false, null, null, 20, "Sạc/adapter/dây nguồn đầy đủ", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 7, null, "MOBILITY_BASIC_TEST", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra robot có thể đứng vững/di chuyển cơ bản theo khả năng (không cần chạy show).", 2, true, true, 3, true, true, null, false, null, null, 10, "Test di chuyển cơ bản", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 8, null, "AUDIO_SPEAKER_TEST", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Phát đoạn âm thanh mẫu; kiểm tra rè, nhỏ bất thường hoặc mất kênh.", 0, false, true, 4, true, false, null, false, null, null, 10, "Test loa (âm thanh)", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 9, null, "DISPLAY_SCREEN_OK", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra không sọc, không loang màu, cảm ứng/điều khiển hiển thị hoạt động.", 1, true, true, 5, true, true, null, false, null, null, 10, "Màn hình hiển thị bình thường", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 10, null, "ACCESSORIES_REMOTE_CONTROLLER", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra có đủ remote/controller và pin remote (nếu dùng).", 1, true, true, 7, true, false, null, false, null, null, 30, "Remote/thiết bị điều khiển (nếu có)", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 11, null, "SAFETY_CASE_PACKING", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Robot được cố định an toàn trong case/thùng; có chèn chống sốc.", 1, true, true, 8, true, true, null, false, null, null, 20, "Đóng gói/thùng/case vận chuyển đúng chuẩn", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) }
                });

            migrationBuilder.InsertData(
                table: "RoboTypes",
                columns: new[] { "Id", "Description", "IsDeleted", "TypeName" },
                values: new object[,]
                {
                    { 1, "Robot lễ tân được sử dụng tại khu vực đón tiếp sự kiện, đóng vai trò như một “front-desk di động”. Robot có khả năng hiển thị đầy đủ nhận diện thương hiệu của khách hàng trên màn hình, bao gồm tên thương hiệu, logo, giao diện màu sắc, banner, hình nền và các nội dung chào mừng. Ngoài ra, robot còn có thể hiển thị QR hoặc CTA để điều hướng khách truy cập tới website, landing page hoặc biểu mẫu đăng ký, cũng như trình chiếu các nội dung tài trợ theo kịch bản.\n\nVề tương tác, robot hỗ trợ chào hỏi và giao tiếp theo kịch bản được cấu hình sẵn, có thể sử dụng nhiều ngôn ngữ khác nhau. Giọng nói, tốc độ đọc, cao độ và âm lượng đều có thể điều chỉnh để phù hợp với không gian sự kiện. Robot cũng có khả năng trả lời các câu hỏi thường gặp thông qua danh sách FAQ được chuẩn bị trước.\n\nTrong trường hợp cần thiết, robot có thể hỗ trợ check-in và thu thập thông tin khách tham dự thông qua QR code hoặc form nhập liệu, kèm theo thông báo quyền riêng tư. Ngoài ra, robot còn có thể hướng dẫn khách di chuyển đến các khu vực, booth hoặc điểm tham quan trong sự kiện, dựa trên danh sách điểm đến và các quy tắc điều hướng đã được cấu hình trước nhằm đảm bảo an toàn.", false, "Reception Robot" },
                    { 2, "Robot biểu diễn được thiết kế để trình diễn trên sân khấu theo kịch bản và chương trình cụ thể. Robot có thể chạy một hoặc nhiều show set được cấu hình sẵn, mỗi show bao gồm nhạc nền, biên đạo, thời lượng và số lần lặp. Thứ tự chạy các tiết mục được xác định rõ ràng nhằm đảm bảo mạch chương trình.\n\nRobot hỗ trợ nhiều hình thức kích hoạt biểu diễn, bao gồm kích hoạt thủ công, theo lịch trình hoặc thông qua tín hiệu điều khiển từ xa. Trong quá trình biểu diễn, các điểm cue hoặc timecode có thể được sử dụng để điều khiển hành động của robot theo đúng nhịp chương trình.\n\nĐể đảm bảo an toàn, robot yêu cầu cấu hình đầy đủ khu vực sân khấu, khoảng cách an toàn và các giới hạn kỹ thuật như tốc độ khớp hoặc góc chuyển động. Một số chương trình có thể yêu cầu rehearsal trước giờ diễn chính thức. Mức độ rủi ro của chương trình cũng được đánh giá để phục vụ công tác quản lý và vận hành.\n\nNgoài phần biểu diễn chính, robot có thể được cá nhân hóa về mặt hình ảnh thông qua trang phục hoặc hiệu ứng LED, đồng thời có thể phát các câu chào mở đầu và kết thúc nhằm tăng tính kết nối với khán giả.", false, "Performance Robot" },
                    { 3, "Robot Host đóng vai trò như một MC dẫn chương trình, hoạt động theo timeline sự kiện. Nội dung dẫn được chia thành các khối kịch bản, mỗi khối bao gồm tiêu đề, nội dung lời dẫn, ngôn ngữ sử dụng, thời lượng ước tính và các gợi ý tương tác với khán giả. Điều này giúp robot dẫn dắt chương trình một cách mạch lạc và linh hoạt.\n\nRobot hỗ trợ cấu hình chi tiết về giọng nói, bao gồm tốc độ, cao độ và âm lượng, đồng thời có thể sử dụng từ điển phát âm riêng để đọc chính xác tên riêng, thương hiệu hoặc thuật ngữ đặc thù. Âm lượng cũng có thể được điều chỉnh theo từng ngữ cảnh hoặc khung giờ yên tĩnh.\n\nTrong quá trình dẫn chương trình, robot có thể hiển thị thêm các nội dung hỗ trợ trên màn hình như hình ảnh, QR code, slide hoặc countdown cho các mốc thời gian quan trọng. Khi sự kiện có MC người tham gia, robot có thể phối hợp dẫn chương trình thông qua các cue chuyển giao rõ ràng giữa robot và MC, đảm bảo chương trình diễn ra trôi chảy.", false, "Host Robot" },
                    { 4, "Robot Promotion được sử dụng trong các hoạt động quảng bá, activation và booth marketing. Robot có khả năng chạy playlist quảng cáo bao gồm hình ảnh hoặc video theo thứ tự và thời lượng được xác định trước. Việc phát nội dung có thể tuân theo lịch trình cụ thể, khung giờ cao điểm hoặc chu kỳ lặp để tối ưu khả năng tiếp cận khách tham quan.\n\nRobot cũng có thể phát nhạc nền hoặc các đoạn thông báo mời chào, với âm lượng được điều chỉnh phù hợp với không gian booth. Các nội dung kêu gọi hành động như QR code, thông điệp CTA hoặc thông tin ưu đãi, voucher có thể được hiển thị nhằm thu hút và chuyển đổi khách hàng.\n\nVề di chuyển, robot có thể đứng cố định hoặc di chuyển tuần tra trong khu vực booth theo lộ trình được xác định trước. Các điểm dừng, khu vực cần tránh và tốc độ di chuyển tối đa đều được cấu hình nhằm đảm bảo an toàn cho khách tham dự và không gian trưng bày.", false, "Promotion Robot" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "DateOfBirth", "FullName", "IdentificationIsValidated", "PhoneNumber", "Status", "UserId", "gender", "isDeleted" },
                values: new object[,]
                {
                    { 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "xuan truong", false, "", "Active", "5373bf8f-51b2-4c2d-b832-be15aedd63bc", "", false },
                    { 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nguyen Truong Giang (K18 HCM)", false, "", "Active", "47ebcce9-fd0c-4173-91f4-a25385622d21", "", false },
                    { 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Trường Giang Nguyễn", false, "", "Active", "fa56f53b-f406-42c4-afdc-f12a1a210b4b", "", false }
                });

            migrationBuilder.InsertData(
                table: "ActivityTypeGroups",
                columns: new[] { "Id", "ActivityTypeId", "IsDeleted" },
                values: new object[,]
                {
                    { 1, 1, false },
                    { 2, 1, false },
                    { 3, 1, false },
                    { 4, 1, false },
                    { 5, 2, false },
                    { 6, 2, false },
                    { 7, 2, false },
                    { 8, 2, false },
                    { 9, 3, false },
                    { 10, 3, false },
                    { 11, 3, false },
                    { 12, 3, false }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId" },
                values: new object[,]
                {
                    { "Google", "101135697127238020611", "Google", "5373bf8f-51b2-4c2d-b832-be15aedd63bc" },
                    { "Google", "111410206604030881459", "Google", "47ebcce9-fd0c-4173-91f4-a25385622d21" },
                    { "Google", "116621369845429820359", "Google", "fa56f53b-f406-42c4-afdc-f12a1a210b4b" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "2", "47ebcce9-fd0c-4173-91f4-a25385622d21" },
                    { "3", "5373bf8f-51b2-4c2d-b832-be15aedd63bc" },
                    { "4", "fa56f53b-f406-42c4-afdc-f12a1a210b4b" }
                });

            migrationBuilder.InsertData(
                table: "ChecklistDeliveryItemTemplates",
                columns: new[] { "Id", "ActivityTypeId", "Code", "CreatedAt", "Description", "EvidenceRequirement", "FailRequiresEvidence", "FailRequiresNote", "Group", "IsActive", "IsCritical", "MeasuredValueLabel", "RequiresMeasuredValue", "RoboTypeId", "RobotTypeId", "SortOrder", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 101, null, "RECEPTION_BRANDING_ASSETS_READY", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra đã có logo/banner/hình nền và giao diện màu sắc đúng theo yêu cầu khách.", 1, true, true, 5, true, true, null, false, 1, null, 10, "Branding assets đã nạp sẵn", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 102, null, "RECEPTION_QR_CTA_WORKING", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Mở màn hình QR/CTA; thử quét QR bằng điện thoại để đảm bảo điều hướng đúng link.", 1, true, true, 5, true, true, null, false, 1, null, 20, "QR/CTA hiển thị đúng & quét được", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 103, null, "RECEPTION_FAQ_SCRIPT_LOADED", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra danh sách câu chào và FAQ theo yêu cầu sự kiện (ngôn ngữ, giọng đọc).", 0, false, true, 0, true, false, null, false, 1, null, 10, "Kịch bản chào hỏi/FAQ đã cấu hình", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 201, null, "PERFORMANCE_SHOWSET_LOADED", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra danh sách tiết mục (nhạc, thời lượng, thứ tự) đã tải đúng phiên bản.", 1, true, true, 0, true, true, null, false, 2, null, 10, "Show set/playlist biểu diễn đã nạp", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 202, null, "PERFORMANCE_TRIGGER_METHOD_READY", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Xác nhận phương thức kích hoạt (manual/remote/schedule) hoạt động đúng.", 2, true, true, 0, true, true, null, false, 2, null, 20, "Cơ chế kích hoạt biểu diễn sẵn sàng", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 203, null, "PERFORMANCE_SAFETY_LIMITS_SET", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Xác nhận đã set vùng an toàn/khoảng cách/giới hạn tốc độ phù hợp kịch bản.", 0, false, true, 8, true, true, null, false, 2, null, 10, "Thiết lập giới hạn an toàn sân khấu", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 301, null, "HOST_SCRIPT_BLOCKS_READY", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra các khối nội dung lời dẫn theo timeline; đúng ngôn ngữ và thời lượng ước tính.", 1, true, true, 0, true, true, null, false, 3, null, 10, "Kịch bản MC (blocks) đã nạp", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 302, null, "HOST_VOICE_SETTINGS_OK", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra tốc độ/cao độ/âm lượng; đảm bảo nghe rõ và không gây chói.", 0, false, true, 4, true, true, null, false, 3, null, 20, "Thiết lập giọng nói phù hợp không gian", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 303, null, "HOST_MIC_TEST_IF_ANY", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra mic thu âm/khử ồn theo cấu hình; ghi nhận nếu micro yếu hoặc nhiễu.", 0, false, true, 4, true, false, null, false, 3, null, 10, "Test micro (nếu robot dùng mic)", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 304, null, "HOST_COUNTDOWN_OR_SLIDE_READY", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra nội dung countdown/slide/QR hiển thị đúng theo mốc chương trình.", 1, true, true, 5, true, false, null, false, 3, null, 30, "Countdown/slide hỗ trợ hiển thị sẵn sàng", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 401, null, "PROMO_MEDIA_PLAYLIST_READY", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra thứ tự, thời lượng và khả năng play mượt của playlist nội dung booth.", 2, true, true, 5, true, true, null, false, 4, null, 10, "Playlist quảng cáo (image/video) đã nạp", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 402, null, "PROMO_CTA_QR_COUPON_READY", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Kiểm tra QR/CTA/voucher đúng nội dung ưu đãi và quét ra đúng link/landing page.", 1, true, true, 5, true, true, null, false, 4, null, 20, "QR/CTA/voucher hiển thị đúng", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) },
                    { 403, null, "PROMO_PATROL_ROUTE_CONFIG", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959), "Xác nhận lộ trình, điểm dừng, khu vực tránh và tốc độ tối đa phù hợp booth.", 0, false, true, 3, true, false, null, false, 4, null, 20, "Lộ trình tuần tra/điểm dừng (nếu di chuyển)", new DateTime(2026, 1, 22, 15, 33, 17, 584, DateTimeKind.Utc).AddTicks(7959) }
                });

            migrationBuilder.InsertData(
                table: "RobotAbilities",
                columns: new[] { "Id", "AbilityGroup", "DataType", "Description", "IsActive", "IsOnSiteAdjustable", "IsPriceImpacting", "IsRequired", "JsonSchema", "Key", "Label", "LockAtCutoff", "Max", "MaxLength", "Min", "OptionsJson", "Placeholder", "Regex", "RobotTypeId", "UiControl" },
                values: new object[,]
                {
                    { 1, "Branding & UI", "string", "Tên thương hiệu hiển thị trên màn hình robot.", true, false, false, true, null, "brandName", "Brand Name", true, null, 100, null, null, "VD: RoboRent", null, 1, "text" },
                    { 2, "Branding & UI", "string", "Đường dẫn logo (PNG/SVG) hiển thị trên robot.", true, false, false, true, null, "logoUrl", "Logo URL", true, null, 500, null, null, "https://...", null, 1, "url" },
                    { 3, "Branding & UI", "json", "Cấu hình giao diện (banner/background/color).", true, false, false, false, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"bannerUrl\":{\"type\":\"string\"},\r\n            \"backgroundUrl\":{\"type\":\"string\"},\r\n            \"primaryColor\":{\"type\":\"string\"},\r\n            \"secondaryColor\":{\"type\":\"string\"}\r\n          }\r\n        }", "themeAssets", "Theme Assets", true, null, null, null, null, null, null, 1, "jsonEditor" },
                    { 4, "Branding & UI", "string", "Text chào mừng hiển thị trên màn hình.", true, true, false, false, null, "welcomeScreenText", "Welcome Screen Text", true, null, 300, null, null, "Chào mừng bạn đến với...", null, 1, "textarea" },
                    { 5, "Branding & UI", "string", "Link/QR điều hướng khách (landing page, form...).", true, true, false, false, null, "ctaQrUrl", "CTA QR URL", true, null, 500, null, null, "https://...", null, 1, "url" },
                    { 6, "Branding & UI", "json", "Danh sách asset tài trợ hiển thị luân phiên.", true, false, false, false, "{ \"type\":\"array\", \"items\":{\"type\":\"string\"} }", "sponsorAssets", "Sponsor Assets", true, null, null, null, null, null, null, 1, "jsonEditor" },
                    { 7, "Greeting & Script", "string", "Kịch bản chào hỏi / giới thiệu ngắn.", true, false, false, false, null, "greetingScript", "Greeting Script", true, null, 2000, null, null, "Xin chào quý khách...", null, 1, "textarea" },
                    { 8, "Greeting & Script", "enum[]", "Ngôn ngữ sử dụng khi chào hỏi / hướng dẫn.", true, false, false, true, null, "languages", "Languages", true, null, null, null, "[\"VI\",\"EN\",\"JP\",\"KR\",\"CN\"]", null, null, 1, "multiSelect" },
                    { 9, "Greeting & Script", "json", "Cấu hình giọng nói (tốc độ/độ cao...).", true, true, false, false, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"voiceName\":{\"type\":\"string\"},\r\n            \"rate\":{\"type\":\"number\",\"minimum\":0.5,\"maximum\":2.0},\r\n            \"pitch\":{\"type\":\"number\",\"minimum\":-10,\"maximum\":10},\r\n            \"volume\":{\"type\":\"number\",\"minimum\":0,\"maximum\":100}\r\n          }\r\n        }", "voiceProfile", "Voice Profile", true, null, null, null, null, null, null, 1, "jsonEditor" },
                    { 10, "Greeting & Script", "json", "Danh sách câu hỏi thường gặp (Q/A + keywords).", true, false, false, false, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"question\":{\"type\":\"string\"},\r\n              \"answer\":{\"type\":\"string\"},\r\n              \"keywords\":{\"type\":\"array\",\"items\":{\"type\":\"string\"}}\r\n            },\r\n            \"required\":[\"question\",\"answer\"]\r\n          }\r\n        }", "faqItems", "FAQ Items", true, null, null, null, null, null, null, 1, "jsonEditor" },
                    { 11, "Check-in & Lead", "enum", "Chế độ check-in tại sự kiện.", true, false, false, true, null, "checkinMode", "Check-in Mode", true, null, null, null, "[\"None\",\"QR\",\"Form\"]", null, null, 1, "select" },
                    { 12, "Check-in & Lead", "enum[]", "Các trường thu thập thông tin khách.", true, false, false, false, null, "leadFormFields", "Lead Form Fields", true, null, null, null, "[\"Name\",\"Phone\",\"Email\",\"Company\",\"Title\"]", null, null, 1, "multiSelect" },
                    { 13, "Check-in & Lead", "string", "Thông báo quyền riêng tư khi thu thập dữ liệu.", true, false, false, false, null, "privacyNoticeText", "Privacy Notice Text", true, null, 2000, null, null, "Thông tin của bạn sẽ được sử dụng để...", null, 1, "textarea" },
                    { 14, "Navigation", "json", "Danh sách điểm đến/booth để robot hướng dẫn.", true, false, true, false, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"name\":{\"type\":\"string\"},\r\n              \"description\":{\"type\":\"string\"},\r\n              \"locationHint\":{\"type\":\"string\"}\r\n            },\r\n            \"required\":[\"name\"]\r\n          }\r\n        }", "pois", "Points of Interest (POI)", true, null, null, null, null, null, null, 1, "jsonEditor" },
                    { 15, "Navigation", "json", "Quy tắc điều hướng (tốc độ, vùng cấm...).", true, true, true, false, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"maxSpeed\":{\"type\":\"number\",\"minimum\":0.1,\"maximum\":2.0},\r\n            \"noGoZones\":{\"type\":\"array\",\"items\":{\"type\":\"string\"}},\r\n            \"preferredPaths\":{\"type\":\"array\",\"items\":{\"type\":\"string\"}}\r\n          }\r\n        }", "navigationRules", "Navigation Rules", true, null, null, null, null, null, null, 1, "jsonEditor" },
                    { 16, "Show Set", "json", "Danh sách show set (nhạc + choreography + thời lượng + lặp).", true, false, true, true, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"setName\":{\"type\":\"string\"},\r\n              \"musicTrackUrl\":{\"type\":\"string\"},\r\n              \"choreographyId\":{\"type\":\"string\"},\r\n              \"durationSec\":{\"type\":\"integer\",\"minimum\":10},\r\n              \"repeatCount\":{\"type\":\"integer\",\"minimum\":1}\r\n            },\r\n            \"required\":[\"setName\",\"durationSec\"]\r\n          }\r\n        }", "showSets", "Show Sets", true, null, null, null, null, null, null, 2, "jsonEditor" },
                    { 17, "Show Set", "json", "Thứ tự chạy các set (theo index).", true, false, true, false, "{ \"type\":\"array\", \"items\":{\"type\":\"integer\",\"minimum\":0} }", "showOrder", "Show Order", true, null, null, null, null, null, null, 2, "jsonEditor" },
                    { 18, "Cues & Triggers", "enum", "Cách kích hoạt biểu diễn.", true, false, false, true, null, "triggerMode", "Trigger Mode", true, null, null, null, "[\"Manual\",\"Scheduled\",\"RemoteSignal\"]", null, null, 2, "select" },
                    { 19, "Cues & Triggers", "json", "Các cue/timecode điều khiển hành động trong show.", true, false, true, false, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"timecodeSec\":{\"type\":\"integer\",\"minimum\":0},\r\n              \"action\":{\"type\":\"string\"},\r\n              \"note\":{\"type\":\"string\"}\r\n            },\r\n            \"required\":[\"timecodeSec\",\"action\"]\r\n          }\r\n        }", "cuePoints", "Cue Points", true, null, null, null, null, null, null, 2, "jsonEditor" },
                    { 20, "Stage & Safety", "json", "Khu vực sân khấu và khoảng cách an toàn.", true, false, false, true, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"widthM\":{\"type\":\"number\",\"minimum\":1},\r\n            \"depthM\":{\"type\":\"number\",\"minimum\":1},\r\n            \"safeDistanceM\":{\"type\":\"number\",\"minimum\":0.5}\r\n          },\r\n          \"required\":[\"widthM\",\"depthM\",\"safeDistanceM\"]\r\n        }", "stageZone", "Stage Zone", true, null, null, null, null, null, null, 2, "jsonEditor" },
                    { 21, "Stage & Safety", "json", "Giới hạn an toàn (tốc độ khớp, góc tay chân...).", true, true, false, false, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"maxJointSpeed\":{\"type\":\"number\",\"minimum\":0.1,\"maximum\":2.0},\r\n            \"maxLimbAngle\":{\"type\":\"number\",\"minimum\":10,\"maximum\":180},\r\n            \"emergencyStopRequired\":{\"type\":\"boolean\"}\r\n          }\r\n        }", "safetyLimits", "Safety Limits", true, null, null, null, null, null, null, 2, "jsonEditor" },
                    { 22, "Stage & Safety", "bool", "Có yêu cầu rehearsal trước giờ chạy show hay không.", true, false, true, true, null, "rehearsalRequired", "Rehearsal Required", true, null, null, null, null, null, null, 2, "switch" },
                    { 23, "Stage & Safety", "enum", "Mức độ rủi ro (do staff set) để phục vụ quản trị an toàn.", true, false, false, true, null, "riskLevel", "Risk Level", true, null, null, null, "[\"Low\",\"Medium\",\"High\"]", null, null, 2, "select" },
                    { 24, "Visual Style", "string", "Theme trang phục/LED cho robot (nếu có).", true, false, true, false, null, "costumeOrLedTheme", "Costume/LED Theme", true, null, 200, null, null, "VD: Neon / Tết / Christmas...", null, 2, "text" },
                    { 25, "Visual Style", "string", "1-2 câu chào mở đầu/kết thúc show.", true, true, false, false, null, "introOutroLines", "Intro/Outro Lines", true, null, 500, null, null, "Xin chào quý khách...", null, 2, "textarea" },
                    { 26, "Script Timeline", "json", "Kịch bản theo timeline (blockTitle/text/language/duration).", true, false, true, true, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"blockTitle\":{\"type\":\"string\"},\r\n              \"timecode\":{\"type\":\"string\"},\r\n              \"text\":{\"type\":\"string\"},\r\n              \"language\":{\"type\":\"string\"},\r\n              \"estimatedDurationSec\":{\"type\":\"integer\",\"minimum\":5},\r\n              \"interactionPrompts\":{\"type\":\"array\",\"items\":{\"type\":\"string\"}}\r\n            },\r\n            \"required\":[\"blockTitle\",\"text\"]\r\n          }\r\n        }", "scriptBlocks", "Script Blocks", true, null, null, null, null, null, null, 3, "jsonEditor" },
                    { 27, "Voice & Pronunciation", "json", "Từ điển phát âm (term/phonetic) cho tên riêng, thương hiệu.", true, false, false, false, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"term\":{\"type\":\"string\"},\r\n              \"phonetic\":{\"type\":\"string\"}\r\n            },\r\n            \"required\":[\"term\",\"phonetic\"]\r\n          }\r\n        }", "pronunciationDict", "Pronunciation Dictionary", true, null, null, null, null, null, null, 3, "jsonEditor" },
                    { 28, "Voice & Pronunciation", "json", "Cấu hình giọng MC (rate/pitch/volume).", true, true, false, false, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"voiceName\":{\"type\":\"string\"},\r\n            \"rate\":{\"type\":\"number\",\"minimum\":0.5,\"maximum\":2.0},\r\n            \"pitch\":{\"type\":\"number\",\"minimum\":-10,\"maximum\":10},\r\n            \"volume\":{\"type\":\"number\",\"minimum\":0,\"maximum\":100}\r\n          }\r\n        }", "voiceProfile", "Voice Profile", true, null, null, null, null, null, null, 3, "jsonEditor" },
                    { 29, "Voice & Pronunciation", "json", "Quy tắc âm lượng theo ngữ cảnh (quiet hours...).", true, true, false, false, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"defaultVolume\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":100},\r\n            \"quietHoursVolume\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":100}\r\n          }\r\n        }", "volumeRules", "Volume Rules", true, null, null, null, null, null, null, 3, "jsonEditor" },
                    { 30, "On-screen Assets", "json", "Asset hiển thị (QR/Image/Slide + thời lượng).", true, false, false, false, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"type\":{\"type\":\"string\"},\r\n              \"url\":{\"type\":\"string\"},\r\n              \"displayDurationSec\":{\"type\":\"integer\",\"minimum\":1}\r\n            },\r\n            \"required\":[\"type\",\"url\"]\r\n          }\r\n        }", "screenAssets", "On-screen Assets", true, null, null, null, null, null, null, 3, "jsonEditor" },
                    { 31, "On-screen Assets", "json", "Cấu hình countdown (nếu dùng).", true, true, false, false, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"enabled\":{\"type\":\"boolean\"},\r\n            \"targetTime\":{\"type\":\"string\"}\r\n          }\r\n        }", "countdownSettings", "Countdown Settings", true, null, null, null, null, null, null, 3, "jsonEditor" },
                    { 32, "Co-host Mode", "bool", "Có MC người phối hợp hay không.", true, false, false, true, null, "humanMcPresent", "Human MC Present", true, null, null, null, null, null, null, 3, "switch" },
                    { 33, "Co-host Mode", "json", "Cue chuyển giao giữa robot và MC người.", true, false, false, false, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"cue\":{\"type\":\"string\"},\r\n              \"who\":{\"type\":\"string\"}\r\n            },\r\n            \"required\":[\"cue\",\"who\"]\r\n          }\r\n        }", "handoffCues", "Handoff Cues", true, null, null, null, "[\"Robot\",\"Human\"]", null, null, 3, "jsonEditor" },
                    { 34, "Ad Playlist", "json", "Playlist quảng cáo (image/video + duration + order).", true, false, true, true, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"assetUrl\":{\"type\":\"string\"},\r\n              \"assetType\":{\"type\":\"string\"},\r\n              \"durationSec\":{\"type\":\"integer\",\"minimum\":1},\r\n              \"order\":{\"type\":\"integer\",\"minimum\":0}\r\n            },\r\n            \"required\":[\"assetUrl\",\"assetType\",\"durationSec\"]\r\n          }\r\n        }", "adPlaylist", "Ad Playlist", true, null, null, null, null, null, null, 4, "jsonEditor" },
                    { 35, "Ad Playlist", "json", "Quy tắc chạy playlist (khung giờ, interval, peak mode).", true, true, false, false, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"start\":{\"type\":\"string\"},\r\n            \"end\":{\"type\":\"string\"},\r\n            \"peakMode\":{\"type\":\"boolean\"},\r\n            \"intervalSec\":{\"type\":\"integer\",\"minimum\":5}\r\n          }\r\n        }", "scheduleRules", "Schedule Rules", true, null, null, null, null, null, null, 4, "jsonEditor" },
                    { 36, "Audio & Announcement", "json", "Danh sách audio (nhạc nền) nếu sử dụng.", true, false, false, false, "{ \"type\":\"array\", \"items\":{\"type\":\"string\"} }", "audioPlaylist", "Audio Playlist", true, null, null, null, null, null, null, 4, "jsonEditor" },
                    { 37, "Audio & Announcement", "string", "Script thông báo/mời chào tại booth.", true, false, false, false, null, "announcementScript", "Announcement Script", true, null, 2000, null, null, "Mời quý khách ghé booth...", null, 4, "textarea" },
                    { 38, "Audio & Announcement", "json", "Cấu hình âm lượng phát tại booth.", true, true, false, false, "{\r\n          \"type\":\"object\",\r\n          \"properties\":{\r\n            \"defaultVolume\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":100}\r\n          }\r\n        }", "volumeRules", "Volume Rules", true, null, null, null, null, null, null, 4, "jsonEditor" },
                    { 39, "CTA & Lead", "string", "Link/QR cho CTA (landing page, đăng ký...).", true, true, false, false, null, "ctaQrUrl", "CTA QR URL", true, null, 500, null, null, "https://...", null, 4, "url" },
                    { 40, "CTA & Lead", "string", "Text CTA hiển thị trên màn hình/booth.", true, true, false, false, null, "ctaText", "CTA Text", true, null, 200, null, null, "Quét QR để nhận ưu đãi!", null, 4, "text" },
                    { 41, "CTA & Lead", "string", "Luật voucher/ưu đãi (nếu có).", true, false, true, false, null, "voucherRule", "Voucher Rule", true, null, 2000, null, null, "VD: Giảm 10% cho 100 khách đầu tiên...", null, 4, "textarea" },
                    { 42, "Booth Route", "enum", "Chế độ di chuyển tại booth.", true, false, false, true, null, "routeMode", "Route Mode", true, null, null, null, "[\"Static\",\"Patrol\"]", null, null, 4, "select" },
                    { 43, "Booth Route", "json", "Các điểm dừng khi robot patrol.", true, false, true, false, "{\r\n          \"type\":\"array\",\r\n          \"items\":{\r\n            \"type\":\"object\",\r\n            \"properties\":{\r\n              \"name\":{\"type\":\"string\"},\r\n              \"stopDurationSec\":{\"type\":\"integer\",\"minimum\":1}\r\n            },\r\n            \"required\":[\"name\",\"stopDurationSec\"]\r\n          }\r\n        }", "routePoints", "Route Points", true, null, null, null, null, null, null, 4, "jsonEditor" },
                    { 44, "Booth Route", "json", "Khu vực tránh (nếu có).", true, true, false, false, "{ \"type\":\"array\", \"items\":{\"type\":\"string\"} }", "avoidZones", "Avoid Zones", true, null, null, null, null, null, null, 4, "jsonEditor" },
                    { 45, "Booth Route", "number", "Tốc độ tối đa (m/s) khi di chuyển.", true, true, false, false, null, "maxSpeed", "Max Speed", true, 2.0m, null, 0.1m, null, null, null, 4, "number" }
                });

            migrationBuilder.InsertData(
                table: "RobotTypeOfEvents",
                columns: new[] { "ActivityTypeId", "RoboTypeId", "Amount" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 1, 2, 1 },
                    { 1, 3, 1 },
                    { 1, 4, 1 },
                    { 2, 1, 1 },
                    { 2, 2, 2 },
                    { 2, 3, 1 },
                    { 2, 4, 2 },
                    { 3, 1, 2 },
                    { 3, 2, 3 },
                    { 3, 3, 2 },
                    { 3, 4, 3 }
                });

            migrationBuilder.InsertData(
                table: "Robots",
                columns: new[] { "Id", "IsDeleted", "Location", "ModelName", "RoboTypeId", "RobotName", "RobotStatus", "Specification", "Status" },
                values: new object[,]
                {
                    { 1, false, "", "REC-BASIC-01", 1, "REC-BASIC-01", "Stored", "", "" },
                    { 2, false, "", "REC-BASIC-02", 1, "REC-BASIC-02", "Stored", "", "" },
                    { 3, false, "", "REC-BASIC-03", 1, "REC-BASIC-03", "Stored", "", "" },
                    { 4, false, "", "REC-BASIC-04", 1, "REC-BASIC-04", "Stored", "", "" },
                    { 5, false, "", "PERF-BASIC-01", 2, "PERF-BASIC-01", "Stored", "", "" },
                    { 6, false, "", "PERF-BASIC-02", 2, "PERF-BASIC-02", "Stored", "", "" },
                    { 7, false, "", "PERF-BASIC-03", 2, "PERF-BASIC-03", "Stored", "", "" },
                    { 8, false, "", "PERF-BASIC-04", 2, "PERF-BASIC-04", "Stored", "", "" },
                    { 9, false, "", "HOST-BASIC-01", 3, "HOST-BASIC-01", "Stored", "", "" },
                    { 10, false, "", "HOST-BASIC-02", 3, "HOST-BASIC-02", "Stored", "", "" },
                    { 11, false, "", "HOST-BASIC-03", 3, "HOST-BASIC-03", "Stored", "", "" },
                    { 12, false, "", "HOST-BASIC-04", 3, "HOST-BASIC-04", "Stored", "", "" },
                    { 13, false, "", "PROMO-BASIC-01", 4, "PROMO-BASIC-01", "Stored", "", "" },
                    { 14, false, "", "PROMO-BASIC-02", 4, "PROMO-BASIC-02", "Stored", "", "" },
                    { 15, false, "", "PROMO-BASIC-03", 4, "PROMO-BASIC-03", "Stored", "", "" },
                    { 16, false, "", "PROMO-BASIC-04", 4, "PROMO-BASIC-04", "Stored", "", "" },
                    { 17, false, "", "REC-STANDARD-01", 1, "REC-STANDARD-01", "Stored", "", "" },
                    { 18, false, "", "REC-STANDARD-02", 1, "REC-STANDARD-02", "Stored", "", "" },
                    { 19, false, "", "REC-STANDARD-03", 1, "REC-STANDARD-03", "Stored", "", "" },
                    { 20, false, "", "REC-STANDARD-04", 1, "REC-STANDARD-04", "Stored", "", "" },
                    { 21, false, "", "PERF-STANDARD-01", 2, "PERF-STANDARD-01", "Stored", "", "" },
                    { 22, false, "", "PERF-STANDARD-02", 2, "PERF-STANDARD-02", "Stored", "", "" },
                    { 23, false, "", "PERF-STANDARD-03", 2, "PERF-STANDARD-03", "Stored", "", "" },
                    { 24, false, "", "PERF-STANDARD-04", 2, "PERF-STANDARD-04", "Stored", "", "" },
                    { 25, false, "", "PERF-STANDARD-05", 2, "PERF-STANDARD-05", "Stored", "", "" },
                    { 26, false, "", "PERF-STANDARD-06", 2, "PERF-STANDARD-06", "Stored", "", "" },
                    { 27, false, "", "PERF-STANDARD-07", 2, "PERF-STANDARD-07", "Stored", "", "" },
                    { 28, false, "", "PERF-STANDARD-08", 2, "PERF-STANDARD-08", "Stored", "", "" },
                    { 29, false, "", "HOST-STANDARD-01", 3, "HOST-STANDARD-01", "Stored", "", "" },
                    { 30, false, "", "HOST-STANDARD-02", 3, "HOST-STANDARD-02", "Stored", "", "" },
                    { 31, false, "", "HOST-STANDARD-03", 3, "HOST-STANDARD-03", "Stored", "", "" },
                    { 32, false, "", "HOST-STANDARD-04", 3, "HOST-STANDARD-04", "Stored", "", "" },
                    { 33, false, "", "PROMO-STANDARD-01", 4, "PROMO-STANDARD-01", "Stored", "", "" },
                    { 34, false, "", "PROMO-STANDARD-02", 4, "PROMO-STANDARD-02", "Stored", "", "" },
                    { 35, false, "", "PROMO-STANDARD-03", 4, "PROMO-STANDARD-03", "Stored", "", "" },
                    { 36, false, "", "PROMO-STANDARD-04", 4, "PROMO-STANDARD-04", "Stored", "", "" },
                    { 37, false, "", "PROMO-STANDARD-05", 4, "PROMO-STANDARD-05", "Stored", "", "" },
                    { 38, false, "", "PROMO-STANDARD-06", 4, "PROMO-STANDARD-06", "Stored", "", "" },
                    { 39, false, "", "PROMO-STANDARD-07", 4, "PROMO-STANDARD-07", "Stored", "", "" },
                    { 40, false, "", "PROMO-STANDARD-08", 4, "PROMO-STANDARD-08", "Stored", "", "" },
                    { 41, false, "", "REC-PREMIUM-01", 1, "REC-PREMIUM-01", "Stored", "", "" },
                    { 42, false, "", "REC-PREMIUM-02", 1, "REC-PREMIUM-02", "Stored", "", "" },
                    { 43, false, "", "REC-PREMIUM-03", 1, "REC-PREMIUM-03", "Stored", "", "" },
                    { 44, false, "", "REC-PREMIUM-04", 1, "REC-PREMIUM-04", "Stored", "", "" },
                    { 45, false, "", "REC-PREMIUM-05", 1, "REC-PREMIUM-05", "Stored", "", "" },
                    { 46, false, "", "REC-PREMIUM-06", 1, "REC-PREMIUM-06", "Stored", "", "" },
                    { 47, false, "", "REC-PREMIUM-07", 1, "REC-PREMIUM-07", "Stored", "", "" },
                    { 48, false, "", "REC-PREMIUM-08", 1, "REC-PREMIUM-08", "Stored", "", "" },
                    { 49, false, "", "PERF-PREMIUM-01", 2, "PERF-PREMIUM-01", "Stored", "", "" },
                    { 50, false, "", "PERF-PREMIUM-02", 2, "PERF-PREMIUM-02", "Stored", "", "" },
                    { 51, false, "", "PERF-PREMIUM-03", 2, "PERF-PREMIUM-03", "Stored", "", "" },
                    { 52, false, "", "PERF-PREMIUM-04", 2, "PERF-PREMIUM-04", "Stored", "", "" },
                    { 53, false, "", "PERF-PREMIUM-05", 2, "PERF-PREMIUM-05", "Stored", "", "" },
                    { 54, false, "", "PERF-PREMIUM-06", 2, "PERF-PREMIUM-06", "Stored", "", "" },
                    { 55, false, "", "PERF-PREMIUM-07", 2, "PERF-PREMIUM-07", "Stored", "", "" },
                    { 56, false, "", "PERF-PREMIUM-08", 2, "PERF-PREMIUM-08", "Stored", "", "" },
                    { 57, false, "", "PERF-PREMIUM-09", 2, "PERF-PREMIUM-09", "Stored", "", "" },
                    { 58, false, "", "PERF-PREMIUM-10", 2, "PERF-PREMIUM-10", "Stored", "", "" },
                    { 59, false, "", "PERF-PREMIUM-11", 2, "PERF-PREMIUM-11", "Stored", "", "" },
                    { 60, false, "", "PERF-PREMIUM-12", 2, "PERF-PREMIUM-12", "Stored", "", "" },
                    { 61, false, "", "HOST-PREMIUM-01", 3, "HOST-PREMIUM-01", "Stored", "", "" },
                    { 62, false, "", "HOST-PREMIUM-02", 3, "HOST-PREMIUM-02", "Stored", "", "" },
                    { 63, false, "", "HOST-PREMIUM-03", 3, "HOST-PREMIUM-03", "Stored", "", "" },
                    { 64, false, "", "HOST-PREMIUM-04", 3, "HOST-PREMIUM-04", "Stored", "", "" },
                    { 65, false, "", "HOST-PREMIUM-05", 3, "HOST-PREMIUM-05", "Stored", "", "" },
                    { 66, false, "", "HOST-PREMIUM-06", 3, "HOST-PREMIUM-06", "Stored", "", "" },
                    { 67, false, "", "HOST-PREMIUM-07", 3, "HOST-PREMIUM-07", "Stored", "", "" },
                    { 68, false, "", "HOST-PREMIUM-08", 3, "HOST-PREMIUM-08", "Stored", "", "" },
                    { 69, false, "", "PROMO-PREMIUM-01", 4, "PROMO-PREMIUM-01", "Stored", "", "" },
                    { 70, false, "", "PROMO-PREMIUM-02", 4, "PROMO-PREMIUM-02", "Stored", "", "" },
                    { 71, false, "", "PROMO-PREMIUM-03", 4, "PROMO-PREMIUM-03", "Stored", "", "" },
                    { 72, false, "", "PROMO-PREMIUM-04", 4, "PROMO-PREMIUM-04", "Stored", "", "" },
                    { 73, false, "", "PROMO-PREMIUM-05", 4, "PROMO-PREMIUM-05", "Stored", "", "" },
                    { 74, false, "", "PROMO-PREMIUM-06", 4, "PROMO-PREMIUM-06", "Stored", "", "" },
                    { 75, false, "", "PROMO-PREMIUM-07", 4, "PROMO-PREMIUM-07", "Stored", "", "" },
                    { 76, false, "", "PROMO-PREMIUM-08", 4, "PROMO-PREMIUM-08", "Stored", "", "" },
                    { 77, false, "", "PROMO-PREMIUM-09", 4, "PROMO-PREMIUM-09", "Stored", "", "" },
                    { 78, false, "", "PROMO-PREMIUM-10", 4, "PROMO-PREMIUM-10", "Stored", "", "" },
                    { 79, false, "", "PROMO-PREMIUM-11", 4, "PROMO-PREMIUM-11", "Stored", "", "" },
                    { 80, false, "", "PROMO-PREMIUM-12", 4, "PROMO-PREMIUM-12", "Stored", "", "" }
                });

            migrationBuilder.InsertData(
                table: "RobotInGroups",
                columns: new[] { "ActivityTypeGroupId", "RobotId", "IsDeleted" },
                values: new object[,]
                {
                    { 1, 1, false },
                    { 1, 2, false },
                    { 1, 3, false },
                    { 1, 4, false },
                    { 2, 5, false },
                    { 2, 6, false },
                    { 2, 7, false },
                    { 2, 8, false },
                    { 3, 9, false },
                    { 3, 10, false },
                    { 3, 11, false },
                    { 3, 12, false },
                    { 4, 13, false },
                    { 4, 14, false },
                    { 4, 15, false },
                    { 4, 16, false },
                    { 5, 17, false },
                    { 5, 18, false },
                    { 5, 19, false },
                    { 5, 20, false },
                    { 5, 21, false },
                    { 5, 22, false },
                    { 6, 23, false },
                    { 6, 24, false },
                    { 6, 25, false },
                    { 6, 26, false },
                    { 6, 27, false },
                    { 6, 28, false },
                    { 7, 29, false },
                    { 7, 30, false },
                    { 7, 31, false },
                    { 7, 32, false },
                    { 7, 33, false },
                    { 7, 34, false },
                    { 8, 35, false },
                    { 8, 36, false },
                    { 8, 37, false },
                    { 8, 38, false },
                    { 8, 39, false },
                    { 8, 40, false },
                    { 9, 41, false },
                    { 9, 42, false },
                    { 9, 43, false },
                    { 9, 44, false },
                    { 9, 45, false },
                    { 9, 46, false },
                    { 9, 47, false },
                    { 9, 48, false },
                    { 9, 49, false },
                    { 9, 50, false },
                    { 10, 51, false },
                    { 10, 52, false },
                    { 10, 53, false },
                    { 10, 54, false },
                    { 10, 55, false },
                    { 10, 56, false },
                    { 10, 57, false },
                    { 10, 58, false },
                    { 10, 59, false },
                    { 10, 60, false },
                    { 11, 61, false },
                    { 11, 62, false },
                    { 11, 63, false },
                    { 11, 64, false },
                    { 11, 65, false },
                    { 11, 66, false },
                    { 11, 67, false },
                    { 11, 68, false },
                    { 11, 69, false },
                    { 11, 70, false },
                    { 12, 71, false },
                    { 12, 72, false },
                    { 12, 73, false },
                    { 12, 74, false },
                    { 12, 75, false },
                    { 12, 76, false },
                    { 12, 77, false },
                    { 12, 78, false },
                    { 12, 79, false },
                    { 12, 80, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTypeGroups_ActivityTypeId",
                table: "ActivityTypeGroups",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTypes_EventActivityId",
                table: "ActivityTypes",
                column: "EventActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualDeliveries_GroupScheduleId",
                table: "ActualDeliveries",
                column: "GroupScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualDeliveries_StaffId",
                table: "ActualDeliveries",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatRoomId",
                table: "ChatMessages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_CustomerId",
                table: "ChatRooms",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_RentalId",
                table: "ChatRooms",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_StaffId",
                table: "ChatRooms",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDeliveries_ActualDeliveryId",
                table: "ChecklistDeliveries",
                column: "ActualDeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDeliveries_CheckedByStaffId",
                table: "ChecklistDeliveries",
                column: "CheckedByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDeliveries_CustomerAcceptedById",
                table: "ChecklistDeliveries",
                column: "CustomerAcceptedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDeliveryEvidences_ChecklistDeliveryId",
                table: "ChecklistDeliveryEvidences",
                column: "ChecklistDeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDeliveryEvidences_ChecklistDeliveryItemId",
                table: "ChecklistDeliveryEvidences",
                column: "ChecklistDeliveryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDeliveryEvidences_UploadedByStaffId",
                table: "ChecklistDeliveryEvidences",
                column: "UploadedByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDeliveryItems_ChecklistDeliveryId",
                table: "ChecklistDeliveryItems",
                column: "ChecklistDeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistDeliveryItemTemplates_RoboTypeId",
                table: "ChecklistDeliveryItemTemplates",
                column: "RoboTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDrafts_ContractTemplatesId",
                table: "ContractDrafts",
                column: "ContractTemplatesId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDrafts_ManagerId",
                table: "ContractDrafts",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDrafts_RentalId",
                table: "ContractDrafts",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDrafts_StaffId",
                table: "ContractDrafts",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractReports_AccusedId",
                table: "ContractReports",
                column: "AccusedId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractReports_DraftClausesId",
                table: "ContractReports",
                column: "DraftClausesId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractReports_PaymentId",
                table: "ContractReports",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractReports_ReporterId",
                table: "ContractReports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractReports_ReviewedBy",
                table: "ContractReports",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContractTemplates_CreatedBy",
                table: "ContractTemplates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContractTemplates_UpdatedBy",
                table: "ContractTemplates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContracts_ContractDraftsId",
                table: "CustomerContracts",
                column: "ContractDraftsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContracts_CustomerId",
                table: "CustomerContracts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContracts_ReviewerId",
                table: "CustomerContracts",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftApprovals_ContractDraftsId",
                table: "DraftApprovals",
                column: "ContractDraftsId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftApprovals_RequestedBy",
                table: "DraftApprovals",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DraftApprovals_ReviewerId",
                table: "DraftApprovals",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftClauses_ContractDraftsId",
                table: "DraftClauses",
                column: "ContractDraftsId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftClauses_TemplateClausesId",
                table: "DraftClauses",
                column: "TemplateClausesId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceProfiles_AccountId",
                table: "FaceProfiles",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceVerifications_AccountId",
                table: "FaceVerifications",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceVerifications_FaceProfileId",
                table: "FaceVerifications",
                column: "FaceProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceVerifications_RentalId",
                table: "FaceVerifications",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSchedules_ActivityTypeGroupId",
                table: "GroupSchedules",
                column: "ActivityTypeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSchedules_RentalId",
                table: "GroupSchedules",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientId",
                table: "Notifications",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RentalId",
                table: "Notifications",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecords_PriceQuoteId",
                table: "PaymentRecords",
                column: "PriceQuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecords_RentalId",
                table: "PaymentRecords",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceQuotes_ManagerId",
                table: "PriceQuotes",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceQuotes_RentalId",
                table: "PriceQuotes",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalChangeLogs_RentalId",
                table: "RentalChangeLogs",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalContracts_RentalId",
                table: "RentalContracts",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalDetails_RentalId",
                table: "RentalDetails",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalDetails_RoboTypeId",
                table: "RentalDetails",
                column: "RoboTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_AccountId",
                table: "Rentals",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_ActivityTypeId",
                table: "Rentals",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_EventActivityId",
                table: "Rentals",
                column: "EventActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_StaffId",
                table: "Rentals",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_RobotAbilities_RobotTypeId",
                table: "RobotAbilities",
                column: "RobotTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RobotAbilityValues_RentalDetailId",
                table: "RobotAbilityValues",
                column: "RentalDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_RobotAbilityValues_RobotAbilityId",
                table: "RobotAbilityValues",
                column: "RobotAbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RobotInGroups_RobotId",
                table: "RobotInGroups",
                column: "RobotId");

            migrationBuilder.CreateIndex(
                name: "IX_Robots_RoboTypeId",
                table: "Robots",
                column: "RoboTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RobotTypeOfEvents_RoboTypeId",
                table: "RobotTypeOfEvents",
                column: "RoboTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateClauses_ContractTemplatesId",
                table: "TemplateClauses",
                column: "ContractTemplatesId");

            migrationBuilder.CreateIndex(
                name: "IX_TypesOfRobos_RoboTypeId",
                table: "TypesOfRobos",
                column: "RoboTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChecklistDeliveryEvidences");

            migrationBuilder.DropTable(
                name: "ChecklistDeliveryItemTemplates");

            migrationBuilder.DropTable(
                name: "ContractReports");

            migrationBuilder.DropTable(
                name: "CustomerContracts");

            migrationBuilder.DropTable(
                name: "DraftApprovals");

            migrationBuilder.DropTable(
                name: "FaceVerifications");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RentalChangeLogs");

            migrationBuilder.DropTable(
                name: "RentalContracts");

            migrationBuilder.DropTable(
                name: "RobotAbilityValues");

            migrationBuilder.DropTable(
                name: "RobotInGroups");

            migrationBuilder.DropTable(
                name: "RobotTypeOfEvents");

            migrationBuilder.DropTable(
                name: "TypesOfRobos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.DropTable(
                name: "ChecklistDeliveryItems");

            migrationBuilder.DropTable(
                name: "DraftClauses");

            migrationBuilder.DropTable(
                name: "PaymentRecords");

            migrationBuilder.DropTable(
                name: "FaceProfiles");

            migrationBuilder.DropTable(
                name: "RentalDetails");

            migrationBuilder.DropTable(
                name: "RobotAbilities");

            migrationBuilder.DropTable(
                name: "Robots");

            migrationBuilder.DropTable(
                name: "ChecklistDeliveries");

            migrationBuilder.DropTable(
                name: "ContractDrafts");

            migrationBuilder.DropTable(
                name: "TemplateClauses");

            migrationBuilder.DropTable(
                name: "PriceQuotes");

            migrationBuilder.DropTable(
                name: "RoboTypes");

            migrationBuilder.DropTable(
                name: "ActualDeliveries");

            migrationBuilder.DropTable(
                name: "ContractTemplates");

            migrationBuilder.DropTable(
                name: "GroupSchedules");

            migrationBuilder.DropTable(
                name: "ActivityTypeGroups");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "ActivityTypes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EventActivity");
        }
    }
}
