using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobConnect.Core.Models
{
	[Table("JobSeekers")]
	public class JobSeeker : User
	{

		public string Address { get; set; }
		public int? YearsOfExperience { get; set; }
		public string Degree { get; set; }
		public string CurrentOrDesiredJob { get; set; }


	}
}
