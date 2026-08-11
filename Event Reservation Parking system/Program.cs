<<<<<<< HEAD
<<<<<<< Updated upstream
=======
=======
using System.Text;

>>>>>>> origin/master
using EventParkingReservationSystem.Data;
using EventParkingReservationSystem.IRepositories;
using EventParkingReservationSystem.IServices;
using EventParkingReservationSystem.Repositories;
using EventParkingReservationSystem.Services;
<<<<<<< HEAD
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
=======

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
>>>>>>> origin/master

var builder = WebApplication.CreateBuilder(args);

// ==============================
// CONTROLLERS
// ==============================

builder.Services.AddControllers();
<<<<<<< HEAD
builder.Services.AddEndpointsApiExplorer();

=======

builder.Services.AddEndpointsApiExplorer();


>>>>>>> origin/master
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
<<<<<<< HEAD
        });
=======
        }
    );
>>>>>>> origin/master

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
<<<<<<< HEAD
            Description = "Enter the JWT returned by /api/auth/login."
        });
=======
            Description = "Enter JWT token"
        }
    );
>>>>>>> origin/master

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
<<<<<<< HEAD
        });
});

=======
        }
    );
});


>>>>>>> origin/master
// ==============================
// DATABASE
// ==============================

<<<<<<< HEAD
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
=======
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString("DefaultConnection")
        )
);


// ==============================
// MEMBER 1 REPOSITORY
// ==============================

builder.Services.AddScoped<
    ICustomerRepository,
    CustomerRepository>();


// ==============================
// MEMBER 1 SERVICES
// ==============================

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<
    ICustomerService,
    CustomerService>();

>>>>>>> origin/master

// ==============================
// JWT AUTHENTICATION
// ==============================

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
<<<<<<< HEAD
        "Jwt:Key is missing from appsettings.json."
    );

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
=======
        "Jwt:Key is missing in appsettings.json."
    );

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme
    )
>>>>>>> origin/master
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
<<<<<<< HEAD
                    ),

                ClockSkew = TimeSpan.FromMinutes(1)
=======
                    )
>>>>>>> origin/master
            };
    });

builder.Services.AddAuthorization();

<<<<<<< HEAD
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
=======
>>>>>>> origin/master

// ==============================
// BUILD APP
// ==============================

var app = builder.Build();

<<<<<<< HEAD
=======

>>>>>>> origin/master
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

<<<<<<< HEAD
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
=======
>>>>>>> origin/master

// ==============================
// MIDDLEWARE
// ==============================

<<<<<<< HEAD
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

=======
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


// ==============================
// ROUTES
// ==============================

app.MapControllers();


>>>>>>> origin/master
// Root URL -> Swagger
app.MapGet("/", () =>
    Results.Redirect("/swagger")
);

<<<<<<< HEAD
app.Run();
>>>>>>> Stashed changes
=======
app.Run();
>>>>>>> origin/master
