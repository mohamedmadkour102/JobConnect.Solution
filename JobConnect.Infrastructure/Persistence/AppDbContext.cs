using JobConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobConnect.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<JobSeeker> JobSeekers => Set<JobSeeker>();
    public DbSet<Employer> Employers => Set<Employer>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobApplication> Applications => Set<JobApplication>();
    public DbSet<SavedJob> SavedJobs => Set<SavedJob>();
    public DbSet<JobSeekerResume> JobSeekerResumes => Set<JobSeekerResume>();
    public DbSet<JobTag> JobTags => Set<JobTag>();
    public DbSet<JobResponsibility> JobResponsibilities => Set<JobResponsibility>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<JobSeekerCertification> JobSeekerCertifications => Set<JobSeekerCertification>();
    public DbSet<JobSeekerCompanyWorkedAt> JobSeekerCompanyWorkedAt => Set<JobSeekerCompanyWorkedAt>();
    public DbSet<JobSeekerSkill> JobSeekerSkills => Set<JobSeekerSkill>();
    public DbSet<JobSeekerWorkedAs> JobSeekerWorkedAs => Set<JobSeekerWorkedAs>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employer>().ToTable("Employers");
        modelBuilder.Entity<JobSeeker>().ToTable("JobSeekers");

        modelBuilder.Entity<Job>()
            .HasOne(j => j.Employer)
            .WithMany(e => e.Jobs)
            .HasForeignKey(j => j.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SavedJob>().HasKey(sj => sj.Id);
        modelBuilder.Entity<SavedJob>()
            .HasOne(sj => sj.JobSeeker)
            .WithMany(js => js.SavedJobs)
            .HasForeignKey(sj => sj.JobSeekerId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SavedJob>()
            .HasOne(sj => sj.Job)
            .WithMany(j => j.SavedJobs)
            .HasForeignKey(sj => sj.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobSeekerResume>()
            .HasOne(jr => jr.JobSeeker)
            .WithMany(js => js.Resumes)
            .HasForeignKey(jr => jr.JobSeekerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplication>()
            .HasOne(a => a.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<JobApplication>()
            .HasOne(a => a.JobSeeker)
            .WithMany(js => js.Applications)
            .HasForeignKey(a => a.JobSeekerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobTag>()
            .HasOne(jt => jt.Job)
            .WithMany(j => j.Tags)
            .HasForeignKey(jt => jt.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobResponsibility>()
            .HasOne(jr => jr.Job)
            .WithMany(j => j.Responsibilities)
            .HasForeignKey(jr => jr.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobSeekerCertification>()
            .HasOne(jc => jc.JobSeeker)
            .WithMany(js => js.Certifications)
            .HasForeignKey(jc => jc.JobSeekerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobSeekerCompanyWorkedAt>()
            .HasOne(jc => jc.JobSeeker)
            .WithMany(js => js.CompanyWorkedAt)
            .HasForeignKey(jc => jc.JobSeekerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobSeekerSkill>()
            .HasOne(js => js.JobSeeker)
            .WithMany(j => j.Skills)
            .HasForeignKey(js => js.JobSeekerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobSeekerWorkedAs>()
            .HasOne(jw => jw.JobSeeker)
            .WithMany(js => js.WorkedAs)
            .HasForeignKey(jw => jw.JobSeekerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId);

        modelBuilder.Entity<DeviceToken>()
            .HasOne(dt => dt.User)
            .WithMany()
            .HasForeignKey(dt => dt.UserId);

        modelBuilder.Entity<Employer>(e => e.Property(em => em.CompanyName).IsRequired());

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
