using System.ComponentModel.DataAnnotations.Schema;
using JobConnect.Core.Models;

namespace JobConnect.Apis.Models
{
	public class JobSeekerJob
	{
		public string JobSeekerId { get; set; }
		[ForeignKey("JobSeekerId")]
		public JobSeeker JobSeeker { get; set; }

		public int JobId { get; set; }
		[ForeignKey("JobId")]
		public Job Job { get; set; }

		public bool IsSaved { get; set; }
	}
}