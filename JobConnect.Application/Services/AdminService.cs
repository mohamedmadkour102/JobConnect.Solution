using JobConnect.Application.Abstractions;
using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Application.DTOs.Admin;
using JobConnect.Domain.Entities;

namespace JobConnect.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _uow;

    public AdminService(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task<IEnumerable<EmployerDto>> GetAllEmployersAsync()
    {
        var employers = await _uow.Admin.GetAllEmployersAsync();
        return employers.Select(e => new EmployerDto
        {
            Id = e.Id,
            CompanyName = e.CompanyName,
            Email = e.Email,
            Industry = e.Industry,
            CompanySize = e.CompanySize,
            Website = e.Website,
            Address = e.Address,
            CompanyDescription = e.CompanyDescription,
            LogoUrl = e.LogoUrl,
            FoundingDate = e.FoundingDate,
            PhoneNumber = e.PhoneNumber,
            JobsCount = e.Jobs.Count
        });
    }

    public async Task<IEnumerable<JobSeekerDto>> GetAllJobSeekersAsync()
    {
        var jobSeekers = await _uow.Admin.GetAllJobSeekersAsync();
        return jobSeekers.Select(js => new JobSeekerDto
        {
            Id = js.Id,
            FirstName = js.FirstName,
            LastName = js.LastName,
            Email = js.Email,
            Address = js.Address,
            YearsOfExperience = js.YearsOfExperience,
            Degree = js.Degree,
            CurrentOrDesiredJob = js.CurrentOrDesiredJob,
            Bio = js.Bio,
            CoverLetter = js.CoverLetter,
            DateOfBirth = js.DateOfBirth,
            Nationality = js.Nationality,
            MaritalStatus = js.MaritalStatus,
            Gender = js.Gender,
            Education = js.Education,
            Portfolio = js.Portfolio,
            FacebookLink = js.FacebookLink,
            TwitterLink = js.TwitterLink,
            InstagramLink = js.InstagramLink,
            LinkedInLink = js.LinkedInLink,
            ApplicationsCount = js.Applications.Count,
            SavedJobsCount = js.SavedJobs.Count
        });
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        if (await _uow.Admin.HasActiveEmployerJobsAsync(userId))
            throw new InvalidOperationException("Cannot delete employer with active jobs.");

        var ok = await _uow.Admin.DeleteUserAsync(userId);
        if (ok)
            await _uow.SaveChangesAsync();
        return ok;
    }

    public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
    {
        var jobs = await _uow.Admin.GetAllJobsAsync();
        return jobs.Select(j => new JobDto
        {
            Id = j.Id,
            Title = j.Title,
            Status = j.Status,
            ApplicationsCount = j.Applications.Count,
            JobType = j.JobType,
            DaysRemaining = CalculateDaysRemaining(j.ExpirationDate),
            PostedDate = GetTimeAgo(j.PostedDate),
            Location = j.Location,
            Tags = j.Tags.Select(t => t.Tag).ToList(),
            Responsibilities = j.Responsibilities.Select(r => r.Responsibility).ToList(),
            EmployerName = j.Employer!.CompanyName
        });
    }

    public async Task<IEnumerable<JobDto>> GetJobsByTagAsync(string tag)
    {
        var jobs = await _uow.Admin.GetJobsByTagAsync(tag);
        return jobs.Select(j => new JobDto
        {
            Id = j.Id,
            Title = j.Title,
            Status = j.Status,
            ApplicationsCount = j.Applications.Count,
            JobType = j.JobType,
            DaysRemaining = CalculateDaysRemaining(j.ExpirationDate),
            PostedDate = GetTimeAgo(j.PostedDate),
            Location = j.Location,
            Tags = j.Tags.Select(t => t.Tag).ToList(),
            Responsibilities = j.Responsibilities.Select(r => r.Responsibility).ToList(),
            EmployerName = j.Employer!.CompanyName
        });
    }

    public async Task<JobApplication?> UpdateApplicationStatusAsync(string applicationId, string status)
    {
        var validStatuses = new[] { "Pending", "Accepted", "Rejected" };
        if (!validStatuses.Contains(status))
            throw new InvalidOperationException($"Invalid status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}.");
        if (!int.TryParse(applicationId, out var parsedApplicationId))
            throw new ArgumentException($"Invalid applicationId: {applicationId}. It must be a valid integer.");

        var application = await _uow.Applications.GetByIdWithJobSeekerAsync(parsedApplicationId);
        if (application == null)
            return null;

        application.Status = status;
        await _uow.SaveChangesAsync();

        return application;
    }

    private static string GetTimeAgo(DateTime date)
    {
        var timeSpan = DateTime.UtcNow - date;
        if (timeSpan.TotalDays >= 7)
            return $"{(int)timeSpan.TotalDays / 7} week ago";
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} days ago";
        return "Today";
    }

    private static int CalculateDaysRemaining(DateTime expirationDate)
    {
        var daysRemaining = (expirationDate - DateTime.UtcNow).Days;
        return daysRemaining > 0 ? daysRemaining : 0;
    }
}
