namespace JobConnect.Apis.DTO_s.EmployerDto
{
	public class ShortlistedJobSeekerDto
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string CurrentOrDesiredJob { get; set; }
		public int? YearsOfExperience { get; set; }
		public string Resume { get; set; } // Path to the resume from the Application
		public string CoverLetter { get; set; } // Cover letter from the Application
		public DateTime ApplicationDate { get; set; } // Date of application
	}
}
