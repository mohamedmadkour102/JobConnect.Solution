namespace JobConnect.Application.DTOs.EmployerDto
{
    public class JobApplicantWithResumeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CurrentOrDesiredJob { get; set; }
        public int? YearsOfExperience { get; set; }
        public string Resume { get; set; } // Resume submitted with the application
        public string CoverLetter { get; set; }
        public DateTime ApplicationDate { get; set; }
        public bool IsShortlisted { get; set; }
    }
}
