using JobConnect.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


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

		public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string accessToken, string refreshToken)
		{
			// Validate the refresh token
			var refreshPrincipal = GetPrincipalFromToken(refreshToken);
			if (refreshPrincipal == null)
			{
				return (null, null); // Invalid refresh token
			}

			// Check if refresh token is expired
			var refreshExpiryDateUnix = long.Parse(refreshPrincipal.FindFirst(JwtRegisteredClaimNames.Exp).Value);
			var refreshExpiryDate = DateTimeOffset.FromUnixTimeSeconds(refreshExpiryDateUnix).UtcDateTime;
			if (refreshExpiryDate < DateTime.UtcNow)
			{
				return (null, null); // Expired refresh token
			}

			var userId = refreshPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return (null, null); // User not found
			}

			// Verify stored refresh token
			var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "JobConnect", "RefreshToken");
			if (storedRefreshToken != refreshToken)
			{
				return (null, null); // Refresh token doesn't match
			}

			// Validate the access token (just to check if it's expired)
			var accessPrincipal = GetPrincipalFromToken(accessToken);
			if (accessPrincipal != null)
			{
				var accessExpiryDateUnix = long.Parse(accessPrincipal.FindFirst(JwtRegisteredClaimNames.Exp).Value);
				var accessExpiryDate = DateTimeOffset.FromUnixTimeSeconds(accessExpiryDateUnix).UtcDateTime;

				// If Access Token is not expired, return the same tokens
				if (accessExpiryDate >= DateTime.UtcNow)
				{
					return (accessToken, refreshToken);
				}
			}

			// If Access Token is expired or invalid, invalidate the old tokens and generate new ones
			await _userManager.RemoveAuthenticationTokenAsync(user, "JobConnect", "RefreshToken"); // Invalidate old refresh token
			return await GenerateTokensAsync(user); // Generate new tokens
		}
		public async Task<bool> LogoutAsync(string refreshToken)
		{
			// Validate the refresh token
			var refreshPrincipal = GetPrincipalFromToken(refreshToken);
			if (refreshPrincipal == null)
			{
				return false; // Invalid refresh token
			}

			var userId = refreshPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return false; // User not found
			}

			// Verify stored refresh token
			var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "JobConnect", "RefreshToken");
			if (storedRefreshToken != refreshToken)
			{
				return false; // Refresh token doesn't match
			}

			// Invalidate the refresh token by removing it
			await _userManager.RemoveAuthenticationTokenAsync(user, "JobConnect", "RefreshToken");
			return true;
		}
		private ClaimsPrincipal GetPrincipalFromToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
				ValidateLifetime = false, // We will check lifetime manually
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