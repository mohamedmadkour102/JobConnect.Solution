namespace JobConnect.Application.DTOs.SeekerDto
{
	public class EmployerInfo
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string CompanyName { get; set; }
		public string CompanySize { get; set; }
		public DateTime? FoundingDate { get; set; }
		public string Industry { get; set; }
		public string LogoBase64 { get; set; }
	}
}
