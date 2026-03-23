using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Domain.Entities;

[Table("JobSeekers")]
public class JobSeeker : User
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

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
    public ICollection<JobSeekerResume> Resumes { get; set; } = new List<JobSeekerResume>();
    public ICollection<JobSeekerCertification> Certifications { get; set; } = new List<JobSeekerCertification>();
    public ICollection<JobSeekerCompanyWorkedAt> CompanyWorkedAt { get; set; } = new List<JobSeekerCompanyWorkedAt>();
    public ICollection<JobSeekerSkill> Skills { get; set; } = new List<JobSeekerSkill>();
    public ICollection<JobSeekerWorkedAs> WorkedAs { get; set; } = new List<JobSeekerWorkedAs>();
}
