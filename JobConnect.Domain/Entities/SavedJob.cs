using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Domain.Entities;

public class SavedJob
{
    public int Id { get; set; }
    public string JobSeekerId { get; set; } = string.Empty;
    [ForeignKey("JobSeekerId")]
    public JobSeeker? JobSeeker { get; set; }
    public int JobId { get; set; }
    [ForeignKey("JobId")]
    public Job? Job { get; set; }
    public DateTime SavedDate { get; set; } = DateTime.UtcNow;
}
