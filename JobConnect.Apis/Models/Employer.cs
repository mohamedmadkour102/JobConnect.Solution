using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobConnect.Core.Models
{
	[Table("Employers")]

	public class Employer : User
	{
		public string CompanyName { get; set; }

		public string CompanySize { get; set; }

		[Url]
		public string Website { get; set; }

		public string Industry { get; set; }
		public string Address { get; set; }
		public string CompanyDescription { get; set; }

	}
}
