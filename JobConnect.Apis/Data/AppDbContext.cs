using JobConnect;
using JobConnect.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using JobConnect.Apis;

namespace JobConnect.Repository.Data
{
	public class AppDbContext : IdentityDbContext<User>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<JobSeeker> JobSeekers { get; set; }
		public DbSet<Employer> Employers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) 
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Employer>().ToTable("Employers");
			modelBuilder.Entity<JobSeeker>().ToTable("JobSeekers");


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
