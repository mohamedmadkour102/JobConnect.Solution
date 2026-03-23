using JobConnect.Application;
using JobConnect.Domain.Entities;
using JobConnect.Infrastructure;
using JobConnect.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
    var defaultOrigins = new[]
    {
        "https://job-connect-pink.vercel.app",
        "http://localhost:3000",
        "http://localhost:8081",
        "https://localhost:7231",
        "https://localhost:5173"
    };
    options.AddPolicy("AllowVercel", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
        {
            var allAllowedOrigins = allowedOrigins.Concat(defaultOrigins).Distinct();
            return allAllowedOrigins.Any(o =>
                origin.Equals(o, StringComparison.OrdinalIgnoreCase) ||
                (o.StartsWith("*", StringComparison.Ordinal) &&
                 origin.EndsWith(o[1..], StringComparison.OrdinalIgnoreCase)));
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

        var isDev = app.Environment.IsDevelopment();
        var errorDetails = new Dictionary<string, object?>
        {
            ["Message"] = "Internal server error",
            ["Path"] = exceptionHandlerPathFeature?.Path
        };
        if (isDev && error != null)
        {
            errorDetails["Exception"] = error.Message;
            errorDetails["StackTrace"] = error.StackTrace;
            errorDetails["InnerException"] = error.InnerException?.Message;
        }

        var errorJson = JsonSerializer.Serialize(errorDetails, new JsonSerializerOptions { WriteIndented = true });
        await context.Response.WriteAsync(errorJson);
    });
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobConnect API V1");
    c.RoutePrefix = "Swagger";
});

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowVercel");
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await DataSeeder.SeedAdmin(userManager, roleManager);
        await DataSeeder.SeedJobs(dbContext, userManager);
        await DataSeeder.SeedJobTags(dbContext);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred during database seeding");
    }
}

app.MapControllers();
app.Run();
