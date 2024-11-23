using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Authorisation.Helpers
{
    public static class UserAndRoleSeeder
    {
        public static void SeedData(UserManager<Employee> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<Employee> userManager)
        {
            if (userManager.FindByNameAsync("Medewerker1").Result == null)
            {
                Employee user = new Employee();
                user.Email = "bla@bla.nl";
                user.UserName = "userBla";

                IdentityResult result = userManager.CreateAsync(user, "Welkom123!").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Medewerker").Wait();
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