//using JobConnect.Core.Models;
//using JobConnect.Apis.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace JobConnect.Apis.IRepository
//{
//	public interface IEmployerRepository
//	{
//		Task<Employer> GetEmployerByIdAsync(string employerId);
//		Task UpdateEmployerAsync(Employer employer);
//		Task<IEnumerable<Job>> GetJobsByEmployerAsync(string employerId);
//		Task<IEnumerable<Job>> GetRecentJobsByEmployerAsync(string employerId);
//		Task<Job?> GetJobByIdAsync(int jobId, string employerId);
//		Task AddJobAsync(Job job);
//		Task UpdateJobAsync(Job job);
//		Task DeleteJobAsync(int jobId, string employerId);
//		Task<int> GetJobsCountAsync(string employerId);
//		Task<int> GetCandidatesCountAsync(string employerId);
//		Task AddToShortlistAsync(int jobId, string jobSeekerId); // New method
//		Task RemoveFromShortlistAsync(int jobId, string jobSeekerId); // New method
//		Task<IEnumerable<Application>> GetShortlistedJobSeekersAsync(int jobId, string employerId); // New method
//		Task<(IEnumerable<Job> Jobs, int TotalCount)> GetJobsByEmployerPaginatedAsync(string employerId, int pageNumber, int pageSize);
//        Task<JobSeeker?> GetJobSeekerByIdAsync(string jobSeekerId);
//    }
//}

using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobConnect.Apis.IRepository
{
    public interface IEmployerRepository
    {
        Task<Employer> GetEmployerByIdAsync(string employerId);
        Task UpdateEmployerAsync(Employer employer);
        Task<IEnumerable<Job>> GetJobsByEmployerAsync(string employerId);
        Task<IEnumerable<Job>> GetRecentJobsByEmployerAsync(string employerId);
        Task<Job?> GetJobByIdAsync(int jobId, string employerId);
        Task<(IEnumerable<Job> Jobs, int TotalCount)> GetJobsByEmployerPaginatedAsync(string employerId, int pageNumber, int pageSize);
        Task AddJobAsync(Job job);
        Task UpdateJobAsync(Job job);
        Task DeleteJobAsync(int jobId, string employerId);
        Task<int> GetJobsCountAsync(string employerId);
        Task<int> GetCandidatesCountAsync(string employerId);
        Task AddToShortlistAsync(int jobId, string jobSeekerId);
        Task RemoveFromShortlistAsync(int jobId, string jobSeekerId);
        Task<IEnumerable<Application>> GetShortlistedJobSeekersAsync(int jobId, string employerId);
        Task<JobSeeker?> GetJobSeekerByIdAsync(string jobSeekerId);
        Task<IEnumerable<Application>> GetApplicationsByJobAsync(int jobId);

    }
}