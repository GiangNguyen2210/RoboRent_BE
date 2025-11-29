using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoboRent_BE.Controller.Hubs;
using RoboRent_BE.Controller.Helpers;  
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Model.Mapping;
using RoboRent_BE.Repository;
using RoboRent_BE.Service;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//variable for google auth
var googleClientId = builder.Configuration["GoogleLoginWeb:Client_id"];
var googleClientSecret = builder.Configuration["GoogleLoginWeb:Client_secret"];

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RoboRent API",
        Version = "v1",
        Description = "RoboRent Backend API with Google OAuth and JWT Authentication"
    });

    // Add JWT Bearer authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers();

builder.Services.AddRepositories().AddServices();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddHttpClient();

builder.Services.AddScoped<ChatNotificationHelper>();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ModifyIdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        // Allow invalid cookies to be rejected silently rather than throwing exceptions
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var secret = jwtSection["Secret"] ?? string.Empty;
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
            ValidIssuer = issuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(audience),
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    })
    .AddGoogle("Google", options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.Scope.Add("openid");
        options.Scope.Add("email");
        options.Scope.Add("profile");
        options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
        options.ClaimActions.MapJsonKey("picture", "picture");
        options.SignInScheme = IdentityConstants.ExternalScheme;
        // Handle remote authentication failures
        options.Events.OnRemoteFailure = async context =>
        {
            // Clear any stale cookies
            await context.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Build absolute URL for error endpoint
            var request = context.Request;
            var error = context.Failure?.Message ?? "Authentication failed";
            var errorUrl = $"{request.Scheme}://{request.Host}/api/Auth/auth-error?error={Uri.EscapeDataString(error)}";
            context.Response.Redirect(errorUrl);
            context.HandleResponse();
        };
        // Handle access denied
        options.Events.OnAccessDenied = async context =>
        {
            await context.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var request = context.Request;
            var errorUrl = $"{request.Scheme}://{request.Host}/api/Auth/auth-error?error={Uri.EscapeDataString("Access denied")}";
            context.Response.Redirect(errorUrl);
            context.HandleResponse();
        };
    });

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000", "https://localhost:3000",  // CRA
            "http://localhost:5173", "https://localhost:5173",  // Vite
            "http://localhost:4200", "https://localhost:4200"   // Angular
        )
        .AllowAnyMethod()      // GET, POST, OPTIONS, etc.
        .AllowAnyHeader()      // Authorization, Content-Type
        .AllowCredentials();   // Cho SignalR cookies/auth
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

// FIX: BỎ CUSTOM OPTIONS MIDDLEWARE - Để CORS handle tự động (nó sẽ set headers đúng)
// app.Use(async (context, next) => { ... });  // Comment out hoặc xóa

app.Run();
