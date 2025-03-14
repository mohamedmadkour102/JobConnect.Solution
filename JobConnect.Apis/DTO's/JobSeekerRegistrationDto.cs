using System.ComponentModel.DataAnnotations;

namespace JobConnect.Apis.DTO_s
{
	public class JobSeekerRegistrationDto
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

		
		public string Address { get; set; }
		public int? YearsOfExperience { get; set; }
		public string Degree { get; set; }
		public string CurrentOrDesiredJob { get; set; }
	}
}
