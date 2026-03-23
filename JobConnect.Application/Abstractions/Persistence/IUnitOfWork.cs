namespace JobConnect.Application.Abstractions.Persistence;

public interface IUnitOfWork : IDisposable
{
    IEmployerRepository Employers { get; }
    IJobSeekerRepository JobSeekers { get; }
    IAdminRepository Admin { get; }
    IApplicationReadRepository Applications { get; }
    INotificationRepository Notifications { get; }
    IHomeRepository Home { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
