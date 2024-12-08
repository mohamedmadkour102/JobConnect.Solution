using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobConnect.Core.Models
{
	public class EmailSettings
	{
		public string SmtpServer { get; set; }
		public int SmtpPort { get; set; }
		public string SmtpUsername { get; set; }
		public string SmtpPassword { get; set; }
		public string FromEmail { get; set; }
		public string FromName { get; set; }
		public string ClientBaseUrl {  get; set; }
	}
}
