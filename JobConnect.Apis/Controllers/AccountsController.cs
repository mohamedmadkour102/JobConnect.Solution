using JobConnect.Application.Abstractions;
using JobConnect.Application.DTOs;
using JobConnect.Domain.Entities;
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
                    PhoneNumber = dto.PhoneNumber,
                    UserName = $"{dto.FirstName}{dto.LastName}",
                    CompanyName = dto.CompanyName,
                    CompanyDescription = dto.CompanyDescription,
                    CompanySize = dto.CompanySize,
                    Industry = dto.Industry,
                    Address = dto.Address,
                    Website = dto.Website,
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

                var (token, refreshToken) = await _tokenService.GenerateTokensAsync(employer);

                return Ok(new
                {
                    Id = employer.Id, 
                    Name = $"{employer.FirstName} {employer.LastName}",
                    Email = employer.Email,
                    Token = token,
                    Role = "Employer",
                    RefreshToken = refreshToken
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
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    UserName = $"{dto.FirstName}{dto.LastName}",
                    YearsOfExperience = dto.YearsOfExperience,
                    CurrentOrDesiredJob = dto.CurrentOrDesiredJob,
                    Degree = dto.Degree
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

                var (token, refreshToken) = await _tokenService.GenerateTokensAsync(jobSeeker);

                return Ok(new
                {
                    Id = jobSeeker.Id, 
                    Name = $"{jobSeeker.FirstName} {jobSeeker.LastName}",
                    Email = jobSeeker.Email,
                    Token = token,
                    Role = "JobSeeker",
                    RefreshToken = refreshToken
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
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Token = token,
                    Role = role,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", dto.Email);
                return StatusCode(500, new { Message = "An error occurred during login." });
            }
        }



        [HttpPost("Logout")]
		public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
		{
			try
			{
				_logger.LogInformation("Logout attempt");

				var result = await _tokenService.LogoutAsync(dto.RefreshToken);
				if (!result)
				{
					_logger.LogWarning("Logout failed: Invalid or expired refresh token");
					return Unauthorized(new { Message = "Invalid or expired refresh token." });
				}

				_logger.LogInformation("Logout successful");
				return Ok(new { Message = "Logout successful." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during logout");
				return StatusCode(500, new { Message = "An error occurred during logout." });
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

				var (newAccessToken, newRefreshToken) = await _tokenService.RefreshTokenAsync(dto.AccessToken, dto.RefreshToken);
				if (newAccessToken == null || newRefreshToken == null)
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