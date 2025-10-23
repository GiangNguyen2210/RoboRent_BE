using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoboRent_BE.Model.Entities;
using RoboRent_BE.Repository;
using RoboRent_BE.Service;
using RoboRent_BE.Controller.Hubs;

var builder = WebApplication.CreateBuilder(args);
//variable for google auth

// var googleClientId = builder.Configuration["GoogleLoginWeb:Client_id"];
// var googleClientSecret = builder.Configuration["GoogleLoginWeb:Client_secret"];

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddRepositories().AddServices();

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
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "Google";
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });
    // .AddGoogle("Google", options =>
    // {
    //     options.ClientId = googleClientId;
    //     options.ClientSecret = googleClientSecret;
    //     options.Scope.Add("openid");
    //     options.Scope.Add("email");
    //     options.Scope.Add("profile");
    //     options.SignInScheme = IdentityConstants.ExternalScheme;
    // });

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:4200") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); 
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


app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;  // Trả OK ngay
        return;  // Không forward request (không gọi next())
    }
    await next();  // Tiếp tục pipeline cho method khác (GET/POST)
});

app.Run();
