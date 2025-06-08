using System.ComponentModel.DataAnnotations;

namespace JobConnect.Apis.DTO_s
{
	public class JobSeekerRegistrationDto
	{
		[Required(ErrorMessage = "First name is required.")]
		[MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
		[MaxLength(15, ErrorMessage = "First name must not exceed 15 characters.")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last name is required.")]
		[MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
		[MaxLength(15, ErrorMessage = "Last name must not exceed 15 characters.")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone number is required.")]
		[RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits.")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Password is required.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%&*()_+\}}])[A-Za-z\d!@#$%&*()_+\}}]{6,}$",
            ErrorMessage = "Password must be at least 6 characters long and include: " +
                          "1 uppercase letter, 1 lowercase letter, 1 number, and " +
                          "1 special character (!@#$%&*()_+}})")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Address is required.")]
		[MinLength(4, ErrorMessage = "Address must be at least 4 characters.")]
		public string Address { get; set; }
        // expe , desired job , degree 

        public int YearsOfExperience { get; set; }
        public string  Degree { get; set; }
        public string CurrentOrDesiredJob { get; set; }

    }
}
