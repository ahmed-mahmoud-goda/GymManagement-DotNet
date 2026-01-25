using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymManagementDAL.Data.DataSeed
{
    public class IdentityDataSeeding
    {
        public static bool SeedData(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            try
            {
                if (!roleManager.Roles.Any())
                {
                    var roles = new List<IdentityRole>()
                {
                    new IdentityRole(){Name="SuperAdmin"},
                    new IdentityRole(){Name="Admin"}
                };

                    foreach (var role in roles)
                    {
                        if (!roleManager.RoleExistsAsync(role.Name).Result)
                        {
                            roleManager.CreateAsync(role).Wait();
                        }
                    }
                }
                if (!userManager.Users.Any())
                {
                    var superAdmin = new ApplicationUser
                    {
                        FirstName = "Ahmed",
                        LastName = "Ali",
                        UserName = "AhmedAli",
                        Email = "superadmin@gmail.com",
                        PhoneNumber = "011234567789"
                    };
                    userManager.CreateAsync(superAdmin, "Password@123").Wait();
                    userManager.AddToRoleAsync(superAdmin, "SuperAdmin").Wait();

                    var admin = new ApplicationUser
                    {
                        FirstName = "Ahmed",
                        LastName = "Hassan",
                        UserName = "AhmedHassan",
                        Email = "admin@gmail.com",
                        PhoneNumber = "010234567789"
                    };
                    userManager.CreateAsync(admin, "Password@123").Wait();
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding Failed: {ex}");
                return false;
            }
        }
    }
}
