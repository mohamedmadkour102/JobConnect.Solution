using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Core.Models
{
    [Table("JobSeekerSkills")]
    public class JobSeekerSkill
    {
        [Key]
        public int Id { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public int ProficiencyLevel { get; set; } // 1-10 for example

        [ForeignKey("JobSeeker")]
        public string JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
    }
}