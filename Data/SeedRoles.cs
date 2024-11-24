using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Authorisation.Helpers
{
    public static class UserAndRoleSeeder
    {
        public static void SeedData(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByEmailAsync("manager@bumbo.nl").Result == null)
            {
                User user = new User
                {
                    Email = "manager@bumbo.nl",
                    FirstName = "Jan",
                    LastName = "De Groot",
                    BirthDate = new DateOnly(1990, 5, 15),
                    EmployedSince = DateOnly.FromDateTime(DateTime.Now),
                    PlaceOfResidence = "Amsterdam",
                    StreetName = "Onderwijsboulevard",
                    StreetNumber = 10,
                    UserName = "manager@bumbo.nl"
                };

                try
                {
                    IdentityResult result = userManager.CreateAsync(user, "Ab12345!").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Manager").Wait();
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating user: {ex.Message}");
                }
            }
        }


        private static void SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Employee").Result)
            {
                IdentityRole<int> role = new IdentityRole<int>();
                role.Name = "Employee";
                IdentityResult result = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Manager").Result)
            {
                IdentityRole<int> role = new IdentityRole<int>();
                role.Name = "Manager";
                IdentityResult result = roleManager.CreateAsync(role).Result;
            }
        }
    }
}