using JobConnect.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobConnect.Core.Services
{
	public interface ITokenServices
	{
		Task<string> CreateTokenAsync(User user);
	}
}
