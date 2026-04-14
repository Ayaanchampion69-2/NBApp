using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Models;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("NBAppContextConnection") ?? throw new InvalidOperationException("Connection string 'NBAppContextConnection' not found.");;

builder.Services.AddDbContext<NBAppContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<NBAppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<NBAppContext>();

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
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapStaticAssets();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
