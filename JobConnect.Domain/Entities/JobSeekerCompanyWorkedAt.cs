using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Domain.Entities;

[Table("JobSeekerCompanyWorkedAt")]
public class JobSeekerCompanyWorkedAt
{
    [Key]
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;

    [ForeignKey("JobSeeker")]
    public string JobSeekerId { get; set; } = string.Empty;
    public JobSeeker? JobSeeker { get; set; }
}
