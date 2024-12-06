using JobConnect.Apis.DTO_s;
using JobConnect.Core.Models;
using JobConnect.Core.Services;
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
		private readonly SignInManager<User> _signInManager;
		private readonly ITokenServices _tokenServices;

		public AccountsController(UserManager<User> userManager , SignInManager<User> signInManager 
			, ITokenServices tokenServices
			
			) 
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenServices = tokenServices;
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
				Token = await _tokenServices.CreateTokenAsync(User)
			};
			return Ok(ReturnedUser);
			
		}

		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var User = await _userManager.FindByEmailAsync(loginDto.Email);
			if (User == null) return Unauthorized();
			var Result = await _signInManager.CheckPasswordSignInAsync(User, loginDto.Password ,false);
			if (!Result.Succeeded) return Unauthorized();
			var ReturnedUser = new UserDto()
			{
				Name = User.UserName,
				Email = User.Email,
				Token = await _tokenServices.CreateTokenAsync(User)
			};
			return Ok(ReturnedUser);

		}
 


	}
}
