#region OldCode
//using JobConnect.Apis.DTO_s;
//using JobConnect.Core.Models;
//using JobConnect.Core.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.Data;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;

//namespace JobConnect.Apis.Controllers
//{
//	[Route("api/[controller]")]
//	[ApiController]
//	public class AccountsController : ControllerBase
//	{
//		private readonly UserManager<User> _userManager;
//		private readonly SignInManager<User> _signInManager;
//		private readonly ITokenServices _tokenServices;
//		private readonly IEmailService _emailService;

//		public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenServices tokenServices, IEmailService emailService)
//		{
//			_userManager = userManager;
//			_signInManager = signInManager;
//			_tokenServices = tokenServices;
//			_emailService = emailService;
//		}


//		[HttpPost("Register/Employer")]
//		public async Task<ActionResult<UserDto>> RegisterEmployer(EmployerRegistrationDto registerDto)
//		{
//			try
//			{
//				var employer = new Employer
//				{
//					FirstName = registerDto.FirstName,
//					LastName = registerDto.LastName,
//					Email = registerDto.Email,
//					PhoneNumber = registerDto.PhoneNumber,
//					UserName = registerDto.Email.Split('@')[0],
//					CompanyName = registerDto.CompanyName,
//					CompanySize = registerDto.CompanySize,
//					Website = registerDto.Website,
//					Industry = registerDto.Industry,
//					Address = registerDto.Address,
//					CompanyDescription = registerDto.CompanyDescription
//				};

//				var result = await _userManager.CreateAsync(employer, registerDto.Password);
//				if (!result.Succeeded)
//					return BadRequest(result.Errors);

//				await _userManager.AddToRoleAsync(employer, "employer");

//				var userDto = new UserDto
//				{
//					Name = $"{employer.FirstName} {employer.LastName}",
//					Email = employer.Email,
//					Token = await _tokenServices.CreateTokenAsync(employer),
//					Role = "employer"
//				};

//				return Ok(userDto);
//			}
//			catch (Exception ex)
//			{
//				return StatusCode(500, $"An error occurred: {ex.Message}");
//			}
//		}

//		[HttpPost("Register/JobSeeker")]
//		public async Task<ActionResult<UserDto>> RegisterJobSeeker(JobSeekerRegistrationDto registerDto)
//		{

//			var jobSeeker = new JobSeeker()
//			{
//				FirstName = registerDto.FirstName,
//				LastName = registerDto.LastName,
//				Email = registerDto.Email,
//				PhoneNumber = registerDto.PhoneNumber,
//				UserName = registerDto.Email.Split('@')[0],
//				Address = registerDto.Address,
//				YearsOfExperience = registerDto.YearsOfExperience,
//				Degree = registerDto.Degree,
//				CurrentOrDesiredJob = registerDto.CurrentOrDesiredJob
//			};

//			var result = await _userManager.CreateAsync(jobSeeker, registerDto.Password);
//			if (!result.Succeeded) return BadRequest(result.Errors);


//			await _userManager.AddToRoleAsync(jobSeeker, "jobSeeker");


//			return new UserDto
//			{
//				Name = $"{jobSeeker.FirstName} {jobSeeker.LastName}",
//				Email = jobSeeker.Email,
//				Token = await _tokenServices.CreateTokenAsync(jobSeeker),
//				Role = "jobSeeker"
//			};
//		}

//		[HttpPost("Login")]
//		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
//		{
//			var user = await _userManager.FindByEmailAsync(loginDto.Email);
//			if (user == null)
//			{
//				return Unauthorized(new { message = "Invalid email or password" });
//			}

//			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
//			if (!result.Succeeded)
//			{
//				return Unauthorized(new { message = "Incorrect password" });
//			}

//			var roles = await _userManager.GetRolesAsync(user);
//			var role = roles.FirstOrDefault();

//			return Ok(new UserDto
//			{
//				Name = $"{user.FirstName} {user.LastName}",
//				Email = user.Email,
//				Token = await _tokenServices.CreateTokenAsync(user),
//				Role = role
//			});
//		}


