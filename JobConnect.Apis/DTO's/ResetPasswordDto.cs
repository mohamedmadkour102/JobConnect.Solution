using System.ComponentModel.DataAnnotations;

namespace JobConnect.Apis.DTO_s
{
	public class ResetPasswordDto
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Token is required.")]
		public string Token { get; set; }

		[Required(ErrorMessage = "New password is required.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$",
			ErrorMessage = "Password must be at least 6 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
		public string NewPassword { get; set; }
	}
}

