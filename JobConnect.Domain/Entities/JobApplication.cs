using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Domain.Entities;

[Table("Applications")]
public class JobApplication
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job? Job { get; set; }
    public string JobSeekerId { get; set; } = string.Empty;
    public JobSeeker? JobSeeker { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
    public string CoverLetter { get; set; } = string.Empty;
    public string Resume { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
    public bool IsShortlisted { get; set; }
}
