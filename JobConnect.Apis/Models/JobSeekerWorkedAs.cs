using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Core.Models
{
    [Table("JobSeekerWorkedAs")]
    public class JobSeekerWorkedAs
    {
        [Key]
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        //public DateTime StartDate { get; set; }
        //public DateTime? EndDate { get; set; }

        [ForeignKey("JobSeeker")]
        public string JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
    }
}