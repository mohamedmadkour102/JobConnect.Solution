namespace JobConnect.Apis.DTO_s.Admin
{
	public class EmployerDto
	{
		public string Id { get; set; }
		public string CompanyName { get; set; }
		public string Email { get; set; }
		public string Industry { get; set; }
		public string CompanySize { get; set; }
		public string Website { get; set; }
		public string Address { get; set; }
		public string CompanyDescription { get; set; }
		public string LogoUrl { get; set; }
		public DateTime? FoundingDate { get; set; }
		public string PhoneNumber { get; set; }
		public int JobsCount { get; set; }
	}
}
