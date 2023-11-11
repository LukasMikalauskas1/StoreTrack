using Microsoft.AspNetCore.Identity;
using StoreTrack.Auth.Model;

namespace StoreTrack.Auth
{
	public class AuthDbSeeder
	{
		private readonly UserManager<StoreRestUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public AuthDbSeeder(UserManager<StoreRestUser> userManager, RoleManager<IdentityRole> roleManager) 
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task SeedAsync()
		{
			await AddDefaultRoles();
			await AddAdminUser();
		}

		private async Task AddDefaultRoles()
		{
			foreach(var role in StoreRoles.All)
			{
				var roleExists = await _roleManager.RoleExistsAsync(role);
				if(!roleExists) 
				{
					await _roleManager.CreateAsync(new IdentityRole(role));
				}
			}
		}

		private async Task AddAdminUser()
		{
			var newAdminUser = new StoreRestUser()
			{
				UserName = "admin",
				Email = "admin@admin.com"
			};

			var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
			if(existingAdminUser == null)
			{
				var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "VerySafePassword1!");
				if(createAdminUserResult.Succeeded)
				{
					await _userManager.AddToRolesAsync(newAdminUser, StoreRoles.All);
				}
			}
		}





	}
}
