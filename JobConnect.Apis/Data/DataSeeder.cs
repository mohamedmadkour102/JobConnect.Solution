using Microsoft.AspNetCore.Identity;
using JobConnect.Core.Models;
using System.Threading.Tasks;

namespace JobConnect.Repository.Data
{
	public class DataSeeder
	{
		public static async Task SeedAdmin(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			
			if (!await roleManager.RoleExistsAsync("admin"))
			{
				await roleManager.CreateAsync(new IdentityRole("admin"));
			}

			
			var adminEmail = "admin@jobconnect.com";
			var adminUser = await userManager.FindByEmailAsync(adminEmail);
			if (adminUser == null)
			{
				var admin = new User
				{
					UserName = adminEmail,
					Email = adminEmail,
					EmailConfirmed = true 
				};

				var result = await userManager.CreateAsync(admin, "Admin@12345"); 
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(admin, "admin");
				}
			}
		}
	}
}