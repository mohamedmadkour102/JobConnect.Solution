using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Core.Models
{
    [Table("JobSeekerCertifications")]
    public class JobSeekerCertification
    {
        [Key]
        public int Id { get; set; }
        public string CertificationName { get; set; } = string.Empty;
        //public string IssuingOrganization { get; set; } = string.Empty;
        //public DateTime IssueDate { get; set; }
        //public DateTime? ExpiryDate { get; set; }

        [ForeignKey("JobSeeker")]
        public string JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
    }
}