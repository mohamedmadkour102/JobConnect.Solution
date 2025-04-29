using JobConnect.Core.Models;

namespace JobConnect.Apis.Models
{

		public class Application
		{
			public int Id { get; set; }

			public int JobId { get; set; }
			public Job Job { get; set; }
			public string JobSeekerId { get; set; }
			public JobSeeker JobSeeker { get; set; }

			public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
			public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected

		public string CoverLetter { get; set; } = string.Empty;
		public string Resume { get; set; } = string.Empty; // Path to the uploaded PDF file
		public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
		public bool IsShortlisted { get; set; } = false;
	}
	
}


