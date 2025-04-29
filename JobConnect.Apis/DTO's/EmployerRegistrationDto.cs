using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JobConnect.Apis.DTO_s
{
	public class EmployerRegistrationDto
	{
		[Required]
		[MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
		[MaxLength(15, ErrorMessage = "First name cannot exceed 15 characters.")]
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain letters.")]
		public string FirstName { get; set; }

		[Required]
		[MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
		[MaxLength(15, ErrorMessage = "Last name cannot exceed 15 characters.")]
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters.")]
		public string LastName { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain digits only.")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$",
			ErrorMessage = "Password must be at least 6 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
		public string Password { get; set; }
		[Required]
		[MinLength(4, ErrorMessage = "Address must be at least 4 characters.")]
		public string Address { get; set; }

		public string CompanyName { get; set; }
		public string CompanySize { get; set; }
		public string Website { get; set; }
		public string Industry { get; set; }
		public string CompanyDescription { get; set; }

		
		[Required(ErrorMessage = "User experience is required.")]
		[Range(0, int.MaxValue, ErrorMessage = "User experience must be a positive number.")]
		public int UserExperience { get; set; }

		[Required(ErrorMessage = "Title is required.")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Degree is required.")]
		public string Degree { get; set; }
	}
}
