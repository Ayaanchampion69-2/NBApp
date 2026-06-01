
using Microsoft.AspNetCore.Identity;

namespace NBApp.Models
{
    public class IdentityConfig
    {
        public static async Task CreateAdminUserAsync(IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<NBAppUser>>();

            string username = "admin@DaGoat.com";
            string password = "MessiDaGoat10!";
            string roleName = "Admin";

            // Check if the role exists, if not, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            if(await userManager.FindByNameAsync(username) == null)
            {
                NBAppUser user = new () { UserName = username, Email = "admin@DaGoat.com", EmailConfirmed = true, DisplayName = "Admin" };
                var result = await userManager.CreateAsync(user, password);
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }   
    }
}
