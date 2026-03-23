using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Domain.Entities;

[Table("JobSeekerWorkedAs")]
public class JobSeekerWorkedAs
{
    [Key]
    public int Id { get; set; }
    public string JobTitle { get; set; } = string.Empty;

    [ForeignKey("JobSeeker")]
    public string JobSeekerId { get; set; } = string.Empty;
    public JobSeeker? JobSeeker { get; set; }
}
