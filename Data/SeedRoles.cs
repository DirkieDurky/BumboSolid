using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
				User manager = new User
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
					IdentityResult result = userManager.CreateAsync(manager, "Ab12345!").Result;
					if (result.Succeeded)
					{
						userManager.AddToRoleAsync(manager, "Manager").Wait();
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

			if (userManager.FindByEmailAsync("pieterdeklein@gmail.com").Result == null)
			{
				User employee = new User
				{
					Email = "pieterdeklein@gmail.com",
					FirstName = "Pieter",
					LastName = "De Klein",
					PlaceOfResidence = "Aalsmeer",
					StreetName = "Aalweg",
					StreetNumber = 2,
					BirthDate = new DateOnly(1990, 6, 18),
					EmployedSince = new DateOnly(2016, 7, 12),
					UserName = "pieterdeklein@gmail.com",
					AvailabilityRules = new List<AvailabilityRule>()
					{
						new AvailabilityRule()
						{
							Date = new DateOnly(2024, 12, 9),
							StartTime = new TimeOnly(8,0),
							EndTime = new TimeOnly(20,0),
							Available = 1,
							School = 0,
						}
					}
				};

				try
				{
					IdentityResult result = userManager.CreateAsync(employee, "Ab12345!").Result;
					if (result.Succeeded)
					{
						userManager.AddToRoleAsync(employee, "Employee").Wait();
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