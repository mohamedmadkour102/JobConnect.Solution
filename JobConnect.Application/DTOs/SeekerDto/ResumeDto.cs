namespace JobConnect.Application.DTOs.SeekerDto
{
    public class ResumeDto
    {
        public string ResumePath { get; set; } = string.Empty;
        public string ResumeName { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
    }
}
