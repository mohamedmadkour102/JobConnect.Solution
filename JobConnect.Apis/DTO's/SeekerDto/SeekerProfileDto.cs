namespace JobConnect.Apis.DTO_s.SeekerDto
{
    public class SeekerProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string CurrentOrDesiredJob { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string CoverLetter { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string Portfolio { get; set; } = string.Empty;
        public string FacebookLink { get; set; } = string.Empty;
        public string TwitterLink { get; set; } = string.Empty;
        public string InstagramLink { get; set; } = string.Empty;
        public string LinkedInLink { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;

        public List<CertificationDto> Certifications { get; set; } = new List<CertificationDto>();
        public List<CompanyWorkedAtDto> CompanyWorkedAt { get; set; } = new List<CompanyWorkedAtDto>();
        public List<SkillDto> Skills { get; set; } = new List<SkillDto>();
        public List<WorkedAsDto> WorkedAs { get; set; } = new List<WorkedAsDto>();
        public List<ResumeDto> Resumes { get; set; } = new List<ResumeDto>();
    }










}

