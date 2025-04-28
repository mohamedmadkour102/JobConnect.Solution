using System;
using System.ComponentModel.DataAnnotations.Schema;
using JobConnect.Core.Models;

namespace JobConnect.Apis.Models
{
	public class SavedJob
	{
		public int Id { get; set; }
		public string JobSeekerId { get; set; }
		[ForeignKey("JobSeekerId")]
		public JobSeeker JobSeeker { get; set; }
		public int JobId { get; set; }
		[ForeignKey("JobId")]
		public Job Job { get; set; }
		public DateTime SavedDate { get; set; } = DateTime.UtcNow;
	}
}