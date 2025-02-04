using System.ComponentModel.DataAnnotations;

namespace JobConnect.Apis.DTO_s
{
	public class EmployerRegistrationDto
	{
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string PhoneNumber { get; set; }
		[Required]
		public string Password { get; set; }

		// Employer-specific properties
		public string CompanyName { get; set; }
		public string CompanySize { get; set; }
		public string Website { get; set; }
		public string Industry { get; set; }
		public string Address { get; set; }
		public string CompanyDescription { get; set; }
	}
}
