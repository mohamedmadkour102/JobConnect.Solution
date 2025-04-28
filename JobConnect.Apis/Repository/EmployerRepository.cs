using JobConnect.Apis.IRepository;
using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using JobConnect.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobConnect.Apis.Repository
{
	public class EmployerRepository : IEmployerRepository
	{
		private readonly AppDbContext _context;

		public EmployerRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<Employer> GetEmployerByIdAsync(string employerId)
		{
			if (string.IsNullOrEmpty(employerId))
				return null;

			return await _context.Employers
				.Include(e => e.Jobs)
				.ThenInclude(j => j.Tags)
				.Include(e => e.Jobs)
				.ThenInclude(j => j.Responsibilities)
				.FirstOrDefaultAsync(e => e.Id == employerId);
		}

		public async Task UpdateEmployerAsync(Employer employer)
		{
			_context.Employers.Update(employer);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Job>> GetJobsByEmployerAsync(string employerId)
		{
			return await _context.Jobs
				.Include(j => j.Applications)
				.Include(j => j.Tags)
				.Include(j => j.Responsibilities)
				.Where(j => j.EmployerId == employerId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Job>> GetRecentJobsByEmployerAsync(string employerId)
		{
			return await _context.Jobs
				.Include(j => j.Applications)
				.Include(j => j.Tags)
				.Include(j => j.Responsibilities)
				.Where(j => j.EmployerId == employerId)
				.OrderByDescending(j => j.PostedDate)
				.Take(10)
				.ToListAsync();
		}

		public async Task<Job?> GetJobByIdAsync(int jobId, string employerId)
		{
			return await _context.Jobs
				.Include(j => j.Applications)
				.Include(j => j.Tags)
				.Include(j => j.Responsibilities)
				.FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employerId);
		}

		public async Task AddJobAsync(Job job)
		{
			await _context.Jobs.AddAsync(job);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateJobAsync(Job job)
		{
			_context.Jobs.Update(job);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteJobAsync(int jobId, string employerId)
		{
			var job = await _context.Jobs
				.FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employerId);
			if (job != null)
			{
				_context.Jobs.Remove(job);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<int> GetJobsCountAsync(string employerId)
		{
			return await _context.Jobs
				.CountAsync(j => j.EmployerId == employerId);
		}

		public async Task<int> GetCandidatesCountAsync(string employerId)
		{
			return await _context.Applications
				.CountAsync(a => a.Job.EmployerId == employerId);
		}
	}
}