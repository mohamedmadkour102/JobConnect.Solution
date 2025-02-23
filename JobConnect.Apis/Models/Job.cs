using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Apis.Models
{
	public class Job
	{
		public int Id { get; set; }
		public string Tag { get; set; } = string.Empty;
		public string Role { get; set; }
		public string Description { get; set; }
		[Column(TypeName = "decimal(18,2)")]
		public decimal MinSalary { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal MaxSalary { get; set; }
		public string SalaryType { get; set; }
		public string Education { get; set; }
		public string Experience { get; set; }
		public int Vacancies { get; set; }
		public DateTime ExpirationDate { get; set; }
		public string Title { get; set; } 
		public string Status { get; set; } 
		public int ApplicationCount { get; set; } 
		public string JobType { get; set; } 
		public int DaysRemaining { get; set; }
		public DateTime PostedDate { get; set; } = DateTime.UtcNow;
		public ICollection<Application> Applications { get; set; } = new List<Application>();
	}
}
