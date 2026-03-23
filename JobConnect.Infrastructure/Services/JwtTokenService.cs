using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobConnect.Application.Abstractions;
using JobConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JobConnect.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public JwtTokenService(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
    {
        var accessToken = await GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user);
        await _userManager.SetAuthenticationTokenAsync(user, "JobConnect", "RefreshToken", refreshToken);
        return (accessToken, refreshToken);
    }

    private async Task<string> GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new("fullName", $"{user.FirstName} {user.LastName}"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in await _userManager.GetRolesAsync(user))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var minutes = double.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "30");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var days = double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(days),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var refreshPrincipal = GetPrincipalFromToken(refreshToken);
        if (refreshPrincipal == null)
            return (null!, null!);

        var refreshExpiryDateUnix = long.Parse(refreshPrincipal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value ?? "0");
        if (DateTimeOffset.FromUnixTimeSeconds(refreshExpiryDateUnix).UtcDateTime < DateTime.UtcNow)
            return (null!, null!);

        var userId = refreshPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return (null!, null!);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (null!, null!);

        var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "JobConnect", "RefreshToken");
        if (storedRefreshToken != refreshToken)
            return (null!, null!);

        var accessPrincipal = GetPrincipalFromToken(accessToken);
        if (accessPrincipal != null)
        {
            var accessExpiryDateUnix = long.Parse(accessPrincipal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value ?? "0");
            if (DateTimeOffset.FromUnixTimeSeconds(accessExpiryDateUnix).UtcDateTime >= DateTime.UtcNow)
                return (accessToken, refreshToken);
        }

        await _userManager.RemoveAuthenticationTokenAsync(user, "JobConnect", "RefreshToken");
        return await GenerateTokensAsync(user);
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var refreshPrincipal = GetPrincipalFromToken(refreshToken);
        if (refreshPrincipal == null)
            return false;

        var userId = refreshPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
            return false;

        var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "JobConnect", "RefreshToken");
        if (storedRefreshToken != refreshToken)
            return false;

        await _userManager.RemoveAuthenticationTokenAsync(user, "JobConnect", "RefreshToken");
        return true;
    }

    private ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            ValidateLifetime = false,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
