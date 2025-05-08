using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.DAL
{
	public class Seed
	{
		public static async Task SeedAsync(IServiceProvider serviceProvider)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

			if (!await roleManager.RoleExistsAsync("Admin"))
			{
				await roleManager.CreateAsync(new IdentityRole("Admin"));
			}
			if (!await roleManager.RoleExistsAsync("User"))
			{
				await roleManager.CreateAsync(new IdentityRole("User"));
			}


			var adminUser = await userManager.FindByEmailAsync("Abdallah@gmail.com");
			if (adminUser == null)
			{
				adminUser = new IdentityUser
				{
					UserName = "Abdallah Soliman",
					Email = "Abdallah@gmail.com",
					EmailConfirmed = true
				};
				await userManager.CreateAsync(adminUser, "Abdallah123");
				await userManager.AddToRoleAsync(adminUser, "Admin");
			}
		}
	}
}

