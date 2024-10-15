using BumboSolid.Data;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			ConfigurationManager configuration = builder.Configuration;

			builder.Services.AddDbContext<BumboDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("BumboDBString")));

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
