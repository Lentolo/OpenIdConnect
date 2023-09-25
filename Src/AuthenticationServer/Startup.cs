using AuthenticationServer.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TCPOS.Authentication.OpenId.Producer;

namespace AuthenticationServer;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                 {
                     options.LoginPath = "/account/login";
                 });

        services.AddDbContext<DbContext>(options =>
        {
            // Configure the context to use sqlite.
            options.UseSqlite($"Filename={Path.GetDirectoryName(typeof(Startup).Assembly.Location)}\\db.sqlite");

            // Register the entity sets needed by OpenIddict.
            options.UseOpenIddict();
        });
        services.AddProducer();
    }

    public static void Configure(WebApplication app)
    {
        //app.MapGet("/", () => "Hello World!");
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });

        app.UseProducer();
    }
}
