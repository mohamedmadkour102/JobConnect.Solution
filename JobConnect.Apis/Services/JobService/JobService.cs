//using JobConnect.Apis.DTO_s.EmployerDto;
//using JobConnect.Apis.IRepository;
//using JobConnect.Apis.IService;
//using JobConnect.Apis.Models;

//namespace JobConnect.Apis.Services.JobService
//{
//	public class JobService : IJobService
//	{
//		private readonly IJobRepository _jobRepository;

//		//public JobService(IJobRepository jobRepository)
//		//{
//		//	_jobRepository = jobRepository;
//		//}
//		public JobService(IJobRepository jobRepository)
//		{
//			_jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
//			Console.WriteLine("JobService initialized successfully");
//		}
//		public async Task<IEnumerable<JobDto>> GetRecentJobsAsync()
//		{
//			var jobs = await _jobRepository.GetRecentJobsAsync();

//			return jobs.Select(job => new JobDto
//			{
//				Id = job.Id,
//				Title = job.Title,
//				Status = job.Status,
//				ApplicationsCount = job.Applications.Count,
//				JobType = job.JobType,
//				DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
//				PostedDate = GetTimeAgo(job.PostedDate)
//			}).ToList();
//		}

//		public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
//		{
//			var jobs = await _jobRepository.GetAllJobsAsync();

//			return jobs.Select(job => new JobDto
//			{
//				Id = job.Id,
//				Title = job.Title,
//				Status = job.Status,
//				ApplicationsCount = job.Applications.Count,
//				JobType = job.JobType,
//				DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
//				PostedDate = GetTimeAgo(job.PostedDate)
//			}).ToList();
//		}

//		public async Task<JobDto?> GetJobByIdAsync(int jobId)
//		{
//			var job = await _jobRepository.GetJobByIdAsync(jobId);
//			if (job == null) return null;

//			return new JobDto
//			{
//				Id = job.Id,
//				Title = job.Title,
//				Status = job.Status,
//				ApplicationsCount = job.Applications.Count,
//				JobType = job.JobType,
//				DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
//				PostedDate = GetTimeAgo(job.PostedDate)
//			};
//		}

//		public async Task AddJobAsync(CreateJobDto jobDto)
//		{
//			var job = new Job
//			{
//				Title = jobDto.Title,
//				Tag = jobDto.Tag,
//				Role = jobDto.Role,
//				MinSalary = jobDto.MinSalary,
//				MaxSalary = jobDto.MaxSalary,
//				SalaryType = jobDto.SalaryType,
//				Education = jobDto.Education,
//				Experience = jobDto.Experience,
//				Vacancies = jobDto.Vacancies,
//				ExpirationDate = jobDto.ExpirationDate,
//				JobType = jobDto.JobType,
//				Description = jobDto.Description,
//				Status = jobDto.Status,

//			};

//			await _jobRepository.AddJobAsync(job);
//		}

//		public async Task UpdateJobAsync(int jobId, UpdateJobDto jobDto)
//		{
//			var job = await _jobRepository.GetJobByIdAsync(jobId);
//			if (job == null) return;


//			job.Title = jobDto.Title;
//			job.Tag = jobDto.Tag;
//			job.Role = jobDto.Role;
//			job.Description = jobDto.Description;
//			job.MaxSalary = jobDto.MaxSalary;
//			job.MinSalary = jobDto.MinSalary;
//			job.SalaryType = jobDto.SalaryType;
//			job.Education = jobDto.Education;
//			job.Experience = jobDto.Experience;
//			job.Vacancies = jobDto.Vacancies;
//			job.ExpirationDate = jobDto.ExpirationDate;
//			job.JobType = jobDto.JobType;
//			job.Status = jobDto.Status;


//			await _jobRepository.UpdateJobAsync(job);
//		}


//		public async Task DeleteJobAsync(int jobId)
//		{
//			await _jobRepository.DeleteJobAsync(jobId);
//		}

//		public async Task<JobStatsDto> GetJobStatsAsync()
//		{
//			return new JobStatsDto
//			{
//				JobsCount = await _jobRepository.GetJobsCountAsync(),
//				CandidatesCount = await _jobRepository.GetCandidatesCountAsync()
//			};
//		}

//		private string GetTimeAgo(DateTime date)
//		{
//			TimeSpan timeSpan = DateTime.UtcNow - date;
//			if (timeSpan.TotalDays >= 7)
//				return $"{(int)timeSpan.TotalDays / 7} week ago";
//			else if (timeSpan.TotalDays >= 1)
//				return $"{(int)timeSpan.TotalDays} days ago";
//			return "Today";
//		}
//		private int CalculateDaysRemaining(DateTime expirationDate)
//		{
//			int daysRemaining = (expirationDate - DateTime.UtcNow).Days;
//			return daysRemaining > 0 ? daysRemaining : 0; 
//		}
//	}
//}
