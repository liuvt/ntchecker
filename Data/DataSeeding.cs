using Microsoft.AspNetCore.Identity;
using ntchecker.Data.Models;

namespace ntchecker.Data
{
    public static class DataSeeding
    {
        public static async Task SeedIdentityDataAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // Các roles
            string[] roles = { "Owner", "Administrator", "Manager", "Driver" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed user + gán role
            async Task CreateUser(string id, string email, string password, string firstName, string lastName, string role)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new AppUser
                    {
                        Id = id,
                        UserName = email,
                        Email = email,
                        NormalizedUserName = email.ToUpper(),
                        NormalizedEmail = email.ToUpper(),
                        FirstName = firstName,
                        LastName = lastName,
                        PhoneNumber = "0868752251",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }

            await CreateUser("OWNER-NTGROUP-2025", "owner@namthanggroup.com", "Owner@123", "Lưu Văn", "Taxi", "Owner");
            await CreateUser("ADMIN-NTGROUP-2025", "administructor@namthanggroup.com", "Admin@123", "Nam Thắng", "Taxi", "Administrator");
            await CreateUser("MANAGER-NTGROUP-2025", "manager@namthanggroup.com", "Manager@123", "Điều Hành", "Taxi", "Manager");
            await CreateUser("DRIVER-NTGROUP-2025", "driver@namthanggroup.com", "Driver@123", "Lái xe", "Taxi", "Driver");
        }
    }
}
