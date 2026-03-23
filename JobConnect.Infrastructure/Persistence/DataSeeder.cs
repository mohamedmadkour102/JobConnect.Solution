using JobConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobConnect.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAdmin(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("admin"))
            await roleManager.CreateAsync(new IdentityRole("admin"));

        const string adminEmail = "admin@jobconnect.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var admin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@12345");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "admin");
        }
    }

    public static async Task SeedJobs(AppDbContext context, UserManager<User> userManager)
    {
        if (await context.Jobs.AnyAsync())
            return;

        const string employerEmail = "employer@jobconnect.com";
        var employer = await userManager.FindByEmailAsync(employerEmail);
        if (employer == null)
        {
            employer = new Employer
            {
                UserName = employerEmail,
                Email = employerEmail,
                EmailConfirmed = true,
                CompanyName = "Default Company"
            };
            await userManager.CreateAsync(employer, "Employer@12345");
        }

        var job = new Job
        {
            Title = "Default Job",
            Description = "This is a default job for seeding purposes.",
            EmployerId = employer.Id,
            PostedDate = DateTime.UtcNow,
            Status = "Active",
            JobType = "Full-Time",
            Location = "Remote",
            MinSalary = 50000,
            MaxSalary = 80000,
            SalaryType = "Annual",
            Education = "Bachelor's Degree",
            Experience = "2-5 years",
            Vacancies = 3,
            ExpirationDate = DateTime.UtcNow.AddDays(30)
        };

        await context.Jobs.AddAsync(job);
        await context.SaveChangesAsync();
    }

    public static async Task SeedJobTags(AppDbContext context)
    {
        if (await context.JobTags.AnyAsync())
            return;

        var job = await context.Jobs.FirstOrDefaultAsync();
        if (job == null)
        {
            const string employerEmail = "employer@jobconnect.com";
            var employer = await context.Users.OfType<Employer>().FirstOrDefaultAsync(e => e.Email == employerEmail);
            if (employer == null)
            {
                employer = new Employer
                {
                    UserName = employerEmail,
                    Email = employerEmail,
                    EmailConfirmed = true,
                    CompanyName = "Default Company"
                };
                await context.Users.AddAsync(employer);
                await context.SaveChangesAsync();
            }

            job = new Job
            {
                Title = "Default Job",
                Description = "This is a default job for seeding purposes.",
                EmployerId = employer.Id,
                PostedDate = DateTime.UtcNow,
                Status = "Active",
                JobType = "Full-Time",
                Location = "Remote",
                MinSalary = 50000,
                MaxSalary = 80000,
                SalaryType = "Annual",
                Education = "Bachelor's Degree",
                Experience = "2-5 years",
                Vacancies = 3,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            await context.Jobs.AddAsync(job);
            await context.SaveChangesAsync();
        }

        var jobId = job.Id;
        var tags = new List<string>
        {
            "Software Engineering", "Front End Development", "Back End Development", "UI/UX", "Data Structure",
            "Design Patterns", "Algorithms", "Mobile App Development", "DevOps", "Database Design",
            "Machine Learning", "Web Security", "System Design", "API Development", "Version Control (Git)"
        };

        var jobTags = tags.Select(tag => new JobTag { JobId = jobId, Tag = tag }).ToList();
        await context.JobTags.AddRangeAsync(jobTags);
        await context.SaveChangesAsync();
    }
}
