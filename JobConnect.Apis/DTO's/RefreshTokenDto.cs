using System.ComponentModel.DataAnnotations;

namespace JobConnect.Apis.DTO_s
{
	public class RefreshTokenDto
	{
		[Required]
		public string AccessToken { get; set; }

		[Required]
		public string RefreshToken { get; set; }
	}
}
