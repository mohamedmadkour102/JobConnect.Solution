using System;
using System.Collections.Generic;

namespace JobConnect.Apis.DTO_s.EmployerDto
{
	public class UpdateJobDto
	{
		public string Title { get; set; } = string.Empty;
		public List<string> Tags { get; set; } = new List<string>();
		public string Description { get; set; } = string.Empty;
		public decimal MaxSalary { get; set; }
		public decimal MinSalary { get; set; }
		public string SalaryType { get; set; } = string.Empty;
		public string Education { get; set; } = string.Empty;
		public string Experience { get; set; } = string.Empty;
		public int Vacancies { get; set; }
		public DateTime ExpirationDate { get; set; }
		public string JobType { get; set; } = string.Empty;
        public string WorkPlace { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
		public List<string> Responsibilities { get; set; } = new List<string>();
		
		public string Location { get; set; } = string.Empty;
	}
}