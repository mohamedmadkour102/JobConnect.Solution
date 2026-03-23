namespace JobConnect.Application.DTOs.SeekerDto
{
	public class ApplyForJobDto
	{
		public int JobId { get; set; }
		public string CoverLetter { get; set; } = string.Empty;
		public IFormFile Resume { get; set; } // For uploading a new resume
		public string? SelectedResumePath { get; set; } = string.Empty; // For selecting an existing resume
	}
}
