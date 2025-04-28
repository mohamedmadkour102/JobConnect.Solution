using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JobConnect.Apis.Models;

namespace JobConnect.Core.Models
{
	[Table("Employers")]
	public class Employer : User
	{
		public string CompanyName { get; set; } = string.Empty;
		public string CompanySize { get; set; } = string.Empty;
		[Url]
		public string Website { get; set; } = string.Empty;
		public string Industry { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string CompanyDescription { get; set; } = string.Empty;
		public string? LogoUrl { get; set; }
		public DateTime? FoundingDate { get; set; }
		public string? PhoneNumber { get; set; }
		public ICollection<Job> Jobs { get; set; } = new List<Job>();
	}
}