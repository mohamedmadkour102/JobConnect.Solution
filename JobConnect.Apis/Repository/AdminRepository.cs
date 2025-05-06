using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using JobConnect.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobConnect.Apis.IRepository;

namespace JobConnect.Apis.Repository
{

	public class AdminRepository : IAdminRepository
	{
		private readonly AppDbContext _context;

		public AdminRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Employer>> GetAllEmployersAsync()
		{
			return await _context.Employers
				.Include(e => e.Jobs)
				.ToListAsync();
		}

		public async Task<IEnumerable<JobSeeker>> GetAllJobSeekersAsync()
		{
			return await _context.JobSeekers
				.Include(js => js.Applications)
				.Include(js => js.SavedJobs)
				.Include(js => js.Resumes)
				.ToListAsync();
		}

		public async Task<bool> DeleteUserAsync(string userId)
		{
			var user = await _context.Users
			  .FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
				return false;

			// Check if the user is an Employer with active jobs
			var employer = await _context.Employers
				.Include(e => e.Jobs)
				.FirstOrDefaultAsync(e => e.Id == userId);

			if (employer != null && employer.Jobs.Any())
			{
				// Prevent deletion if the employer has active jobs
				throw new InvalidOperationException("Cannot delete employer with active jobs.");
			}

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<Job>> GetAllJobsAsync()
		{
			return await _context.Jobs
				.Include(j => j.Employer)
				.Include(j => j.Tags)
				.Include(j => j.Responsibilities)
				.Include(j => j.Applications)
				.ToListAsync();
		}

		public async Task<IEnumerable<Job>> GetJobsByTagAsync(string tag)
		{
			return await _context.Jobs
				.Include(j => j.Employer)
				.Include(j => j.Tags)
				.Include(j => j.Responsibilities)
				.Include(j => j.Applications)
				.Where(j => j.Tags.Any(t => t.Tag == tag))
				.ToListAsync();
		}
	}
}