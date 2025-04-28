using JobConnect.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JobConnect.Apis.Services
{

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
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim("fullName", $"{user.FirstName} {user.LastName}"),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var roles = await _userManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var accessToken = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(15), // Short-lived access token
				signingCredentials: creds
			);

			var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);
			var refreshToken = GenerateRefreshToken();

			// Store refresh token in user data
			await _userManager.SetAuthenticationTokenAsync(user, "JobConnect", "RefreshToken", refreshToken);

			return (accessTokenString, refreshToken);
		}

		public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
		{
			var principal = GetPrincipalFromExpiredToken(refreshToken);
			if (principal == null)
			{
				return (null, null);
			}

			var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return (null, null);
			}

			var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "JobConnect", "RefreshToken");
			if (storedRefreshToken != refreshToken)
			{
				return (null, null);
			}

			return await GenerateTokensAsync(user);
		}

		private string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
				ValidateLifetime = false, // Allow expired tokens for refresh
				ValidIssuer = _configuration["Jwt:Issuer"],
				ValidAudience = _configuration["Jwt:Audience"]
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
				if (securityToken is not JwtSecurityToken jwtSecurityToken ||
					!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				{
					return null;
				}

				return principal;
			}
			catch
			{
				return null;
			}
		}
	}
}
