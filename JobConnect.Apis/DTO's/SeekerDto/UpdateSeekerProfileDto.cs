

namespace JobConnect.Apis.DTO_s.SeekerDto
{
    public class UpdateSeekerProfileDto
    {
        public string? Address { get; set; } 
        public int? YearsOfExperience { get; set; }
        public string? Degree { get; set; } 
        public string? CurrentOrDesiredJob { get; set; } 
        public string? Bio { get; set; } 
        public string? CoverLetter { get; set; } 
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; } 
        public string? MaritalStatus { get; set; } 
        public string? Gender { get; set; } 
        public string? Education { get; set; } 
        public string? Portfolio { get; set; }
        public string? FacebookLink { get; set; } 
        public string? TwitterLink { get; set; } 
        public string? InstagramLink { get; set; } 
        public string? LinkedInLink { get; set; }
        public string? CollegeName { get; set; } 
        public string? University { get; set; } 

        public List<CertificationDto>? Certifications { get; set; }
        public List<CompanyWorkedAtDto>? CompanyWorkedAt { get; set; }
        public List<SkillDto>? Skills { get; set; }
        public List<WorkedAsDto>? WorkedAs { get; set; }
    }
}