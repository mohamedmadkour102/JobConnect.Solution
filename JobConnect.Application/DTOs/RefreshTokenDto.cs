using System.ComponentModel.DataAnnotations;

namespace JobConnect.Application.DTOs
{
	public class RefreshTokenDto
	{
		[Required]
		public string AccessToken { get; set; }

		[Required]
		public string RefreshToken { get; set; }
	}
}
