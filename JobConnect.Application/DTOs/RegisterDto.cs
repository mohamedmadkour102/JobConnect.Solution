using System.ComponentModel.DataAnnotations;

namespace JobConnect.Application.DTOs
{
	public class RegisterDto
	{

		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }

		[Required]
		[Phone]
		public string PhoneNumber { get; set; }

		[Required]
		[RegularExpression("(?=^.{6,10}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%&amp;*()_+}]).*$",
			ErrorMessage = "Password must contains 1 Uppercase, 1 Lowercase, 1 Digit, 1 Special Character")]
		public string Password { get; set; }
	}
}
