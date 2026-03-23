using JobConnect.Domain.Entities;

namespace JobConnect.Application.Abstractions;

public interface IJwtTokenService
{
    Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user);
    Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<bool> LogoutAsync(string refreshToken);
}
