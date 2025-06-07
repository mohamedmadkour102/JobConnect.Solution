using JobConnect.Apis.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Core.Models
{
	[Table("JobSeekers")]
	public class JobSeeker : User
	{
		public string Address { get; set; } = string.Empty;
		public int? YearsOfExperience { get; set; }
		public string Degree { get; set; } = string.Empty;
		public string CurrentOrDesiredJob { get; set; } = string.Empty;
		public string Bio { get; set; } = string.Empty;
		public string CoverLetter { get; set; } = string.Empty;
		
		public DateTime? DateOfBirth { get; set; }
		public string Nationality { get; set; } = string.Empty;
		public string MaritalStatus { get; set; } = string.Empty;
		public string Gender { get; set; } = string.Empty;
		public string Education { get; set; } = string.Empty;
		public string Portfolio { get; set; } = string.Empty;
		public string FacebookLink { get; set; } = string.Empty;
		public string TwitterLink { get; set; } = string.Empty;
		public string InstagramLink { get; set; } = string.Empty;
		public string LinkedInLink { get; set; } = string.Empty;

		public ICollection<Application> Applications { get; set; } = new List<Application>();
		public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
		public ICollection<JobSeekerResume> Resumes { get; set; } = new List<JobSeekerResume>();

		// Location
	}
}