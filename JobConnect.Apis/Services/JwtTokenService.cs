using JobConnect.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
			// Generate Access Token
			var accessToken = await GenerateAccessToken(user);

			// Generate Refresh Token (as JWT)
			var refreshToken = GenerateRefreshToken(user);

			// Store refresh token in user data
			await _userManager.SetAuthenticationTokenAsync(user, "JobConnect", "RefreshToken", refreshToken);

			return (accessToken, refreshToken);
		}

		private async Task<string> GenerateAccessToken(User user)
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

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"])),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private string GenerateRefreshToken(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationDays"])),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
		{
			// Validate the refresh token
			var principal = GetPrincipalFromToken(refreshToken);
			if (principal == null)
			{
				return (null, null); // Invalid token
			}

			// Check if token is expired
			var expiryDateUnix = long.Parse(principal.FindFirst(JwtRegisteredClaimNames.Exp).Value);
			var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix).UtcDateTime;
			if (expiryDate < DateTime.UtcNow)
			{
				return (null, null); // Expired token
			}

			var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return (null, null); // User not found
			}

			// Verify stored refresh token
			var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "JobConnect", "RefreshToken");
			if (storedRefreshToken != refreshToken)
			{
				return (null, null); // Token doesn't match
			}

			// Generate new tokens
			return await GenerateTokensAsync(user);
		}

		private ClaimsPrincipal GetPrincipalFromToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
				ValidateLifetime = true, // Validate token expiration
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
