using JobConnect.Apis.DTO_s;
using JobConnect.Core.Models;
using JobConnect.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JobConnect.Apis.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly ITokenServices _tokenServices;
		private readonly IEmailService _emailService;

		public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenServices tokenServices, IEmailService emailService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenServices = tokenServices;
			_emailService = emailService;
		}

		[HttpPost("Register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			var user = new User()
			{
				FirstName = registerDto.FirstName,
				LastName = registerDto.LastName,
				Email = registerDto.Email,
				PhoneNumber = registerDto.PhoneNumber,
				UserName = registerDto.Email.Split('@')[0]
			};

			var result = await _userManager.CreateAsync(user, registerDto.Password);
			if (!result.Succeeded) return BadRequest(result);

			var returnedUser = new UserDto()
			{
				Name = $"{registerDto.FirstName} {registerDto.LastName}",
				Email = registerDto.Email,
				Token = await _tokenServices.CreateTokenAsync(user)
			};

			return Ok(returnedUser);
		}

		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var user = await _userManager.FindByEmailAsync(loginDto.Email);
			if (user == null) return Unauthorized();

			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
			if (!result.Succeeded) return Unauthorized();

			var returnedUser = new UserDto()
			{
				Name = user.UserName,
				Email = user.Email,
				Token = await _tokenServices.CreateTokenAsync(user)
			};

			return Ok(returnedUser);
		}

		[HttpPost("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return BadRequest("Email not found");

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			// تحديد الـ URL هنا مباشرة في الكود
			var resetLink = $"https://yourapp.com/reset-password?email={user.Email}&token={token}";

			// Send email with the reset link
			await _emailService.SendEmailAsync(user.Email, "Password Reset", $"Click the link to reset your password: {resetLink}");

			return Ok("Password reset email sent.");
		}

		// API to handle reset password
		[HttpPost("ResetPassword")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return BadRequest("Invalid email address");

			var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
			if (!result.Succeeded)
				return BadRequest("Password reset failed");

			return Ok("Password has been reset successfully");
		}
	}
}
