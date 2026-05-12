using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<NBAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NBAppContextConnection")));

//DI for DbContext
builder.Services.AddDefaultIdentity<NBAppUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<NBAppContext>();
    

// Add Session State with server memory (distributed memory cache)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddControllersWithViews();

/*{
    var app = CreateHostBuilder(args).Build();

    CreateDbIfNotExists(app);

    app.Run();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<NBAppContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
;*/

// This automatically scans your project and registers all validators it finds
builder.Services
    .AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<NBAppContext>();
    context.Database.Migrate(); // Apply any pending migrations
    DbInitializer.Initialize(context);
}


app.UseStaticFiles();
app.UseSession(); // Enable Session State middleware

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapRazorPages(); // Required for Identity UI




app.Run();