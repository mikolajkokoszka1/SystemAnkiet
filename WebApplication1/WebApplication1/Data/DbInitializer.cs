using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = "admin@ankiety.pl";
            var adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(newAdmin, adminPassword);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

            var userEmail = "user@ankiety.pl";
            var userPassword = "User123!";

            var normalUser = await userManager.FindByEmailAsync(userEmail);
            if (normalUser == null)
            {
                var newUser = new IdentityUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };

                var createUser = await userManager.CreateAsync(newUser, userPassword);
                if (createUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "User");
                }
            }
        }
    }
}
