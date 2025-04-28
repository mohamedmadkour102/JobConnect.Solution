using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JobConnect.Core.Models;

namespace JobConnect.Apis.Models
{
	public class Job
	{
		public int Id { get; set; }
		
		public string Description { get; set; } = string.Empty;
		[Column(TypeName = "decimal(18,2)")]
		public decimal MinSalary { get; set; }
		[Column(TypeName = "decimal(18,2)")]
		public decimal MaxSalary { get; set; }
		public string SalaryType { get; set; } = string.Empty;
		public string Education { get; set; } = string.Empty;
		public string Experience { get; set; } = string.Empty;
		public int Vacancies { get; set; }
		public DateTime ExpirationDate { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public int ApplicationCount { get; set; }
		public string JobType { get; set; } = string.Empty;
		public int DaysRemaining { get; set; }
		public DateTime PostedDate { get; set; } = DateTime.UtcNow;
	
		public bool ShortListed { get; set; }
		public string Location { get; set; } = string.Empty;

		// One-to-Many with Employer
		public string EmployerId { get; set; }
		[ForeignKey("EmployerId")]
		public Employer Employer { get; set; }

		// Many-to-Many with JobSeeker (via Applications)
		public ICollection<Application> Applications { get; set; } = new List<Application>();

		// Many-to-Many with JobSeeker (for saved jobs)
		public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();

		// One-to-Many with Tags and Responsibilities
		public ICollection<JobTag> Tags { get; set; } = new List<JobTag>();
		public ICollection<JobResponsibility> Responsibilities { get; set; } = new List<JobResponsibility>();
	}
}