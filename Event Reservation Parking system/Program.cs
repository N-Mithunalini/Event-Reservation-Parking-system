<<<<<<< Updated upstream
=======
using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Repositories;
using EventParkingReservationSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// CONTROLLERS
// ==============================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ==============================
// SWAGGER
// ==============================

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Event & Parking Reservation System API",
            Version = "v1"
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter the JWT returned by /api/auth/login."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

// ==============================
// DATABASE
// ==============================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// ==============================
// JWT AUTHENTICATION
// ==============================

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key is missing from appsettings.json."
    );

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                ClockSkew = TimeSpan.FromMinutes(1)
            };
    });

builder.Services.AddAuthorization();

// ==============================
// REPOSITORIES
// ==============================

// Member 1
builder.Services.AddScoped<
    ICustomerRepository,
    CustomerRepository>();

// Member 2
builder.Services.AddScoped<
    IVenueRepository,
    VenueRepository>();

builder.Services.AddScoped<
    ICategoryRepository,
    CategoryRepository>();

builder.Services.AddScoped<
    IEventRepository,
    EventRepository>();

// Member 3
builder.Services.AddScoped<
    ISeatRepository,
    SeatRepository>();

builder.Services.AddScoped<
    IParkingRepository,
    ParkingRepository>();

// Member 4
builder.Services.AddScoped<
    IBookingRepository,
    BookingRepository>();

builder.Services.AddScoped<
    IPaymentRepository,
    PaymentRepository>();

builder.Services.AddScoped<
    INotificationRepository,
    NotificationRepository>();

// ==============================
// SERVICES
// ==============================

// Member 1
builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<
    ICustomerService,
    CustomerService>();

// Member 2
builder.Services.AddScoped<
    IVenueService,
    VenueService>();

builder.Services.AddScoped<
    ICategoryService,
    CategoryService>();

builder.Services.AddScoped<
    IEventService,
    EventService>();

// Member 3
builder.Services.AddScoped<
    ISeatService,
    SeatService>();

builder.Services.AddScoped<
    IParkingService,
    ParkingService>();

// Member 4
builder.Services.AddScoped<
    IBookingService,
    BookingService>();

builder.Services.AddScoped<
    IPaymentService,
    PaymentService>();

builder.Services.AddScoped<
    INotificationService,
    NotificationService>();

builder.Services.AddScoped<
    IDashboardService,
    DashboardService>();

builder.Services.AddHostedService<
    BookingExpiryService>();

// ==============================
// BUILD APP
// ==============================

var app = builder.Build();

// ==============================
// SWAGGER
// ==============================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Event & Parking Reservation System API v1"
        );

        options.RoutePrefix = "swagger";
    });
}

// ==============================
// DATABASE MIGRATION + SEED
// ==============================

using (var scope = app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    await db.Database.MigrateAsync();

    await DbSeeder.SeedAsync(db);
}

// ==============================
// MIDDLEWARE
// ==============================

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Root URL -> Swagger
app.MapGet("/", () =>
    Results.Redirect("/swagger")
);

app.Run();
>>>>>>> Stashed changes
