using JobConnect.Apis.IRepository;
using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using JobConnect.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobConnect.Apis.Repository
{
	public class JobSeekerRepository : IJobSeekerRepository
	{
		private readonly AppDbContext _context;

		public JobSeekerRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId)
		{
			if (string.IsNullOrEmpty(jobSeekerId))
				return null;

			return await _context.JobSeekers
				.Include(js => js.SavedJobs)
				.ThenInclude(sj => sj.Job)
				.Include(js => js.Applications)
				.Include(js => js.Resumes) // Include the new Resumes table
				.FirstOrDefaultAsync(js => js.Id == jobSeekerId);
		}

		public async Task UpdateJobSeekerAsync(JobSeeker jobSeeker)
		{
			_context.JobSeekers.Update(jobSeeker);
			await _context.SaveChangesAsync();
		}

		//public async Task<IEnumerable<Job>> GetSavedJobsAsync(string jobSeekerId)
		//{
		//	return await _context.SavedJobs
		//		.Where(sj => sj.JobSeekerId == jobSeekerId)
		//		.Select(sj => sj.Job)
		//		.Include(j => j.Applications)
		//		.ToListAsync();
		//}

		public async Task<IEnumerable<Job>> GetSavedJobsAsync(string jobSeekerId)
		{
			return await _context.SavedJobs
				.Where(sj => sj.JobSeekerId == jobSeekerId)
				.Include(sj => sj.Job) // Include Job first
				.ThenInclude(j => j.Applications) // Then Include Applications
				// Include Tags for the Job
				 // Include Responsibilities for the Job
				.Select(sj => sj.Job)
				.ToListAsync();
		}

		public async Task SaveJobAsync(string jobSeekerId, int jobId)
		{
			var existing = await _context.SavedJobs
				.FirstOrDefaultAsync(sj => sj.JobSeekerId == jobSeekerId && sj.JobId == jobId);

			if (existing == null)
			{
				_context.SavedJobs.Add(new SavedJob
				{
					JobSeekerId = jobSeekerId,
					JobId = jobId,
					SavedDate = DateTime.UtcNow
				});
				await _context.SaveChangesAsync();
			}
		}

		public async Task UnsaveJobAsync(string jobSeekerId, int jobId)
		{
			var existing = await _context.SavedJobs
				.FirstOrDefaultAsync(sj => sj.JobSeekerId == jobSeekerId && sj.JobId == jobId);

			if (existing != null)
			{
				_context.SavedJobs.Remove(existing);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Job>> GetAllJobsAsync()
		{
			return await _context.Jobs
				.Include(j => j.Applications)
				.ToListAsync();
		}

		public async Task<Job> GetJobByIdAsync(int jobId)
		{
			return await _context.Jobs
				.Include(j => j.Applications)
				.FirstOrDefaultAsync(j => j.Id == jobId);
		}

		public async Task ApplyForJobAsync(string jobSeekerId, int jobId, string coverLetter, string resumePath)
		{
			var application = new Application
			{
				JobSeekerId = jobSeekerId,
				JobId = jobId,
				CoverLetter = coverLetter,
				Resume = resumePath,
				ApplicationDate = DateTime.UtcNow
			};

			await _context.Applications.AddAsync(application);
			await _context.SaveChangesAsync();
		}
	}
}