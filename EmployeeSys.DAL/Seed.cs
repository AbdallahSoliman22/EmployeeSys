// DAL/DbInitializer/Seed.cs
using Microsoft.AspNetCore.Identity;

public static class Seed
{
	public static async Task SeedRolesAndAdminAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		string adminEmail = "Abdallah@gmail.com";
		string adminPassword = "Abdallah123";

		if (!await roleManager.RoleExistsAsync("Admin"))
		{
			await roleManager.CreateAsync(new IdentityRole("Admin"));
		}
		if (!await roleManager.RoleExistsAsync("User"))
		{
			await roleManager.CreateAsync(new IdentityRole("User"));
		}

		var adminUser = await userManager.FindByEmailAsync(adminEmail);
		if (adminUser == null)
		{
			var user = new IdentityUser
			{
				UserName = adminEmail,
				Email = adminEmail,
				EmailConfirmed = true
			};

			await userManager.CreateAsync(user, adminPassword);
			await userManager.AddToRoleAsync(user, "Admin");
		}
	}
}
