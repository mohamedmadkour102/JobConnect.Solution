using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Domain.Entities;

public class Job
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinSalary { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal MaxSalary { get; set; }
    public string SalaryType { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public int Vacancies { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ApplicationCount { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string WorkPlace { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    public string Location { get; set; } = string.Empty;

    public string EmployerId { get; set; } = string.Empty;
    [ForeignKey("EmployerId")]
    public Employer? Employer { get; set; }

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
    public ICollection<JobTag> Tags { get; set; } = new List<JobTag>();
    public ICollection<JobResponsibility> Responsibilities { get; set; } = new List<JobResponsibility>();
}
