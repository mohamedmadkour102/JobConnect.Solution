using JobConnect.Application.DTOs.EmployerDto;

namespace JobConnect.Application.DTOs.SeekerDto
{
    public class AppliedJobSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
       
        public string Location { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public List<ApplicantDto> Applicants { get; set; } = new List<ApplicantDto>();
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string JobType { get; set; } = string.Empty;
        public string WorkPlace { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string PostedDate { get; set; } = string.Empty;
        public EmployerInfo Employer { get; set; }
        public string SalaryType {  get; set; }

    }
}