//		[HttpPost("ForgotPassword")]
//		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
//		{
//			var user = await _userManager.FindByEmailAsync(request.Email);
//			if (user == null)
//				return BadRequest("Email not found");

//			var token = await _userManager.GeneratePasswordResetTokenAsync(user);


//			var resetLink = $"https://yourapp.com/reset-password?email={user.Email}&token={token}";


//			await _emailService.SendEmailAsync(user.Email, "Password Reset", $"Click the link to reset your password: {resetLink}");

//			return Ok("Password reset email sent.");
//		}


//		[HttpPost("ResetPassword")]
//		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
//		{
//			var user = await _userManager.FindByEmailAsync(request.Email);
//			if (user == null)
//				return BadRequest("Invalid email address");

//			var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
//			if (!result.Succeeded)
//				return BadRequest("Password reset failed");

//			return Ok("Password has been reset successfully");
//		}
//	}
//} 
#endregion
using JobConnect.Apis.DTO_s;

using JobConnect.Apis.Services;
using JobConnect.Core.Models;
using JobConnect.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JobConnect.Apis.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IJwtTokenService _tokenService;
		private readonly IEmailService _emailService;
		private readonly ILogger<AccountsController> _logger;

		public AccountsController(
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			IJwtTokenService tokenService,
			IEmailService emailService,
			ILogger<AccountsController> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
			_emailService = emailService;
			_logger = logger;
		}

		[HttpPost("Register/Employer")]
		public async Task<IActionResult> RegisterEmployer([FromBody] EmployerRegistrationDto dto)
		{
			try
			{
				_logger.LogInformation("Starting employer registration for email: {Email}", dto.Email);

				var employer = new Employer
				{
					FirstName = dto.FirstName,
					LastName = dto.LastName,
					Email = dto.Email,
					UserName = $"{dto.FirstName}{dto.LastName}",
					PhoneNumber = dto.PhoneNumber,
					CompanyName = dto.CompanyName,
					CompanySize = dto.CompanySize,
					Website = dto.Website,
					Industry = dto.Industry,
					Address = dto.Address,
					CompanyDescription = dto.CompanyDescription
				};

				var result = await _userManager.CreateAsync(employer, dto.Password);
				if (!result.Succeeded)
				{
					_logger.LogWarning("Employer registration failed for {Email}: {Errors}",
						dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
					return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
				}

				await _userManager.AddToRoleAsync(employer, "Employer");
				_logger.LogInformation("Employer registered successfully with ID: {Id}", employer.Id);

				var (token, _) = await _tokenService.GenerateTokensAsync(employer);

				return Ok(new UserDto
				{
					Name = $"{employer.FirstName} {employer.LastName}",
					Email = employer.Email,
					Token = token,
					Role = "Employer"
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during employer registration for {Email}", dto.Email);
				return StatusCode(500, new { Message = "An error occurred during registration." });
			}
		}

		[HttpPost("Register/JobSeeker")]
		public async Task<IActionResult> RegisterJobSeeker([FromBody] JobSeekerRegistrationDto dto)
		{
			try
			{
				_logger.LogInformation("Starting job seeker registration for email: {Email}", dto.Email);

				var jobSeeker = new JobSeeker
				{
					FirstName = dto.FirstName,
					LastName = dto.LastName,
					Email = dto.Email,
					UserName = $"{dto.FirstName}{dto.LastName}",
					PhoneNumber = dto.PhoneNumber,
					Address = dto.Address,
					YearsOfExperience = dto.YearsOfExperience,
					Degree = dto.Degree,
					CurrentOrDesiredJob = dto.CurrentOrDesiredJob
				};

				var result = await _userManager.CreateAsync(jobSeeker, dto.Password);
				if (!result.Succeeded)
				{
					_logger.LogWarning("Job seeker registration failed for {Email}: {Errors}",
						dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
					return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
				}

				await _userManager.AddToRoleAsync(jobSeeker, "JobSeeker");
				_logger.LogInformation("Job seeker registered successfully with ID: {Id}", jobSeeker.Id);

				var (token, _) = await _tokenService.GenerateTokensAsync(jobSeeker);

				return Ok(new UserDto
				{
					Name = $"{jobSeeker.FirstName} {jobSeeker.LastName}",
					Email = jobSeeker.Email,
					Token = token,
					Role = "JobSeeker"
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during job seeker registration for {Email}", dto.Email);
				return StatusCode(500, new { Message = "An error occurred during registration." });
			}
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			try
			{
				_logger.LogInformation("Login attempt for email: {Email}", dto.Email);

				var user = await _userManager.FindByEmailAsync(dto.Email);
				if (user == null)
				{
					_logger.LogWarning("Login failed: User with email {Email} not found", dto.Email);
					return Unauthorized(new { Message = "Invalid email or password." });
				}

				var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
				if (!result.Succeeded)
				{
					_logger.LogWarning("Login failed: Incorrect password for {Email}", dto.Email);
					return Unauthorized(new { Message = "Invalid email or password." });
				}

				var roles = await _userManager.GetRolesAsync(user);
				var role = roles.FirstOrDefault();

				var (token, refreshToken) = await _tokenService.GenerateTokensAsync(user);

				_logger.LogInformation("Login successful for {Email}, Role: {Role}", dto.Email, role);

				return Ok(new
				{
					user = new UserDto
					{
						Name = $"{user.FirstName} {user.LastName}",
						Email = user.Email,
						Token = token,
						Role = role
					},
					RefreshToken = refreshToken
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login for {Email}", dto.Email);
				return StatusCode(500, new { Message = "An error occurred during login." });
			}
		}

		[HttpPost("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
		{
			try
			{
				_logger.LogInformation("Forgot password request for email: {Email}", dto.Email);

				var user = await _userManager.FindByEmailAsync(dto.Email);
				if (user == null)
				{
					_logger.LogWarning("Forgot password: User with email {Email} not found", dto.Email);
					return BadRequest(new { Message = "Email not found." });
				}

				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var resetLink = $"https://yourapp.com/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

				await _emailService.SendEmailAsync(user.Email, "Password Reset Request",
					$"Please reset your password by clicking this link: {resetLink}");

				_logger.LogInformation("Password reset email sent to {Email}", dto.Email);
				return Ok(new { Message = "Password reset email sent." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during forgot password for {Email}", dto.Email);
				return StatusCode(500, new { Message = "An error occurred while processing your request." });
			}
		}

		[HttpPost("ResetPassword")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
		{
			try
			{
				_logger.LogInformation("Reset password request for email: {Email}", dto.Email);

				var user = await _userManager.FindByEmailAsync(dto.Email);
				if (user == null)
				{
					_logger.LogWarning("Reset password: User with email {Email} not found", dto.Email);
					return BadRequest(new { Message = "Invalid email address." });
				}

				var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
				if (!result.Succeeded)
				{
					_logger.LogWarning("Reset password failed for {Email}: {Errors}",
						dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
					return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
				}

				_logger.LogInformation("Password reset successfully for {Email}", dto.Email);
				return Ok(new { Message = "Password has been reset successfully." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during reset password for {Email}", dto.Email);
				return StatusCode(500, new { Message = "An error occurred while resetting the password." });
			}
		}

		[HttpPost("RefreshToken")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
		{
			try
			{
				_logger.LogInformation("Refresh token request");

				var (newAccessToken, newRefreshToken) = await _tokenService.RefreshTokenAsync(dto.RefreshToken);
				if (newAccessToken == null)
				{
					_logger.LogWarning("Invalid or expired refresh token");
					return Unauthorized(new { Message = "Invalid or expired refresh token." });
				}

				_logger.LogInformation("Token refreshed successfully");
				return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during token refresh");
				return StatusCode(500, new { Message = "An error occurred while refreshing the token." });
			}
		}
	}
}