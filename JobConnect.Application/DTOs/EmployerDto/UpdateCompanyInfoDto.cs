namespace JobConnect.Application.DTOs.EmployerDto
{
	public class UpdateCompanyInfoDto
	{
	
		public string CompanyName { get; set; }
		public string CompanyDescription { get; set; }
		public IFormFile? Logo { get; set; } 
	}
}
