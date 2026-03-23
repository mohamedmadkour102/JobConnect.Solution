using System.ComponentModel.DataAnnotations;

namespace JobConnect.Application.DTOs.EmployerDto
{
	public class ChangePasswordDto
	{
		[Required]
		public string CurrentPassword { get; set; }
		[Required]
		public string NewPassword { get; set; }
		[Required]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
