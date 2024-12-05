using JobConnect.Apis.DTO_s;
using JobConnect.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobConnect.Apis.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly UserManager<User> _userManager;

		public AccountsController(UserManager<User> userManager ) 
		{
			_userManager = userManager;
		}

		[HttpPost("Register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			var User = new User()
			{
				FirstName = registerDto.FirstName,
				LastName = registerDto.LastName,
				Email = registerDto.Email,
				PhoneNumber = registerDto.PhoneNumber,
				UserName = registerDto.Email.Split('@')[0]
			};
			var result = await _userManager.CreateAsync(User, registerDto.Password);
			if (!result.Succeeded) return BadRequest(result);
			var ReturnedUser = new UserDto() 
			{
				Name = $"{registerDto.FirstName} {registerDto.LastName}",
				Email = registerDto.Email,
				Token = "This is Token"
			};
			return Ok(ReturnedUser);
			}


	}
}
