using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace JobConnect.Repository.Data
{
	public class AppDbContext : IdentityDbContext<User>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<JobSeeker> JobSeekers { get; set; }
		public DbSet<Employer> Employers { get; set; }
		public DbSet<Job> Jobs { get; set; }
		public DbSet<Application> Applications { get; set; }
		public DbSet<SavedJob> SavedJobs { get; set; }
		public DbSet<JobSeekerResume> JobSeekerResumes { get; set; }

		public DbSet<JobTag> JobTags { get; set; }
		public DbSet<JobResponsibility> JobResponsibilities { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Employer>().ToTable("Employers");
			modelBuilder.Entity<JobSeeker>().ToTable("JobSeekers");

			// One-to-Many: Employer -> Jobs (with Restrict to avoid cascade issues)
			modelBuilder.Entity<Job>()
				.HasOne(j => j.Employer)
				.WithMany(e => e.Jobs)
				.HasForeignKey(j => j.EmployerId)
				.OnDelete(DeleteBehavior.Restrict);

			// Many-to-Many: JobSeeker <-> Job (for saved jobs)
			modelBuilder.Entity<SavedJob>()
				.HasKey(sj => sj.Id);

			modelBuilder.Entity<SavedJob>()
				.HasOne(sj => sj.JobSeeker)
				.WithMany(js => js.SavedJobs)
				.HasForeignKey(sj => sj.JobSeekerId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SavedJob>()
				.HasOne(sj => sj.Job)
				.WithMany(j => j.SavedJobs)
				.HasForeignKey(sj => sj.JobId)
				.OnDelete(DeleteBehavior.Restrict);

			// One-to-Many: JobSeeker -> Resumes
			modelBuilder.Entity<JobSeekerResume>()
				.HasOne(jr => jr.JobSeeker)
				.WithMany(js => js.Resumes)
				.HasForeignKey(jr => jr.JobSeekerId)
				.OnDelete(DeleteBehavior.Cascade);

			// One-to-Many: Job -> Applications (with Restrict to avoid cascade issues)
			//modelBuilder.Entity<Application>()
			//	.HasOne(a => a.Job)
			//	.WithMany(j => j.Applications)
			//	.HasForeignKey(a => a.JobId)
			//	.OnDelete(DeleteBehavior.Restrict);
			modelBuilder.Entity<Application>()
	.HasOne(a => a.Job)
	.WithMany(j => j.Applications)
	.HasForeignKey(a => a.JobId)
	.OnDelete(DeleteBehavior.Cascade);


			// One-to-Many: JobSeeker -> Applications (still with Cascade)
			modelBuilder.Entity<Application>()
				.HasOne(a => a.JobSeeker)
				.WithMany(js => js.Applications)
				.HasForeignKey(a => a.JobSeekerId);

						  // One-to-Many: Job -> Tags
			modelBuilder.Entity<JobTag>()
				.HasOne(jt => jt.Job)
				.WithMany(j => j.Tags)
				.HasForeignKey(jt => jt.JobId)
				.OnDelete(DeleteBehavior.Cascade);

			// One-to-Many: Job -> Responsibilities
			modelBuilder.Entity<JobResponsibility>()
				.HasOne(jr => jr.Job)
				.WithMany(j => j.Responsibilities)
				.HasForeignKey(jr => jr.JobId)
				.OnDelete(DeleteBehavior.Cascade)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Employer>(e =>
			{
				e.Property(em => em.CompanyName).IsRequired();
			});

			modelBuilder.Entity<JobSeeker>(e =>
			{
				e.Property(js => js.Address).IsRequired();
			});




			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}