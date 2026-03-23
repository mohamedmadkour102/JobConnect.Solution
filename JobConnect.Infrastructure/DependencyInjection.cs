using JobConnect.Application.Abstractions;
using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Application.Configuration;
using JobConnect.Infrastructure.Configuration;
using JobConnect.Infrastructure.Persistence;
using JobConnect.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobConnect.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITokenServices, TokenServices>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<JobMatchingService>();
        services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));

        services.AddHttpClient();

        return services;
    }
}
