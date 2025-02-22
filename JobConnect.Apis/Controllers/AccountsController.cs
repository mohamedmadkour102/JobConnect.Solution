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

		//[HttpPost("Register/Employer")]
		//public async Task<ActionResult<UserDto>> RegisterEmployer(EmployerRegistrationDto registerDto)
		//{

		//	var employer = new Employer()
		//	{
		//		FirstName = registerDto.FirstName,
		//		LastName = registerDto.LastName,
		//		Email = registerDto.Email,
		//		PhoneNumber = registerDto.PhoneNumber,
		//		UserName = registerDto.Email.Split('@')[0],
		//		CompanyName = registerDto.CompanyName,
		//		CompanySize = registerDto.CompanySize,
		//		Website = registerDto.Website,
		//		Industry = registerDto.Industry,
		//		Address = registerDto.Address,
		//		CompanyDescription = registerDto.CompanyDescription
		//	};

		//	var result = await _userManager.CreateAsync(employer, registerDto.Password);
		//	if (!result.Succeeded) return BadRequest(result.Errors);


		//	await _userManager.AddToRoleAsync(employer, "Employer");


		//	return new UserDto
		//	{
		//		Name = $"{employer.FirstName} {employer.LastName}",
		//		Email = employer.Email,
		//		Token = await _tokenServices.CreateTokenAsync(employer),
		//		Role = "Employer"
		//	};
		//}
		[HttpPost("Register/Employer")]
		public async Task<ActionResult<UserDto>> RegisterEmployer(EmployerRegistrationDto registerDto)
		{
			try
			{
				var employer = new Employer
				{
					FirstName = registerDto.FirstName,
					LastName = registerDto.LastName,
					Email = registerDto.Email,
					PhoneNumber = registerDto.PhoneNumber,
					UserName = registerDto.Email.Split('@')[0],
					CompanyName = registerDto.CompanyName,
					CompanySize = registerDto.CompanySize,
					Website = registerDto.Website,
					Industry = registerDto.Industry,
					Address = registerDto.Address,
					CompanyDescription = registerDto.CompanyDescription
				};

				var result = await _userManager.CreateAsync(employer, registerDto.Password);
				if (!result.Succeeded)
					return BadRequest(result.Errors);

				await _userManager.AddToRoleAsync(employer, "Employer");

				var userDto = new UserDto
				{
					Name = $"{employer.FirstName} {employer.LastName}",
					Email = employer.Email,
					Token = await _tokenServices.CreateTokenAsync(employer),
					Role = "Employer"
				};

				return Ok(userDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

		[HttpPost("Register/JobSeeker")]
		public async Task<ActionResult<UserDto>> RegisterJobSeeker(JobSeekerRegistrationDto registerDto)
		{
			
			var jobSeeker = new JobSeeker()
			{
				FirstName = registerDto.FirstName,
				LastName = registerDto.LastName,
				Email = registerDto.Email,
				PhoneNumber = registerDto.PhoneNumber,
				UserName = registerDto.Email.Split('@')[0],
				Address = registerDto.Address,
				YearsOfExperience = registerDto.YearsOfExperience,
				Degree = registerDto.Degree,
				CurrentOrDesiredJob = registerDto.CurrentOrDesiredJob
			};

			var result = await _userManager.CreateAsync(jobSeeker, registerDto.Password);
			if (!result.Succeeded) return BadRequest(result.Errors);

			
			await _userManager.AddToRoleAsync(jobSeeker, "JobSeeker");

			
			return new UserDto
			{
				Name = $"{jobSeeker.FirstName} {jobSeeker.LastName}",
				Email = jobSeeker.Email,
				Token = await _tokenServices.CreateTokenAsync(jobSeeker),
				Role = "JobSeeker"
			};
		}

		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var user = await _userManager.FindByEmailAsync(loginDto.Email);
			if (user == null)
			{
				return Unauthorized(new { message = "Invalid email or password" });
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
			if (!result.Succeeded)
			{
				return Unauthorized(new { message = "Incorrect password" });
			}

			var roles = await _userManager.GetRolesAsync(user);
			var role = roles.FirstOrDefault();

			return Ok(new UserDto
			{
				Name = $"{user.FirstName} {user.LastName}",
				Email = user.Email,
				Token = await _tokenServices.CreateTokenAsync(user),
				Role = role
			});
		}


		[HttpPost("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return BadRequest("Email not found");

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

	
			var resetLink = $"https://yourapp.com/reset-password?email={user.Email}&token={token}";

			
			await _emailService.SendEmailAsync(user.Email, "Password Reset", $"Click the link to reset your password: {resetLink}");

			return Ok("Password reset email sent.");
		}

		
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
