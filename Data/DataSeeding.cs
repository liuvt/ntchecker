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
                        UserName = email.Replace("@namthanggroup.com",""),
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
            await CreateUser("BACLIEU-NTGROUP-2025", "ntbaclieu@namthanggroup.com", "Ntbaclieu@123", "Điều Hành", "BẠC LIÊU", "Manager");
            await CreateUser("RACHGIA-NTGROUP-2025", "ntrachgia@namthanggroup.com", "Ntrachgia@123", "Điều Hành", "RẠCH GIÁ", "Manager");
            await CreateUser("PHUQUOC-NTGROUP-2025", "ntphuquoc@namthanggroup.com", "Ntphuquoc@123", "Điều Hành", "PHÚ QUỐC", "Manager");
            await CreateUser("SOCTRANG-NTGROUP-2025", "ntsoctrang@namthanggroup.com", "Ntsoctrang@123", "Điều Hành", "SÓC TRĂNG", "Manager");
            await CreateUser("VINHLONG-NTGROUP-2025", "ntvinhlong@namthanggroup.com", "Ntvinhlong@123", "Điều Hành", "VĨNH LONG", "Manager");
            await CreateUser("CAMAU-NTGROUP-2025", "ntcamau@namthanggroup.com", "Ntcamau@123", "Điều Hành", "CÀ MAU", "Manager");
            await CreateUser("HAUGIANG-NTGROUP-2025", "nthaugiang@namthanggroup.com", "Nthaugiang@123", "Điều Hành", "HẬU GIANG", "Manager");
            await CreateUser("ANGIANG-NTGROUP-2025", "ntangiang@namthanggroup.com", "Ntangiang@123", "Điều Hành", "AN GIANG", "Manager");
            await CreateUser("CONDAO-NTGROUP-2025", "ntcondao@namthanggroup.com", "Ntcondao@123", "Điều Hành", "CÔN ĐẢO", "Manager");
            await CreateUser("DRIVER-NTGROUP-2025", "driver@namthanggroup.com", "Driver@123", "Lái xe", "Taxi", "Driver");
        }
    }
}
