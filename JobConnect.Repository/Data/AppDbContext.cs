using JobConnect.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobConnect.Repository.Data
{
	public class AppDbContext : IdentityDbContext<User>
	{
		public AppDbContext(DbContextOptions options) : base(options) { }

		public DbSet<JobSeeker> JobSeekers { get; set; }
		public DbSet<Employer> Employers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


			modelBuilder.Entity<Employer>().ToTable("Employers");
			modelBuilder.Entity<JobSeeker>().ToTable("JobSeekers");

			// تكوين الخصائص الإضافية لكل جدول
			modelBuilder.Entity<Employer>(e =>
			{
				e.Property(em => em.CompanyName).IsRequired();
				// إضافة باقي الخصائص...
			});

			modelBuilder.Entity<JobSeeker>(e =>
			{
				e.Property(js => js.Address).IsRequired();
				// إضافة باقي الخصائص...
			});

			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}
