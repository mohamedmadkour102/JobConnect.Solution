using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using JobConnect.Apis.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobConnect.Apis.DTO_s.Admin;
using JobConnect.Apis.IRepository;
using JobConnect.Apis.IService;


namespace JobConnect.Apis.Services
{


	public class AdminService : IAdminService
	{
		private readonly IAdminRepository _adminRepository;

		public AdminService(IAdminRepository adminRepository)
		{
			_adminRepository = adminRepository;
		}

		public async Task<IEnumerable<EmployerDto>> GetAllEmployersAsync()
		{
			var employers = await _adminRepository.GetAllEmployersAsync();
			return employers.Select(e => new EmployerDto
			{
				Id = e.Id,
				CompanyName = e.CompanyName,
				Email = e.Email,
				Industry = e.Industry,
				CompanySize = e.CompanySize,
				Website = e.Website,
				Address = e.Address,
				CompanyDescription = e.CompanyDescription,
				LogoUrl = e.LogoUrl,
				FoundingDate = e.FoundingDate,
				PhoneNumber = e.PhoneNumber,
				JobsCount = e.Jobs.Count
			});
		}

		public async Task<IEnumerable<JobSeekerDto>> GetAllJobSeekersAsync()
		{
			var jobSeekers = await _adminRepository.GetAllJobSeekersAsync();
			return jobSeekers.Select(js => new JobSeekerDto
			{
				Id = js.Id,
				FirstName = js.FirstName,
				LastName = js.LastName,
				Email = js.Email,
				Address = js.Address,
				YearsOfExperience = js.YearsOfExperience,
				Degree = js.Degree,
				CurrentOrDesiredJob = js.CurrentOrDesiredJob,
				Bio = js.Bio,
				CoverLetter = js.CoverLetter,
				DateOfBirth = js.DateOfBirth,
				Nationality = js.Nationality,
				MaritalStatus = js.MaritalStatus,
				Gender = js.Gender,
				Education = js.Education,
				Portfolio = js.Portfolio,
				FacebookLink = js.FacebookLink,
				TwitterLink = js.TwitterLink,
				InstagramLink = js.InstagramLink,
				LinkedInLink = js.LinkedInLink,
				ApplicationsCount = js.Applications.Count,
				SavedJobsCount = js.SavedJobs.Count
			});
		}

		public async Task<bool> DeleteUserAsync(string userId)
		{
			return await _adminRepository.DeleteUserAsync(userId);
		}

		public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
		{
			var jobs = await _adminRepository.GetAllJobsAsync();
			return jobs.Select(j => new JobDto
			{
				Id = j.Id,
				Title = j.Title,
				Status = j.Status,
				ApplicationsCount = j.Applications.Count,
				JobType = j.JobType,
				DaysRemaining = CalculateDaysRemaining(j.ExpirationDate),
				PostedDate = GetTimeAgo(j.PostedDate),
				Location = j.Location,
				Tags = j.Tags.Select(t => t.Tag).ToList(),
				Responsibilities = j.Responsibilities.Select(r => r.Responsibility).ToList(),
				EmployerName = j.Employer.CompanyName
			});
		}

		public async Task<IEnumerable<JobDto>> GetJobsByTagAsync(string tag)
		{
			var jobs = await _adminRepository.GetJobsByTagAsync(tag);
			return jobs.Select(j => new JobDto
			{
				Id = j.Id,
				Title = j.Title,
				Status = j.Status,
				ApplicationsCount = j.Applications.Count,
				JobType = j.JobType,
				DaysRemaining = CalculateDaysRemaining(j.ExpirationDate),
				PostedDate = GetTimeAgo(j.PostedDate),
				Location = j.Location,
				Tags = j.Tags.Select(t => t.Tag).ToList(),
				Responsibilities = j.Responsibilities.Select(r => r.Responsibility).ToList(),
				EmployerName = j.Employer.CompanyName
			});
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