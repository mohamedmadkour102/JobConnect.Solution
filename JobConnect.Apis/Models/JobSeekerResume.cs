using JobConnect.Core.Models;

namespace JobConnect.Apis.Models
{
	public class JobSeekerResume
	{
		public int Id { get; set; }
		public string JobSeekerId { get; set; }
		public JobSeeker JobSeeker { get; set; }
		public string? ResumePath { get; set; } = string.Empty;
		public string ResumeName { get; set; } = string.Empty; // Descriptive name for the resume
		public DateTime UploadDate { get; set; } = DateTime.UtcNow;
	}
}
