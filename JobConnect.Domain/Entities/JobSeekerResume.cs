using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Domain.Entities;

public class JobSeekerResume
{
    public int Id { get; set; }
    public string JobSeekerId { get; set; } = string.Empty;
    public JobSeeker? JobSeeker { get; set; }
    public string? ResumePath { get; set; }
    public string ResumeName { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
}
