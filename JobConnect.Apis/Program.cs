using JobConnect.Core.Models;
using JobConnect.Core.Services;
using JobConnect.Repository.Data;
using JobConnect.Apis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JobConnect.Services;
using JobConnect.Apis.IRepository;
using JobConnect.Apis.Repository;
using JobConnect.Apis.IService;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JobConnect.Apis.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // for console logs
builder.Logging.AddDebug();   // for debug output (e.g., in Visual Studio)
builder.Host.ConfigureLogging(logging =>
{
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JobConnect API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] {}
                }
        });
});



builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ITokenServices, TokenServices>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmployerService, EmployerService>();
builder.Services.AddScoped<IEmployerRepository, EmployerRepository>();
builder.Services.AddScoped<IJobSeekerRepository, JobSeekerRepository>();
builder.Services.AddScoped<IJobSeekerService, JobSeekerService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

// builder.Services.AddScoped<IJobRepository, JobRepository>();
// builder.Services.AddScoped<IJobService, JobService>();
#endregion

#region Identity
builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

#endregion

#region EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
#endregion

// CORS Configuration
// CORS Configuration
builder.Services.AddCors(options =>
{
    // Get allowed origins from config (empty array if not configured)
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

    // Default origins that will always be allowed
    var defaultOrigins = new[]
    {
                "https://*.vercel.app",
                "http://localhost:3000",
                "http://localhost:8081",
                "https://localhost:7231",
                "https://localhost:5173"
        };

    options.AddPolicy("AllowVercel", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
    {
        // Combine config and default origins
        var allAllowedOrigins = allowedOrigins.Concat(defaultOrigins).Distinct();

        return allAllowedOrigins.Any(o =>
            origin.Equals(o, StringComparison.OrdinalIgnoreCase) ||
            (o.StartsWith("*") &&
             origin.EndsWith(o.Substring(1), StringComparison.OrdinalIgnoreCase))
    );
    })
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
    });

});

var app = builder.Build();

// Seeding Admin
using (var scope = app.Services.CreateScope())
{
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	await DataSeeder.SeedAdmin(userManager, roleManager);
}



app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        var error = exceptionHandlerPathFeature?.Error;

        logger.LogError(error, "Unhandled exception occurred");

        var errorDetails = new
        {
            Message = "Internal server error",
            Exception = error?.Message,
            StackTrace = error?.StackTrace,
            Path = exceptionHandlerPathFeature?.Path,
            InnerException = error?.InnerException?.Message
        };

        var errorJson = JsonSerializer.Serialize(errorDetails, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await context.Response.WriteAsync(errorJson);
    });
});


app.UseSwagger();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobConnect API V1");
    c.RoutePrefix = "swagger";
});

app.UseStaticFiles();
app.UseHttpsRedirection();
//app.UseCors("AllowFrontend");
app.UseCors("AllowVercel");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();
