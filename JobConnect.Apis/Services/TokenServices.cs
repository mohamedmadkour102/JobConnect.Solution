using JobConnect.Core.Models;
using JobConnect.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JobConnect.Services
{
	public class TokenServices : ITokenServices
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<User> _userManager;

		public TokenServices(
			IConfiguration configuration,
			UserManager<User> userManager)
		{
			_configuration = configuration;
			_userManager = userManager;
		}

		public async Task<string> CreateTokenAsync(User user)
		{

			var authClaims = new List<Claim>()
			{
				new Claim (ClaimTypes.NameIdentifier , user.Id),
				new Claim(ClaimTypes.GivenName, user.UserName),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim("fullName", $"{user.FirstName} {user.LastName}"),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};

			
			var userRoles = await _userManager.GetRolesAsync(user);
			foreach (var role in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, role));
			}

		
			var authKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"], 
				expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
