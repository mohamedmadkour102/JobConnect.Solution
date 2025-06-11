//using JobConnect.Core.Models;
//using JobConnect.Apis.Models;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using System.Reflection;


//namespace JobConnect.Repository.Data
//{
//	public class AppDbContext : IdentityDbContext<User>
//	{
//		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//		public DbSet<JobSeeker> JobSeekers { get; set; }
//		public DbSet<Employer> Employers { get; set; }
//		public DbSet<Job> Jobs { get; set; }
//		public DbSet<Application> Applications { get; set; }
//		public DbSet<SavedJob> SavedJobs { get; set; }
//		public DbSet<JobSeekerResume> JobSeekerResumes { get; set; }

//		public DbSet<JobTag> JobTags { get; set; }
//		public DbSet<JobResponsibility> JobResponsibilities { get; set; }
//		public DbSet<ContactMessage> ContactMessages { get; set; }
//		protected override void OnModelCreating(ModelBuilder modelBuilder)
//		{
//			base.OnModelCreating(modelBuilder);

//			modelBuilder.Entity<Employer>().ToTable("Employers");
//			modelBuilder.Entity<JobSeeker>().ToTable("JobSeekers");

//			// One-to-Many: Employer -> Jobs (with Restrict to avoid cascade issues)
//			modelBuilder.Entity<Job>()
//				.HasOne(j => j.Employer)
//				.WithMany(e => e.Jobs)
//				.HasForeignKey(j => j.EmployerId)
//				.OnDelete(DeleteBehavior.Restrict);

//			// Many-to-Many: JobSeeker <-> Job (for saved jobs)
//			modelBuilder.Entity<SavedJob>()
//				.HasKey(sj => sj.Id);

//			modelBuilder.Entity<SavedJob>()
//				.HasOne(sj => sj.JobSeeker)
//				.WithMany(js => js.SavedJobs)
//				.HasForeignKey(sj => sj.JobSeekerId)
//				.OnDelete(DeleteBehavior.Cascade);

//			modelBuilder.Entity<SavedJob>()
//				.HasOne(sj => sj.Job)
//				.WithMany(j => j.SavedJobs)
//				.HasForeignKey(sj => sj.JobId)
//				.OnDelete(DeleteBehavior.Cascade);

//			// One-to-Many: JobSeeker -> Resumes
//			modelBuilder.Entity<JobSeekerResume>()
//				.HasOne(jr => jr.JobSeeker)
//				.WithMany(js => js.Resumes)
//				.HasForeignKey(jr => jr.JobSeekerId)
//				.OnDelete(DeleteBehavior.Cascade);


//			modelBuilder.Entity<Application>()
//	.HasOne(a => a.Job)
//	.WithMany(j => j.Applications)
//	.HasForeignKey(a => a.JobId)
//	.OnDelete(DeleteBehavior.Cascade);


//			// One-to-Many: JobSeeker -> Applications (still with Cascade)
//			modelBuilder.Entity<Application>()
//				.HasOne(a => a.JobSeeker)
//				.WithMany(js => js.Applications)
//				.HasForeignKey(a => a.JobSeekerId);

//			// One-to-Many: Job -> Tags
//			modelBuilder.Entity<JobTag>()
//				.HasOne(jt => jt.Job)
//				.WithMany(j => j.Tags)
//				.HasForeignKey(jt => jt.JobId)
//				.OnDelete(DeleteBehavior.Cascade);

//			// One-to-Many: Job -> Responsibilities
//			modelBuilder.Entity<JobResponsibility>()
//				.HasOne(jr => jr.Job)
//				.WithMany(j => j.Responsibilities)
//				.HasForeignKey(jr => jr.JobId)
//				.OnDelete(DeleteBehavior.Cascade)
//				.OnDelete(DeleteBehavior.Cascade);

//			modelBuilder.Entity<Employer>(e =>
//			{
//				e.Property(em => em.CompanyName).IsRequired();
//			});

//			modelBuilder.Entity<JobSeeker>(e =>
//			{
//				e.Property(js => js.Address).IsRequired();
//			});




//			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
//		}
//	}
//}

using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<JobSeekerCertification> JobSeekerCertifications { get; set; }
        public DbSet<JobSeekerCompanyWorkedAt> JobSeekerCompanyWorkedAt { get; set; }
        public DbSet<JobSeekerSkill> JobSeekerSkills { get; set; }
        public DbSet<JobSeekerWorkedAs> JobSeekerWorkedAs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<DeviceToken> DeviceTokens { get; set; }

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
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: JobSeeker -> Resumes
            modelBuilder.Entity<JobSeekerResume>()
                .HasOne(jr => jr.JobSeeker)
                .WithMany(js => js.Resumes)
                .HasForeignKey(jr => jr.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Job -> Applications
            modelBuilder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: JobSeeker -> Applications (now explicitly with Cascade)
            modelBuilder.Entity<Application>()
                .HasOne(a => a.JobSeeker)
                .WithMany(js => js.Applications)
                .HasForeignKey(a => a.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Job -> Tags
            modelBuilder.Entity<JobTag>()
                .HasOne(jt => jt.Job)
                .WithMany(j => j.Tags)
                .HasForeignKey(jt => jt.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Job -> Responsibilities (fixed duplicate OnDelete)
            modelBuilder.Entity<JobResponsibility>()
                .HasOne(jr => jr.Job)
                .WithMany(j => j.Responsibilities)
                .HasForeignKey(jr => jr.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: JobSeeker -> Certifications
            modelBuilder.Entity<JobSeekerCertification>()
                .HasOne(jc => jc.JobSeeker)
                .WithMany(js => js.Certifications)
                .HasForeignKey(jc => jc.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: JobSeeker -> CompanyWorkedAt
            modelBuilder.Entity<JobSeekerCompanyWorkedAt>()
                .HasOne(jc => jc.JobSeeker)
                .WithMany(js => js.CompanyWorkedAt)
                .HasForeignKey(jc => jc.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: JobSeeker -> Skills
            modelBuilder.Entity<JobSeekerSkill>()
                .HasOne(js => js.JobSeeker)
                .WithMany(j => j.Skills)
                .HasForeignKey(js => js.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: JobSeeker -> WorkedAs
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