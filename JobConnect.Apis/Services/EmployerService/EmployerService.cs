using Microsoft.AspNetCore.Identity;
using JobConnect.Core.Models;
using JobConnect.Apis.DTO_s.EmployerDto;
using JobConnect.Apis.IRepository;
using JobConnect.Apis.IService;
using JobConnect.Apis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JobConnect.Apis.Services
{
	public class EmployerService : IEmployerService
	{
		private readonly IEmployerRepository _employerRepository;
		private readonly UserManager<User> _userManager;
		private readonly IWebHostEnvironment _environment;

		public EmployerService(IEmployerRepository employerRepository, UserManager<User> userManager, IWebHostEnvironment environment)
		{
			_employerRepository = employerRepository;
			_userManager = userManager;
			_environment = environment;
		}

		public async Task<Employer> GetEmployerByIdAsync(string employerId)
		{
			var employer = await _employerRepository.GetEmployerByIdAsync(employerId);
			if (employer == null)
				throw new Exception("Employer not found.");
			return employer;
		}

		public async Task UpdateCompanyInfoAsync(string employerId, UpdateCompanyInfoDto dto)
		{
			var employer = await _employerRepository.GetEmployerByIdAsync(employerId);
			if (employer == null)
				throw new Exception("Employer not found.");

			employer.CompanyName = dto.CompanyName ?? employer.CompanyName;
			employer.CompanyDescription = dto.CompanyDescription ?? employer.CompanyDescription;

			if (dto.Logo != null)
			{
				var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads/logos");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var fileName = $"{Guid.NewGuid()}_{dto.Logo.FileName}";
				var filePath = Path.Combine(uploadsFolder, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await dto.Logo.CopyToAsync(stream);
				}

				employer.LogoUrl = $"/Uploads/logos/{fileName}";
			}

			await _employerRepository.UpdateEmployerAsync(employer);
		}

		public async Task UpdateFoundingInfoAsync(string employerId, UpdateFoundingInfoDto dto)
		{
			var employer = await _employerRepository.GetEmployerByIdAsync(employerId);
			if (employer == null)
				throw new Exception("Employer not found.");

			employer.Industry = dto.Industry ?? employer.Industry;
			employer.CompanySize = dto.CompanySize ?? employer.CompanySize;
			employer.FoundingDate = dto.FoundingDate ?? employer.FoundingDate;
			employer.Website = dto.Website ?? employer.Website;
			employer.PhoneNumber = dto.PhoneNumber ?? employer.PhoneNumber;

			await _employerRepository.UpdateEmployerAsync(employer);
		}

		public async Task ChangePasswordAsync(string employerId, ChangePasswordDto dto)
		{
			var employer = await _userManager.FindByIdAsync(employerId);
			if (employer == null)
				throw new Exception("Employer not found.");

			var passwordCheck = await _userManager.CheckPasswordAsync(employer, dto.CurrentPassword);
			if (!passwordCheck)
				throw new Exception("Current password is incorrect.");

			var result = await _userManager.ChangePasswordAsync(employer, dto.CurrentPassword, dto.NewPassword);
			if (!result.Succeeded)
				throw new Exception("Failed to change password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
		}

		public async Task<IEnumerable<JobDto>> GetRecentJobsAsync(string employerId)
		{
			var jobs = await _employerRepository.GetRecentJobsByEmployerAsync(employerId);

			return jobs.Select(job => new JobDto
			{
				Id = job.Id,
				Title = job.Title,
				Status = job.Status,
				ApplicationsCount = job.Applications.Count,
				JobType = job.JobType,
				DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
				PostedDate = GetTimeAgo(job.PostedDate),
				Location = job.Location,
				ShortListed = job.ShortListed,
				Tags = job.Tags.Select(t => t.Tag).ToList(),
				Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList()
			}).ToList();
		}

		public async Task<IEnumerable<JobDto>> GetJobsByEmployerAsync(string employerId)
		{
			var jobs = await _employerRepository.GetJobsByEmployerAsync(employerId);

			return jobs.Select(job => new JobDto
			{
				Id = job.Id,
				Title = job.Title,
				Status = job.Status,
				ApplicationsCount = job.Applications.Count,
				JobType = job.JobType,
				DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
				PostedDate = GetTimeAgo(job.PostedDate),
				Location = job.Location,
				ShortListed = job.ShortListed,
				Tags = job.Tags.Select(t => t.Tag).ToList(),
				Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList()
			}).ToList();
		}

		public async Task<JobDto?> GetJobByIdAsync(int jobId, string employerId)
		{
			var job = await _employerRepository.GetJobByIdAsync(jobId, employerId);
			if (job == null) return null;

			return new JobDto
			{
				Id = job.Id,
				Title = job.Title,
				Status = job.Status,
				ApplicationsCount = job.Applications.Count,
				JobType = job.JobType,
				DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
				PostedDate = GetTimeAgo(job.PostedDate),
				Location = job.Location,
				ShortListed = job.ShortListed,
				Tags = job.Tags.Select(t => t.Tag).ToList(),
				Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList()
			};
		}

		public async Task AddJobAsync(string employerId, CreateJobDto jobDto)
		{
			var job = new Job
			{
				Title = jobDto.Title,
				Description = jobDto.Description,
				MinSalary = jobDto.MinSalary,
				MaxSalary = jobDto.MaxSalary,
				SalaryType = jobDto.SalaryType,
				Education = jobDto.Education,
				Experience = jobDto.Experience,
				Vacancies = jobDto.Vacancies,
				ExpirationDate = jobDto.ExpirationDate,
				JobType = jobDto.JobType,
				Status = jobDto.Status,
				ShortListed = jobDto.ShortListed,
				Location = jobDto.Location,
				EmployerId = employerId
			};

			// Add Tags
			if (jobDto.Tags != null && jobDto.Tags.Any())
			{
				job.Tags = jobDto.Tags.Select(tag => new JobTag { Tag = tag }).ToList();
			}

			// Add Responsibilities
			if (jobDto.Responsibilities != null && jobDto.Responsibilities.Any())
			{
				job.Responsibilities = jobDto.Responsibilities.Select(resp => new JobResponsibility { Responsibility = resp }).ToList();
			}

			await _employerRepository.AddJobAsync(job);
		}

		public async Task UpdateJobAsync(int jobId, string employerId, UpdateJobDto jobDto)
		{
			var job = await _employerRepository.GetJobByIdAsync(jobId, employerId);
			if (job == null) return;

			job.Title = jobDto.Title;
			job.Description = jobDto.Description;
			job.MaxSalary = jobDto.MaxSalary;
			job.MinSalary = jobDto.MinSalary;
			job.SalaryType = jobDto.SalaryType;
			job.Education = jobDto.Education;
			job.Experience = jobDto.Experience;
			job.Vacancies = jobDto.Vacancies;
			job.ExpirationDate = jobDto.ExpirationDate;
			job.JobType = jobDto.JobType;
			job.Status = jobDto.Status;
			job.ShortListed = jobDto.ShortListed;
			job.Location = jobDto.Location;

			// Update Tags
			job.Tags.Clear();
			if (jobDto.Tags != null && jobDto.Tags.Any())
			{
				job.Tags = jobDto.Tags.Select(tag => new JobTag { Tag = tag }).ToList();
			}

			// Update Responsibilities
			job.Responsibilities.Clear();
			if (jobDto.Responsibilities != null && jobDto.Responsibilities.Any())
			{
				job.Responsibilities = jobDto.Responsibilities.Select(resp => new JobResponsibility { Responsibility = resp }).ToList();
			}

			await _employerRepository.UpdateJobAsync(job);
		}

		public async Task DeleteJobAsync(int jobId, string employerId)
		{
			await _employerRepository.DeleteJobAsync(jobId, employerId);
		}

		public async Task<JobStatsDto> GetJobStatsAsync(string employerId)
		{
			return new JobStatsDto
			{
				JobsCount = await _employerRepository.GetJobsCountAsync(employerId),
				CandidatesCount = await _employerRepository.GetCandidatesCountAsync(employerId)
			};
		}

		private string GetTimeAgo(DateTime date)
		{
			TimeSpan timeSpan = DateTime.UtcNow - date;
			if (timeSpan.TotalDays >= 7)
				return $"{(int)timeSpan.TotalDays / 7} week ago";
			else if (timeSpan.TotalDays >= 1)
				return $"{(int)timeSpan.TotalDays} days ago";
			return "Today";
		}

		private int CalculateDaysRemaining(DateTime expirationDate)
		{
			int daysRemaining = (expirationDate - DateTime.UtcNow).Days;
			return daysRemaining > 0 ? daysRemaining : 0;
		}
	}
}