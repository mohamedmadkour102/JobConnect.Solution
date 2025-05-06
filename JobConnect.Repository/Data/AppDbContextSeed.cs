using JobConnect.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobConnect.Repository.Data
{
	public static class AppDbContextSeed
	{
		public static async Task SeedUserAsync(UserManager<User> _userManager)
		{
			if (_userManager.Users.Count() == 0)
			{
				var user = new User()
				{
					FirstName = "Admin",
					LastName = "123",
					Email = "Admin123@gmail.com",
					UserName = "Admin_123",
					PhoneNumber = "01122334455"

				};
				await _userManager.CreateAsync(user, "Pa$$W0rd1");
			}

		}
	}
}
