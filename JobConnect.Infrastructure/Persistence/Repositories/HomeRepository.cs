using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobConnect.Infrastructure.Persistence.Repositories;

public class HomeRepository : RepositoryBase<ContactMessage>, IHomeRepository
{
    public HomeRepository(AppDbContext context) : base(context) { }

    public async Task AddContactMessageAsync(ContactMessage message, CancellationToken cancellationToken = default) =>
        await Context.ContactMessages.AddAsync(message, cancellationToken);

    public async Task<List<ContactMessage>> GetContactMessagesOrderedAsync(CancellationToken cancellationToken = default) =>
        await Context.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<List<string>> GetDistinctJobTagsAsync(CancellationToken cancellationToken = default) =>
        await Context.JobTags
            .Select(jt => jt.Tag)
            .Distinct()
            .ToListAsync(cancellationToken);

    public async Task<(List<Job> Jobs, int TotalCount)> GetJobsWithFiltersAsync(
        IReadOnlyDictionary<string, string>? filters,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Jobs
            .Include(j => j.Employer)
            .AsQueryable();

        if (filters != null && filters.Count > 0)
        {
            foreach (var filter in filters)
            {
                var key = filter.Key.ToLowerInvariant();
                var value = filter.Value?.Trim();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                switch (key)
                {
                    case "searchterm":
                    case "title":
                        query = query.Where(j =>
                            j.Title.ToLower().Contains(value.ToLower()) ||
                            j.Location.ToLower().Contains(value.ToLower()));
                        break;
                    case "minsalary":
                        if (decimal.TryParse(value, out var minSalary))
                            query = query.Where(j => j.MinSalary >= minSalary);
                        break;
                    case "maxsalary":
                        if (decimal.TryParse(value, out var maxSalary))
                            query = query.Where(j => j.MaxSalary <= maxSalary);
                        break;
                    case "jobtype":
                        query = query.Where(j => j.JobType.ToLower() == value.ToLower());
                        break;
                    case "educationlevel":
                        query = query.Where(j => j.Education.ToLower() == value.ToLower());
                        break;
                    case "location":
                        query = query.Where(j => j.Location.ToLower().Contains(value.ToLower()));
                        break;
                    case "workplace":
                        query = query.Where(j => j.WorkPlace.ToLower() == value.ToLower());
                        break;
                }
            }
        }

        var jobs = await query.ToListAsync(cancellationToken);

        if (filters != null && filters.TryGetValue("experience", out var expFilterValue))
        {
            expFilterValue = expFilterValue?.Trim();
            if (!string.IsNullOrWhiteSpace(expFilterValue) && !string.Equals(expFilterValue, "all", StringComparison.OrdinalIgnoreCase))
            {
                jobs = jobs.Where(j =>
                {
                    if (!int.TryParse(j.Experience.Split(' ')[0], out var exp))
                        return false;
                    return expFilterValue switch
                    {
                        "0" => exp == 0,
                        "0-2" => exp is >= 0 and <= 2,
                        "2-5" => exp is >= 2 and <= 5,
                        "5-8" => exp is >= 5 and <= 8,
                        "8+" => exp >= 8,
                        _ => true
                    };
                }).ToList();
            }
        }

        var totalCount = jobs.Count;
        var paged = jobs
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (paged, totalCount);
    }

    public async Task<List<Job>> GetJobsByTagAndIdAsync(string tag, int tagId, CancellationToken cancellationToken = default) =>
        await Context.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .Include(j => j.Applications)
            .Where(j => j.Tags.Any(t => t.Tag == tag && t.Id == tagId))
            .ToListAsync(cancellationToken);
}
