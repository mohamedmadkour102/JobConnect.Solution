using JobConnect.Application.Abstractions.Persistence;

namespace JobConnect.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IEmployerRepository? _employers;
    private IJobSeekerRepository? _jobSeekers;
    private IAdminRepository? _admin;
    private IApplicationReadRepository? _applications;
    private INotificationRepository? _notifications;
    private IHomeRepository? _home;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IEmployerRepository Employers => _employers ??= new Repositories.EmployerRepository(_context);
    public IJobSeekerRepository JobSeekers => _jobSeekers ??= new Repositories.JobSeekerRepository(_context);
    public IAdminRepository Admin => _admin ??= new Repositories.AdminRepository(_context);
    public IApplicationReadRepository Applications => _applications ??= new Repositories.ApplicationReadRepository(_context);
    public INotificationRepository Notifications => _notifications ??= new Repositories.NotificationRepository(_context);
    public IHomeRepository Home => _home ??= new Repositories.HomeRepository(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public void Dispose() { }
}
