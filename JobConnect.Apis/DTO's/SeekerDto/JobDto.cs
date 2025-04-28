namespace JobConnect.Apis.DTO_s.SeekerDto
{
	public class JobDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string JobType { get; set; } = string.Empty;
		public int DaysRemaining { get; set; }
		public int ApplicationsCount { get; set; }
		public string PostedDate { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public bool ShortListed { get; set; }
		public string Description { get; set; } = string.Empty;
		public decimal MinSalary { get; set; }
		public decimal MaxSalary { get; set; }
		public string SalaryType { get; set; } = string.Empty;
		public string Education { get; set; } = string.Empty;
		public string Experience { get; set; } = string.Empty;
		public int Vacancies { get; set; }
		public List<string> Responsibilities { get; set; } = new List<string>();
		public List<string> Tags { get; set; } = new List<string>();
	}
}
