using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Authorisation.Helpers
{
    public static class UserAndRoleSeeder
    {
        public static void SeedData(UserManager<Employee> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            SeedRoles(roleManager);
            //SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<Employee> userManager)
        {
            if (userManager.FindByEmailAsync("medewerker1@bumbo.nl").Result == null)
            {
                Employee user = new Employee
                {
                    Email = "medewerker1@bumbo.nl",
                    FirstName = "Jan",
                    LastName = "De Groot",
                    BirthDate = new DateOnly(1990, 5, 15),
                    EmployedSince = DateOnly.FromDateTime(DateTime.Now),
                    PlaceOfResidence = "Amsterdam",
                    StreetName = "Main Street",
                    StreetNumber = 10,
                    UserName = "medewerker1@bumbo.nl"
                };

                try
                {
                    IdentityResult result = userManager.CreateAsync(user, "Ab12345!").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Medewerker").Wait();
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
            if (!roleManager.RoleExistsAsync("Medewerker").Result)
            {
                IdentityRole<int> role = new IdentityRole<int>();
                role.Name = "Medewerker";
                IdentityResult result = roleManager.CreateAsync(role).Result;
            }
        }
    }
}