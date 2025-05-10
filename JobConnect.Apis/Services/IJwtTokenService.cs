using JobConnect.Core.Models;

namespace JobConnect.Apis.Services
{
	public interface IJwtTokenService
	{
		/// <summary>
		/// Generates a new Access Token and Refresh Token for the specified user.
		/// </summary>
		/// <param name="user">The user for whom the tokens are generated.</param>
		/// <returns>A tuple containing the Access Token and Refresh Token.</returns>
		Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user);

		/// <summary>
		/// Refreshes the Access Token and Refresh Token if the Access Token is expired.
		/// If the Access Token is not expired, returns the same tokens.
		/// </summary>
		/// <param name="accessToken">The current Access Token to validate.</param>
		/// <param name="refreshToken">The current Refresh Token to validate.</param>
		/// <returns>A tuple containing the Access Token and Refresh Token (either new or the same).</returns>
		Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken);
	}
}
