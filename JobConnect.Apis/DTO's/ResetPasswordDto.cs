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
		[RegularExpression(
						@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%&*()_+\}}])[A-Za-z\d!@#$%&*()_+\}}]{6,}$",
						ErrorMessage = "Password must be at least 6 characters long and include: " +
													"1 uppercase letter, 1 lowercase letter, 1 number, and " +
													"1 special character (!@#$%&*()_+}})")]
		public string NewPassword { get; set; }
	}
}

