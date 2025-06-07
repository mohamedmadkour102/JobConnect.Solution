using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Core.Models
{
    [Table("JobSeekerCompanyWorkedAt")]
    public class JobSeekerCompanyWorkedAt
    {
        [Key]
        public int Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [ForeignKey("JobSeeker")]
        public string JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
    }
}