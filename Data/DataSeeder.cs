using Auction.MyConstants;
using Microsoft.AspNetCore.Identity;

namespace Auction.Data
{
    public class DataSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));




            //creating Default Admin
            var user = new ApplicationUser
            {
                UserName = "hassankhan032370@gmail.com",
                Email = "hassankhan032370@gmail.com",
                Name = "Hassan",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "Admin@123");
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }

        }
    }
}
