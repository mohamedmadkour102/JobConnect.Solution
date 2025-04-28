using JobConnect.Core.Models;

namespace JobConnect.Apis.Services
{
	public interface IJwtTokenService
	{
		Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user);
		Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
	}
}
