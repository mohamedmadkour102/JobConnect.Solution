using System.ComponentModel.DataAnnotations;

namespace JobConnect.Apis.DTO_s
{
	public class ForgotPasswordDto
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }
	}
}

