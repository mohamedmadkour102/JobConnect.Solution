using JobConnect.Apis.DTO_s.SeekerDto;
using JobConnect.Apis.IRepository;
using JobConnect.Apis.IService;
using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JobConnect.Apis.Services
{
	public class JobSeekerService : IJobSeekerService
	{
		private readonly IJobSeekerRepository _jobSeekerRepository;
		private readonly IWebHostEnvironment _environment;

		public JobSeekerService(IJobSeekerRepository jobSeekerRepository, IWebHostEnvironment environment)
		{
			_jobSeekerRepository = jobSeekerRepository;
			_environment = environment;
		}

		public async Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId)
		{
			var jobSeeker = await _jobSeekerRepository.GetJobSeekerByIdAsync(jobSeekerId);
			if (jobSeeker == null)
				throw new Exception("JobSeeker not found.");
			return jobSeeker;
		}

		public async Task<IEnumerable<JobDto>> GetSavedJobsAsync(string jobSeekerId)
		{
			var jobs = await _jobSeekerRepository.GetSavedJobsAsync(jobSeekerId);

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
				ShortListed = job.ShortListed
			}).ToList();
		}

		public async Task SaveJobAsync(string jobSeekerId, int jobId)
		{
			await _jobSeekerRepository.SaveJobAsync(jobSeekerId, jobId);
		}

		public async Task UnsaveJobAsync(string jobSeekerId, int jobId)
		{
			await _jobSeekerRepository.UnsaveJobAsync(jobSeekerId, jobId);
		}

		public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
		{
			var jobs = await _jobSeekerRepository.GetAllJobsAsync();

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
				ShortListed = job.ShortListed
			}).ToList();
		}

		public async Task<JobDto> GetJobByIdAsync(int jobId)
		{
			var job = await _jobSeekerRepository.GetJobByIdAsync(jobId);
			if (job == null)
				throw new Exception("Job not found.");

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
				Description = job.Description,
				MinSalary = job.MinSalary,
				MaxSalary = job.MaxSalary,
				SalaryType = job.SalaryType,
				Education = job.Education,
				Experience = job.Experience,
				Vacancies = job.Vacancies,
				Responsibilities = (List<string>)job.Responsibilities,
				Tags = (List<string>)job.Tags
			};
		}

		public async Task ApplyForJobAsync(string jobSeekerId, ApplyForJobDto applyDto)
		{
			string resumePath;

			// Get the JobSeeker to access their existing resumes
			var jobSeeker = await _jobSeekerRepository.GetJobSeekerByIdAsync(jobSeekerId);
			if (jobSeeker == null)
				throw new Exception("JobSeeker not found.");

			// Check if the JobSeeker selected an existing resume
			if (!string.IsNullOrEmpty(applyDto.SelectedResumePath))
			{
				// Validate that the selected resume exists in JobSeeker.Resumes
				var selectedResume = jobSeeker.Resumes.FirstOrDefault(r => r.ResumePath == applyDto.SelectedResumePath);
				if (selectedResume == null)
					throw new Exception("Selected resume not found in your profile.");

				resumePath = selectedResume.ResumePath;
			}
			else if (applyDto.Resume != null)
			{
				// Upload the new resume file to the server
				var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads/resumes");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var fileName = $"{Guid.NewGuid()}_{applyDto.Resume.FileName}";
				var filePath = Path.Combine(uploadsFolder, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await applyDto.Resume.CopyToAsync(stream);
				}

				resumePath = $"/uploads/resumes/{fileName}";

				// Add the new resume to JobSeeker.Resumes as a new JobSeekerResume entity
				var newResume = new JobSeekerResume
				{
					JobSeekerId = jobSeekerId,
					ResumePath = resumePath,
					ResumeName = applyDto.Resume.FileName,
					UploadDate = DateTime.UtcNow
				};
				jobSeeker.Resumes.Add(newResume);
				await _jobSeekerRepository.UpdateJobSeekerAsync(jobSeeker);
			}
			else
			{
				throw new Exception("You must either select an existing resume or upload a new one.");
			}

			await _jobSeekerRepository.ApplyForJobAsync(jobSeekerId, applyDto.JobId, applyDto.CoverLetter, resumePath);
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