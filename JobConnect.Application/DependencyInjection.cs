using JobConnect.Application.Abstractions;
using JobConnect.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobConnect.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployerService, EmployerService>();
        services.AddScoped<IJobSeekerService, JobSeekerService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IHomeService, HomeService>();
        return services;
    }
}
