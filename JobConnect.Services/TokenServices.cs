using JobConnect.Core.Models;
using JobConnect.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JobConnect.Services
{
	public class TokenServices : ITokenServices
	{
		private readonly IConfiguration _configuration;

		public TokenServices(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public async Task<string> CreateTokenAsync(User user)
		{
			// PayLoad
			// private claim [user defined]
			var AuthClaims = new List<Claim>()
			{
				new Claim(ClaimTypes.GivenName , user.UserName),
				new Claim(ClaimTypes.Email , user.Email)
			};

			// Key 
			var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

			var Token = new JwtSecurityToken(
                 	issuer: _configuration["JWT:ValidIssure"],
                	audience: _configuration["JWT:ValidAudience"],
                	expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDayes"])),
	                claims: AuthClaims,
                	signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
	              );
			return new JwtSecurityTokenHandler().WriteToken(Token);

		} 
	}
}
