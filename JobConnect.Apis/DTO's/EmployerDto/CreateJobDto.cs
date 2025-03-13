namespace JobConnect.Apis.DTO_s.EmployerDto
{
	public class CreateJobDto
	{
		public string Title { get; set; } 
		public string Tag { get; set; }
		public string Role { get; set; }
		public string Description { get; set; }
		public decimal MaxSalary { get; set; }
		public decimal MinSalary { get; set; }
		public string SalaryType { get; set; }
		public string Education { get; set; }
		public string Experience { get; set; }
		public int Vacancies { get; set; }
		public DateTime ExpirationDate { get; set; }
		public string JobType { get; set; }
		public string Status { get; set; }

	}
}
