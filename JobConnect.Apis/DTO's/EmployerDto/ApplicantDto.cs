namespace JobConnect.Apis.DTO_s.EmployerDto
{
	public class ApplicantDto
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string CurrentOrDesiredJob { get; set; }
		public int? YearsOfExperience { get; set; }
		public string ResumeBase64 { get; set; }
		public string Status { get; set; }
		public string CoverLetter { get; set; }
		public DateTime ApplicationDate { get; set; }
		public bool IsShortlisted { get; set; }
	}
}
