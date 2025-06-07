using JobConnect.Apis.DTO_s.EmployerDto;

namespace JobConnect.Apis.DTO_s.SeekerDto
{
    public class SavedJobSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EmployerName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public List<ApplicantDto> Applicants { get; set; } = new List<ApplicantDto>();
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string JobType { get; set; } = string.Empty;
        public string WorkPlace { get; set; } = string.Empty;
    }
}