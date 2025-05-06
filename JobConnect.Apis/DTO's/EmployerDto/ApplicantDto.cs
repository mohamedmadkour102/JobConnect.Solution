namespace JobConnect.Apis.DTO_s.EmployerDto
{
	public class ApplicantDto
	{
		public string Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string CurrentOrDesiredJob { get; set; } = string.Empty;
		public int? YearsOfExperience { get; set; }
		public string Resume { get; set; } = string.Empty;
		public string CoverLetter { get; set; } = string.Empty;
		public DateTime ApplicationDate { get; set; }
		public bool IsShortlisted { get; set; }
	}
}
