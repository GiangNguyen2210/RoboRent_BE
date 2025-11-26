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
                name: "EventActivities",
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
                    table.PrimaryKey("PK_EventActivities", x => x.Id);
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
                    EventActivityId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityTypes_EventActivities_EventActivityId",
                        column: x => x.EventActivityId,
                        principalTable: "EventActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RobotAbilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoboTypeId = table.Column<int>(type: "integer", nullable: true),
                    Ability = table.Column<string>(type: "text", nullable: true),
                    IsCustomizable = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RobotAbilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RobotAbilities_RoboTypes_RoboTypeId",
                        column: x => x.RoboTypeId,
                        principalTable: "RoboTypes",
                        principalColumn: "Id");
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
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderCode = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PaymentLinkId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Status = table.Column<string>(type: "text", nullable: true),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    EventActivityId = table.Column<int>(type: "integer", nullable: true),
                    ActivityTypeId = table.Column<int>(type: "integer", nullable: true),
                    StaffId = table.Column<int>(type: "integer", nullable: true),
                    PlannedDeliveryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PlannedPickupTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                        name: "FK_Rentals_EventActivities_EventActivityId",
                        column: x => x.EventActivityId,
                        principalTable: "EventActivities",
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
                name: "ActualDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalId = table.Column<int>(type: "integer", nullable: false),
                    StaffId = table.Column<int>(type: "integer", nullable: true),
                    ScheduledDeliveryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualDeliveryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ScheduledPickupTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualPickupTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CustomerRequestNotes = table.Column<string>(type: "text", nullable: true),
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
                        name: "FK_ActualDeliveries_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
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
                name: "GroupSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EventLocation = table.Column<string>(type: "text", nullable: true),
                    EventCity = table.Column<string>(type: "text", nullable: true),
                    DeliveryTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
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
                name: "PriceQuotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RentalId = table.Column<int>(type: "integer", nullable: false),
                    Delivery = table.Column<double>(type: "double precision", nullable: true),
                    Deposit = table.Column<double>(type: "double precision", nullable: true),
                    Complete = table.Column<double>(type: "double precision", nullable: true),
                    Service = table.Column<double>(type: "double precision", nullable: true),
                    StaffDescription = table.Column<string>(type: "text", nullable: true),
                    ManagerFeedback = table.Column<string>(type: "text", nullable: true),
                    CustomerReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    DeliveryFee = table.Column<decimal>(type: "numeric", nullable: true),
                    DeliveryDistance = table.Column<int>(type: "integer", nullable: true),
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
                    RobotAbilityId = table.Column<int>(type: "integer", nullable: true),
                    Script = table.Column<string>(type: "text", nullable: true),
                    Branding = table.Column<string>(type: "text", nullable: true),
                    Scenario = table.Column<string>(type: "text", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_RentalDetails_RobotAbilities_RobotAbilityId",
                        column: x => x.RobotAbilityId,
                        principalTable: "RobotAbilities",
                        principalColumn: "Id");
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
                name: "ContractReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DraftClausesId = table.Column<int>(type: "integer", nullable: true),
                    ReporterId = table.Column<int>(type: "integer", nullable: true),
                    ReportRole = table.Column<string>(type: "text", nullable: true),
                    AccusedId = table.Column<int>(type: "integer", nullable: true),
                    ReportCategory = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EvidencePath = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Resolution = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedBy = table.Column<int>(type: "integer", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolutionType = table.Column<string>(type: "text", nullable: true),
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
                        name: "FK_ContractReports_PaymentTransactions_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "Staff", "STAFF" },
                    { "3", null, "Customer", "CUSTOMER" },
                    { "4", null, "Manager", "MANAGER" }
                });

            migrationBuilder.InsertData(
                table: "EventActivities",
                columns: new[] { "Id", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, "Robot biểu diễn trước khán giả", false, "Performance" },
                    { 2, "Robot giao tiếp, nói, dẫn chương trình", false, "Presentation & Hosting" },
                    { 3, "Robot tương tác nội dung với khán giả", false, "Storytelling & Interaction" },
                    { 4, "Robot hỗ trợ event nhưng không phải “tiết mục biểu diễn chính”", false, "Entertainment Support" },
                    { 5, "Hoạt động marketing, bán hàng, PR", false, "Promotion & Marketing" }
                });

            migrationBuilder.InsertData(
                table: "RoboTypes",
                columns: new[] { "Id", "Description", "IsDeleted", "TypeName" },
                values: new object[,]
                {
                    { 1, "Robot hình người, có thể nhảy múa, di chuyển linh hoạt", false, "Humanoid Performance Robot" },
                    { 2, "Robot chuyên thực hiện nhảy múa, hành động đồng bộ theo nhóm", false, "Dance / Choreography Robot" },
                    { 3, "Robot diễn xuất kịch, có khả năng cử chỉ, biểu cảm", false, "Acting Robot / Drama Robot" },
                    { 5, "Robot dẫn chương trình, có thể giao tiếp và tương tác với khán giả", false, "Host / MC Robot" },
                    { 6, "Robot trình bày sản phẩm, nội dung, hỗ trợ thuyết minh tại sự kiện", false, "Presentation Robot" },
                    { 7, "Robot cho phép thuyết trình, giao tiếp từ xa thông qua điều khiển", false, "Telepresence Robot" },
                    { 8, "Robot kể chuyện, tương tác với người nghe, phù hợp cho trẻ em và giáo dục", false, "Storytelling Robot" },
                    { 9, "Robot giao tiếp, hỏi đáp, tương tác và tham gia trò chơi với khán giả", false, "Interaction / Social Robot" },
                    { 10, "Robot dẫn các trò chơi nhỏ, quiz, minigame tương tác cùng khán giả", false, "Game / Quiz Robot" },
                    { 11, "Robot chào đón khách, hướng dẫn khách, hỗ trợ check-in tại sự kiện", false, "Hospitality Robot" },
                    { 12, "Robot hỗ trợ chụp ảnh với khách tham dự, tạo trải nghiệm tương tác vui vẻ", false, "Selfie / Photo Bot" },
                    { 13, "Robot dạng mascot hoặc hoạt hình chuyển động tạo không khí sự kiện", false, "Animatronic Robot" },
                    { 14, "Robot thu hút khách tại booth, kích hoạt thương hiệu và tạo điểm nhấn sự kiện", false, "Brand Activation Robot" },
                    { 15, "Robot giới thiệu tính năng sản phẩm, hướng dẫn dùng thử, demo công nghệ", false, "Product Demo Robot" },
                    { 16, "Robot quảng bá, mời chào, dẫn traffic và hỗ trợ hoạt động marketing", false, "Promotional Robot" }
                });

            migrationBuilder.InsertData(
                table: "ActivityTypes",
                columns: new[] { "Id", "EventActivityId", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, 1, false, "Solo Singing" },
                    { 2, 1, false, "Duet Singing" },
                    { 3, 1, false, "Birthday Song Performance" },
                    { 4, 1, false, "Instrument Simulation Performance" },
                    { 5, 1, false, "Solo Dance" },
                    { 6, 1, false, "Group Robot Dance" },
                    { 7, 1, false, "Kids Dance Performance" },
                    { 8, 1, false, "Drama Acting" },
                    { 9, 1, false, "Comedy Performance" },
                    { 10, 1, false, "Theme Performance (Holiday/Seasonal)" },
                    { 11, 2, false, "Main Event Hosting (MC)" },
                    { 12, 2, false, "Supporting MC" },
                    { 13, 2, false, "Product Presentation" },
                    { 14, 2, false, "Corporate Introduction" },
                    { 15, 2, false, "Educational Presentation" },
                    { 16, 2, false, "Ceremony Opening Speech" },
                    { 17, 2, false, "Guest Introduction" },
                    { 18, 2, false, "Event Flow Guidance" },
                    { 19, 3, false, "Fairy Tale Storytelling" },
                    { 20, 3, false, "Inspirational Storytelling" },
                    { 21, 3, false, "Historical Storytelling" },
                    { 22, 3, false, "Poetry Reading" },
                    { 23, 3, false, "Q&A Interaction" },
                    { 24, 3, false, "AI Scripted Conversation" },
                    { 25, 3, false, "Audience Interview" },
                    { 26, 3, false, "Trivia Quiz Hosting" },
                    { 27, 3, false, "Mini Games with Audience" },
                    { 28, 4, false, "Greeting Guests" },
                    { 29, 4, false, "Selfie with Guests" },
                    { 30, 4, false, "Guest Check-in Support" },
                    { 31, 4, false, "Event Area Tour Guidance" },
                    { 32, 4, false, "Crowd Engagement" },
                    { 33, 4, false, "Short Dance Break" },
                    { 34, 4, false, "Birthday Celebration Support" },
                    { 35, 4, false, "Children Entertainment Interaction" },
                    { 36, 5, false, "Product Review" },
                    { 37, 5, false, "Promotional Announcement" },
                    { 38, 5, false, "Sampling Distribution" },
                    { 39, 5, false, "Brand Activation Speech" },
                    { 40, 5, false, "Customer Product Consultation" },
                    { 41, 5, false, "Collect Customer Information" },
                    { 42, 5, false, "Feature Demonstration" }
                });

            migrationBuilder.InsertData(
                table: "Robots",
                columns: new[] { "Id", "IsDeleted", "Location", "ModelName", "RoboTypeId", "RobotName", "RobotStatus", "Specification", "Status" },
                values: new object[,]
                {
                    { 1, false, "", "HPR-01", 1, "HPR-01", "Stored", "", "" },
                    { 2, false, "", "HPR-02", 1, "HPR-02", "Stored", "", "" },
                    { 3, false, "", "HPR-03", 1, "HPR-03", "Stored", "", "" },
                    { 4, false, "", "HPR-04", 1, "HPR-04", "Stored", "", "" },
                    { 5, false, "", "HPR-05", 1, "HPR-05", "Stored", "", "" },
                    { 6, false, "", "HPR-06", 1, "HPR-06", "Stored", "", "" },
                    { 7, false, "", "HPR-07", 1, "HPR-07", "Stored", "", "" },
                    { 8, false, "", "HPR-08", 1, "HPR-08", "Stored", "", "" },
                    { 9, false, "", "HPR-09", 1, "HPR-09", "Stored", "", "" },
                    { 10, false, "", "HPR-10", 1, "HPR-10", "Stored", "", "" },
                    { 11, false, "", "DCR-01", 2, "DCR-01", "Stored", "", "" },
                    { 12, false, "", "DCR-02", 2, "DCR-02", "Stored", "", "" },
                    { 13, false, "", "DCR-03", 2, "DCR-03", "Stored", "", "" },
                    { 14, false, "", "DCR-04", 2, "DCR-04", "Stored", "", "" },
                    { 15, false, "", "DCR-05", 2, "DCR-05", "Stored", "", "" },
                    { 16, false, "", "DCR-06", 2, "DCR-06", "Stored", "", "" },
                    { 17, false, "", "DCR-07", 2, "DCR-07", "Stored", "", "" },
                    { 18, false, "", "DCR-08", 2, "DCR-08", "Stored", "", "" },
                    { 19, false, "", "DCR-09", 2, "DCR-09", "Stored", "", "" },
                    { 20, false, "", "DCR-10", 2, "DCR-10", "Stored", "", "" },
                    { 21, false, "", "ARR-01", 3, "ARR-01", "Stored", "", "" },
                    { 22, false, "", "ARR-02", 3, "ARR-02", "Stored", "", "" },
                    { 23, false, "", "ARR-03", 3, "ARR-03", "Stored", "", "" },
                    { 24, false, "", "ARR-04", 3, "ARR-04", "Stored", "", "" },
                    { 25, false, "", "ARR-05", 3, "ARR-05", "Stored", "", "" },
                    { 26, false, "", "ARR-06", 3, "ARR-06", "Stored", "", "" },
                    { 27, false, "", "ARR-07", 3, "ARR-07", "Stored", "", "" },
                    { 28, false, "", "ARR-08", 3, "ARR-08", "Stored", "", "" },
                    { 29, false, "", "ARR-09", 3, "ARR-09", "Stored", "", "" },
                    { 30, false, "", "ARR-10", 3, "ARR-10", "Stored", "", "" },
                    { 41, false, "", "HMR-01", 5, "HMR-01", "Stored", "", "" },
                    { 42, false, "", "HMR-02", 5, "HMR-02", "Stored", "", "" },
                    { 43, false, "", "HMR-03", 5, "HMR-03", "Stored", "", "" },
                    { 44, false, "", "HMR-04", 5, "HMR-04", "Stored", "", "" },
                    { 45, false, "", "HMR-05", 5, "HMR-05", "Stored", "", "" },
                    { 46, false, "", "HMR-06", 5, "HMR-06", "Stored", "", "" },
                    { 47, false, "", "HMR-07", 5, "HMR-07", "Stored", "", "" },
                    { 48, false, "", "HMR-08", 5, "HMR-08", "Stored", "", "" },
                    { 49, false, "", "HMR-09", 5, "HMR-09", "Stored", "", "" },
                    { 50, false, "", "HMR-10", 5, "HMR-10", "Stored", "", "" },
                    { 51, false, "", "PRR-01", 6, "PRR-01", "Stored", "", "" },
                    { 52, false, "", "PRR-02", 6, "PRR-02", "Stored", "", "" },
                    { 53, false, "", "PRR-03", 6, "PRR-03", "Stored", "", "" },
                    { 54, false, "", "PRR-04", 6, "PRR-04", "Stored", "", "" },
                    { 55, false, "", "PRR-05", 6, "PRR-05", "Stored", "", "" },
                    { 56, false, "", "PRR-06", 6, "PRR-06", "Stored", "", "" },
                    { 57, false, "", "PRR-07", 6, "PRR-07", "Stored", "", "" },
                    { 58, false, "", "PRR-08", 6, "PRR-08", "Stored", "", "" },
                    { 59, false, "", "PRR-09", 6, "PRR-09", "Stored", "", "" },
                    { 60, false, "", "PRR-10", 6, "PRR-10", "Stored", "", "" },
                    { 61, false, "", "TPR-01", 7, "TPR-01", "Stored", "", "" },
                    { 62, false, "", "TPR-02", 7, "TPR-02", "Stored", "", "" },
                    { 63, false, "", "TPR-03", 7, "TPR-03", "Stored", "", "" },
                    { 64, false, "", "TPR-04", 7, "TPR-04", "Stored", "", "" },
                    { 65, false, "", "TPR-05", 7, "TPR-05", "Stored", "", "" },
                    { 66, false, "", "TPR-06", 7, "TPR-06", "Stored", "", "" },
                    { 67, false, "", "TPR-07", 7, "TPR-07", "Stored", "", "" },
                    { 68, false, "", "TPR-08", 7, "TPR-08", "Stored", "", "" },
                    { 69, false, "", "TPR-09", 7, "TPR-09", "Stored", "", "" },
                    { 70, false, "", "TPR-10", 7, "TPR-10", "Stored", "", "" },
                    { 71, false, "", "STR-01", 8, "STR-01", "Stored", "", "" },
                    { 72, false, "", "STR-02", 8, "STR-02", "Stored", "", "" },
                    { 73, false, "", "STR-03", 8, "STR-03", "Stored", "", "" },
                    { 74, false, "", "STR-04", 8, "STR-04", "Stored", "", "" },
                    { 75, false, "", "STR-05", 8, "STR-05", "Stored", "", "" },
                    { 76, false, "", "STR-06", 8, "STR-06", "Stored", "", "" },
                    { 77, false, "", "STR-07", 8, "STR-07", "Stored", "", "" },
                    { 78, false, "", "STR-08", 8, "STR-08", "Stored", "", "" },
                    { 79, false, "", "STR-09", 8, "STR-09", "Stored", "", "" },
                    { 80, false, "", "STR-10", 8, "STR-10", "Stored", "", "" },
                    { 81, false, "", "ISR-01", 9, "ISR-01", "Stored", "", "" },
                    { 82, false, "", "ISR-02", 9, "ISR-02", "Stored", "", "" },
                    { 83, false, "", "ISR-03", 9, "ISR-03", "Stored", "", "" },
                    { 84, false, "", "ISR-04", 9, "ISR-04", "Stored", "", "" },
                    { 85, false, "", "ISR-05", 9, "ISR-05", "Stored", "", "" },
                    { 86, false, "", "ISR-06", 9, "ISR-06", "Stored", "", "" },
                    { 87, false, "", "ISR-07", 9, "ISR-07", "Stored", "", "" },
                    { 88, false, "", "ISR-08", 9, "ISR-08", "Stored", "", "" },
                    { 89, false, "", "ISR-09", 9, "ISR-09", "Stored", "", "" },
                    { 90, false, "", "ISR-10", 9, "ISR-10", "Stored", "", "" },
                    { 91, false, "", "GQR-01", 10, "GQR-01", "Stored", "", "" },
                    { 92, false, "", "GQR-02", 10, "GQR-02", "Stored", "", "" },
                    { 93, false, "", "GQR-03", 10, "GQR-03", "Stored", "", "" },
                    { 94, false, "", "GQR-04", 10, "GQR-04", "Stored", "", "" },
                    { 95, false, "", "GQR-05", 10, "GQR-05", "Stored", "", "" },
                    { 96, false, "", "GQR-06", 10, "GQR-06", "Stored", "", "" },
                    { 97, false, "", "GQR-07", 10, "GQR-07", "Stored", "", "" },
                    { 98, false, "", "GQR-08", 10, "GQR-08", "Stored", "", "" },
                    { 99, false, "", "GQR-09", 10, "GQR-09", "Stored", "", "" },
                    { 100, false, "", "GQR-10", 10, "GQR-10", "Stored", "", "" },
                    { 101, false, "", "HSR-01", 11, "HSR-01", "Stored", "", "" },
                    { 102, false, "", "HSR-02", 11, "HSR-02", "Stored", "", "" },
                    { 103, false, "", "HSR-03", 11, "HSR-03", "Stored", "", "" },
                    { 104, false, "", "HSR-04", 11, "HSR-04", "Stored", "", "" },
                    { 105, false, "", "HSR-05", 11, "HSR-05", "Stored", "", "" },
                    { 106, false, "", "HSR-06", 11, "HSR-06", "Stored", "", "" },
                    { 107, false, "", "HSR-07", 11, "HSR-07", "Stored", "", "" },
                    { 108, false, "", "HSR-08", 11, "HSR-08", "Stored", "", "" },
                    { 109, false, "", "HSR-09", 11, "HSR-09", "Stored", "", "" },
                    { 110, false, "", "HSR-10", 11, "HSR-10", "Stored", "", "" },
                    { 111, false, "", "SPB-01", 12, "SPB-01", "Stored", "", "" },
                    { 112, false, "", "SPB-02", 12, "SPB-02", "Stored", "", "" },
                    { 113, false, "", "SPB-03", 12, "SPB-03", "Stored", "", "" },
                    { 114, false, "", "SPB-04", 12, "SPB-04", "Stored", "", "" },
                    { 115, false, "", "SPB-05", 12, "SPB-05", "Stored", "", "" },
                    { 116, false, "", "SPB-06", 12, "SPB-06", "Stored", "", "" },
                    { 117, false, "", "SPB-07", 12, "SPB-07", "Stored", "", "" },
                    { 118, false, "", "SPB-08", 12, "SPB-08", "Stored", "", "" },
                    { 119, false, "", "SPB-09", 12, "SPB-09", "Stored", "", "" },
                    { 120, false, "", "SPB-10", 12, "SPB-10", "Stored", "", "" },
                    { 121, false, "", "ANR-01", 13, "ANR-01", "Stored", "", "" },
                    { 122, false, "", "ANR-02", 13, "ANR-02", "Stored", "", "" },
                    { 123, false, "", "ANR-03", 13, "ANR-03", "Stored", "", "" },
                    { 124, false, "", "ANR-04", 13, "ANR-04", "Stored", "", "" },
                    { 125, false, "", "ANR-05", 13, "ANR-05", "Stored", "", "" },
                    { 126, false, "", "ANR-06", 13, "ANR-06", "Stored", "", "" },
                    { 127, false, "", "ANR-07", 13, "ANR-07", "Stored", "", "" },
                    { 128, false, "", "ANR-08", 13, "ANR-08", "Stored", "", "" },
                    { 129, false, "", "ANR-09", 13, "ANR-09", "Stored", "", "" },
                    { 130, false, "", "ANR-10", 13, "ANR-10", "Stored", "", "" },
                    { 131, false, "", "BAR-01", 14, "BAR-01", "Stored", "", "" },
                    { 132, false, "", "BAR-02", 14, "BAR-02", "Stored", "", "" },
                    { 133, false, "", "BAR-03", 14, "BAR-03", "Stored", "", "" },
                    { 134, false, "", "BAR-04", 14, "BAR-04", "Stored", "", "" },
                    { 135, false, "", "BAR-05", 14, "BAR-05", "Stored", "", "" },
                    { 136, false, "", "BAR-06", 14, "BAR-06", "Stored", "", "" },
                    { 137, false, "", "BAR-07", 14, "BAR-07", "Stored", "", "" },
                    { 138, false, "", "BAR-08", 14, "BAR-08", "Stored", "", "" },
                    { 139, false, "", "BAR-09", 14, "BAR-09", "Stored", "", "" },
                    { 140, false, "", "BAR-10", 14, "BAR-10", "Stored", "", "" },
                    { 141, false, "", "PDR-01", 15, "PDR-01", "Stored", "", "" },
                    { 142, false, "", "PDR-02", 15, "PDR-02", "Stored", "", "" },
                    { 143, false, "", "PDR-03", 15, "PDR-03", "Stored", "", "" },
                    { 144, false, "", "PDR-04", 15, "PDR-04", "Stored", "", "" },
                    { 145, false, "", "PDR-05", 15, "PDR-05", "Stored", "", "" },
                    { 146, false, "", "PDR-06", 15, "PDR-06", "Stored", "", "" },
                    { 147, false, "", "PDR-07", 15, "PDR-07", "Stored", "", "" },
                    { 148, false, "", "PDR-08", 15, "PDR-08", "Stored", "", "" },
                    { 149, false, "", "PDR-09", 15, "PDR-09", "Stored", "", "" },
                    { 150, false, "", "PDR-10", 15, "PDR-10", "Stored", "", "" },
                    { 151, false, "", "PMR-01", 16, "PMR-01", "Stored", "", "" },
                    { 152, false, "", "PMR-02", 16, "PMR-02", "Stored", "", "" },
                    { 153, false, "", "PMR-03", 16, "PMR-03", "Stored", "", "" },
                    { 154, false, "", "PMR-04", 16, "PMR-04", "Stored", "", "" },
                    { 155, false, "", "PMR-05", 16, "PMR-05", "Stored", "", "" },
                    { 156, false, "", "PMR-06", 16, "PMR-06", "Stored", "", "" },
                    { 157, false, "", "PMR-07", 16, "PMR-07", "Stored", "", "" },
                    { 158, false, "", "PMR-08", 16, "PMR-08", "Stored", "", "" },
                    { 159, false, "", "PMR-09", 16, "PMR-09", "Stored", "", "" },
                    { 160, false, "", "PMR-10", 16, "PMR-10", "Stored", "", "" }
                });

            migrationBuilder.InsertData(
                table: "ActivityTypeGroups",
                columns: new[] { "Id", "ActivityTypeId", "IsDeleted" },
                values: new object[,]
                {
                    { 1, 1, false },
                    { 2, 2, false },
                    { 3, 3, false },
                    { 4, 4, false },
                    { 5, 5, false },
                    { 6, 6, false },
                    { 7, 7, false },
                    { 8, 8, false },
                    { 9, 9, false },
                    { 10, 10, false },
                    { 11, 11, false },
                    { 12, 12, false },
                    { 13, 13, false },
                    { 14, 14, false },
                    { 15, 15, false },
                    { 16, 16, false },
                    { 17, 17, false },
                    { 18, 18, false },
                    { 19, 19, false },
                    { 20, 20, false },
                    { 21, 21, false },
                    { 22, 22, false },
                    { 23, 23, false },
                    { 24, 24, false },
                    { 25, 25, false },
                    { 26, 26, false },
                    { 27, 27, false },
                    { 28, 28, false },
                    { 29, 29, false },
                    { 30, 30, false },
                    { 31, 31, false },
                    { 32, 32, false },
                    { 33, 33, false },
                    { 34, 34, false },
                    { 35, 35, false },
                    { 36, 36, false },
                    { 37, 37, false },
                    { 38, 38, false },
                    { 39, 39, false },
                    { 40, 40, false },
                    { 41, 41, false },
                    { 42, 42, false },
                    { 43, 6, false },
                    { 44, 9, false },
                    { 45, 9, false },
                    { 46, 9, false },
                    { 47, 13, false },
                    { 48, 13, false },
                    { 49, 13, false },
                    { 50, 13, false },
                    { 51, 15, false },
                    { 52, 15, false },
                    { 53, 11, false },
                    { 54, 11, false },
                    { 55, 12, false },
                    { 56, 16, false },
                    { 57, 16, false },
                    { 58, 17, false },
                    { 59, 19, false },
                    { 60, 20, false },
                    { 61, 21, false },
                    { 62, 22, false },
                    { 63, 26, false },
                    { 64, 26, false },
                    { 65, 27, false },
                    { 66, 27, false },
                    { 67, 25, false },
                    { 68, 34, false },
                    { 69, 34, false },
                    { 70, 32, false },
                    { 71, 33, false },
                    { 72, 35, false },
                    { 73, 35, false },
                    { 74, 35, false },
                    { 75, 39, false },
                    { 76, 39, false },
                    { 77, 41, false },
                    { 78, 40, false },
                    { 79, 38, false },
                    { 80, 42, false },
                    { 81, 42, false },
                    { 82, 36, false },
                    { 83, 36, false },
                    { 84, 36, false },
                    { 85, 36, false }
                });

            migrationBuilder.InsertData(
                table: "RobotTypeOfEvents",
                columns: new[] { "ActivityTypeId", "RoboTypeId", "Amount" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 1, 1 },
                    { 4, 1, 1 },
                    { 5, 2, 1 },
                    { 6, 1, 1 },
                    { 6, 2, 3 },
                    { 7, 1, 1 },
                    { 7, 2, 2 },
                    { 8, 1, 1 },
                    { 8, 3, 2 },
                    { 9, 3, 2 },
                    { 10, 1, 2 },
                    { 10, 2, 1 },
                    { 11, 5, 1 },
                    { 12, 5, 1 },
                    { 13, 6, 1 },
                    { 13, 7, 1 },
                    { 14, 6, 1 },
                    { 15, 6, 1 },
                    { 15, 7, 1 },
                    { 16, 5, 1 },
                    { 17, 5, 1 },
                    { 18, 5, 1 },
                    { 19, 8, 1 },
                    { 20, 8, 1 },
                    { 21, 8, 1 },
                    { 22, 8, 1 },
                    { 23, 8, 1 },
                    { 23, 9, 1 },
                    { 24, 8, 1 },
                    { 24, 9, 1 },
                    { 25, 9, 1 },
                    { 26, 9, 1 },
                    { 26, 10, 1 },
                    { 27, 9, 1 },
                    { 27, 10, 1 },
                    { 28, 11, 1 },
                    { 29, 12, 1 },
                    { 30, 11, 1 },
                    { 31, 11, 1 },
                    { 32, 11, 1 },
                    { 32, 12, 1 },
                    { 33, 11, 1 },
                    { 33, 13, 1 },
                    { 34, 11, 1 },
                    { 34, 12, 1 },
                    { 34, 13, 1 },
                    { 35, 12, 1 },
                    { 35, 13, 1 },
                    { 36, 15, 1 },
                    { 37, 16, 1 },
                    { 38, 14, 1 },
                    { 38, 16, 1 },
                    { 39, 14, 1 },
                    { 39, 16, 1 },
                    { 40, 15, 1 },
                    { 40, 16, 1 },
                    { 41, 14, 1 },
                    { 41, 16, 1 },
                    { 42, 14, 1 },
                    { 42, 15, 1 }
                });

            migrationBuilder.InsertData(
                table: "RobotInGroups",
                columns: new[] { "ActivityTypeGroupId", "RobotId", "IsDeleted" },
                values: new object[,]
                {
                    { 1, 1, false },
                    { 2, 2, false },
                    { 2, 11, false },
                    { 3, 3, false },
                    { 4, 4, false },
                    { 5, 12, false },
                    { 6, 5, false },
                    { 6, 13, false },
                    { 6, 14, false },
                    { 6, 15, false },
                    { 7, 6, false },
                    { 7, 16, false },
                    { 7, 17, false },
                    { 8, 7, false },
                    { 8, 21, false },
                    { 8, 22, false },
                    { 9, 23, false },
                    { 9, 24, false },
                    { 10, 8, false },
                    { 10, 9, false },
                    { 11, 41, false },
                    { 12, 42, false },
                    { 13, 51, false },
                    { 13, 61, false },
                    { 14, 52, false },
                    { 15, 59, false },
                    { 15, 62, false },
                    { 16, 43, false },
                    { 17, 44, false },
                    { 18, 45, false },
                    { 19, 71, false },
                    { 20, 72, false },
                    { 21, 73, false },
                    { 22, 74, false },
                    { 23, 75, false },
                    { 23, 81, false },
                    { 24, 76, false },
                    { 24, 82, false },
                    { 25, 83, false },
                    { 26, 84, false },
                    { 26, 91, false },
                    { 27, 85, false },
                    { 27, 92, false },
                    { 28, 101, false },
                    { 29, 111, false },
                    { 30, 102, false },
                    { 31, 103, false },
                    { 32, 104, false },
                    { 32, 112, false },
                    { 33, 105, false },
                    { 33, 121, false },
                    { 34, 106, false },
                    { 34, 113, false },
                    { 34, 122, false },
                    { 35, 114, false },
                    { 35, 123, false },
                    { 36, 141, false },
                    { 37, 151, false },
                    { 38, 131, false },
                    { 38, 152, false },
                    { 39, 132, false },
                    { 39, 153, false },
                    { 40, 142, false },
                    { 40, 154, false },
                    { 41, 133, false },
                    { 41, 155, false },
                    { 42, 134, false },
                    { 42, 143, false },
                    { 43, 10, false },
                    { 43, 18, false },
                    { 43, 19, false },
                    { 43, 20, false },
                    { 44, 25, false },
                    { 44, 26, false },
                    { 45, 27, false },
                    { 45, 28, false },
                    { 46, 29, false },
                    { 46, 30, false },
                    { 47, 53, false },
                    { 47, 63, false },
                    { 48, 54, false },
                    { 48, 64, false },
                    { 49, 55, false },
                    { 49, 65, false },
                    { 50, 56, false },
                    { 50, 66, false },
                    { 51, 57, false },
                    { 51, 67, false },
                    { 52, 58, false },
                    { 52, 68, false },
                    { 53, 45, false },
                    { 54, 46, false },
                    { 55, 47, false },
                    { 56, 48, false },
                    { 57, 49, false },
                    { 58, 50, false },
                    { 59, 77, false },
                    { 60, 78, false },
                    { 61, 79, false },
                    { 62, 80, false },
                    { 63, 86, false },
                    { 63, 93, false },
                    { 64, 87, false },
                    { 64, 94, false },
                    { 65, 88, false },
                    { 65, 95, false },
                    { 66, 89, false },
                    { 66, 96, false },
                    { 67, 90, false },
                    { 68, 107, false },
                    { 68, 115, false },
                    { 68, 124, false },
                    { 69, 108, false },
                    { 69, 116, false },
                    { 69, 125, false },
                    { 70, 109, false },
                    { 70, 117, false },
                    { 71, 110, false },
                    { 71, 126, false },
                    { 72, 118, false },
                    { 72, 127, false },
                    { 73, 119, false },
                    { 73, 128, false },
                    { 74, 120, false },
                    { 74, 129, false },
                    { 75, 135, false },
                    { 75, 156, false },
                    { 76, 136, false },
                    { 76, 157, false },
                    { 77, 137, false },
                    { 77, 158, false },
                    { 78, 144, false },
                    { 78, 159, false },
                    { 79, 138, false },
                    { 79, 160, false },
                    { 80, 139, false },
                    { 80, 145, false },
                    { 81, 140, false },
                    { 81, 146, false },
                    { 82, 147, false },
                    { 83, 148, false },
                    { 84, 149, false },
                    { 85, 150, false }
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
                name: "IX_ActualDeliveries_RentalId",
                table: "ActualDeliveries",
                column: "RentalId");

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
                name: "IX_GroupSchedules_ActivityTypeGroupId",
                table: "GroupSchedules",
                column: "ActivityTypeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSchedules_RentalId",
                table: "GroupSchedules",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_AccountId",
                table: "PaymentTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceQuotes_ManagerId",
                table: "PriceQuotes",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceQuotes_RentalId",
                table: "PriceQuotes",
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
                name: "IX_RentalDetails_RobotAbilityId",
                table: "RentalDetails",
                column: "RobotAbilityId");

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
                name: "IX_RobotAbilities_RoboTypeId",
                table: "RobotAbilities",
                column: "RoboTypeId");

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
                name: "ActualDeliveries");

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
                name: "ContractReports");

            migrationBuilder.DropTable(
                name: "CustomerContracts");

            migrationBuilder.DropTable(
                name: "DraftApprovals");

            migrationBuilder.DropTable(
                name: "GroupSchedules");

            migrationBuilder.DropTable(
                name: "PriceQuotes");

            migrationBuilder.DropTable(
                name: "RentalContracts");

            migrationBuilder.DropTable(
                name: "RentalDetails");

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
                name: "DraftClauses");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "RobotAbilities");

            migrationBuilder.DropTable(
                name: "ActivityTypeGroups");

            migrationBuilder.DropTable(
                name: "Robots");

            migrationBuilder.DropTable(
                name: "ContractDrafts");

            migrationBuilder.DropTable(
                name: "TemplateClauses");

            migrationBuilder.DropTable(
                name: "RoboTypes");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "ContractTemplates");

            migrationBuilder.DropTable(
                name: "ActivityTypes");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "EventActivities");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
