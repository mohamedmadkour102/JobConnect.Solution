using System;
using System.Collections.Generic;

namespace JobConnect.Apis.DTO_s.EmployerDto
{
    public class EmployerProfileDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string CompanySize { get; set; }
        public string Website { get; set; }
        public string Industry { get; set; }
        public string Address { get; set; }
        public string CompanyDescription { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime? FoundingDate { get; set; }
        public string? PhoneNumber { get; set; }
        public List<JobEmpDto> Jobs { get; set; } = new List<JobEmpDto>();
    }

    public class JobEmpDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string JobType { get; set; }
        public string WorkPlace { get; set; }
        public string PostedDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string SalaryType { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }
        public int Vacancies { get; set; }
    }
}