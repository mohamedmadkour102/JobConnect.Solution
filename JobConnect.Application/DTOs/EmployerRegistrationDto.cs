using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace JobConnect.Application.DTOs
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
		[RegularExpression(
						@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%&*()_+\}}])[A-Za-z\d!@#$%&*()_+\}}]{6,}$",
						ErrorMessage = "Password must be at least 6 characters long and include: " +
													"1 uppercase letter, 1 lowercase letter, 1 number, and " +
													"1 special character (!@#$%&*()_+}})")]
		public string Password { get; set; }

        //  companyname , compay size , website , industry , address  , description 

        public string CompanyName { get; set; } 
        public string CompanySize { get; set; } 
        public string Website { get; set; } 
        public string Industry { get; set; } 
        public string Address { get; set; } 
        public string CompanyDescription { get; set; }


    }
}
