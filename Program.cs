using Microsoft.EntityFrameworkCore;
using BumboSolid.Data;
using Microsoft.AspNetCore.Identity;
using BumboSolid.Data.Models;
using Authorisation.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connection = String.Empty;
if (builder.Environment.IsDevelopment())
{
	builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
	connection = builder.Configuration.GetConnectionString("LOCAL_CONNECTIONSTRING");
}
else
{
	connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
}

builder.Services.AddDbContext<BumboDbContext>(options =>
	options.UseSqlServer(connection));

// Configure Identity services
builder.Services.AddIdentity<Employee, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
}).AddEntityFrameworkStores<BumboDbContext>()
  .AddDefaultTokenProviders();
// Configure authentication cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Seed roles during application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<Employee>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    UserAndRoleSeeder.SeedData(userManager, roleManager);
}

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
