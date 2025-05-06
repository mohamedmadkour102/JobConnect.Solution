namespace JobConnect.Apis.DTO_s.Admin
{
	public class JobSeekerDto
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public int? YearsOfExperience { get; set; }
		public string Degree { get; set; }
		public string CurrentOrDesiredJob { get; set; }
		public string Bio { get; set; }
		public string CoverLetter { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string Nationality { get; set; }
		public string MaritalStatus { get; set; }
		public string Gender { get; set; }
		public string Education { get; set; }
		public string Portfolio { get; set; }
		public string FacebookLink { get; set; }
		public string TwitterLink { get; set; }
		public string InstagramLink { get; set; }
		public string LinkedInLink { get; set; }
		public int ApplicationsCount { get; set; }
		public int SavedJobsCount { get; set; }
	}
}
