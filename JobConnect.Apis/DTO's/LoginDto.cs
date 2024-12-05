using System.ComponentModel.DataAnnotations;

namespace JobConnect.Apis.DTO_s
{
	public class LoginDto
	{
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
