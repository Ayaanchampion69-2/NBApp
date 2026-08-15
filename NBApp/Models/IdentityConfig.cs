using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace NBApp.Models
{
    public class IdentityConfig
    {
        public static async Task CreateAdminUserAsync(IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<NBAppUser>>();
            var config = provider.GetRequiredService<IConfiguration>();

            string username = config["AdminUser:Email"]!;
            string password = config["AdminUser:Password"]!;
            string roleName = "Admin";

            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            if (await userManager.FindByNameAsync(username) == null)
            {
                NBAppUser user = new()
                {
                    UserName = username,
                    Email = username,
                    EmailConfirmed = true,
                    DisplayName = "Admin",
                    ProfilePicturePath = "/Images/ProfilePictures/AdminPfp.png"
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
    }
}