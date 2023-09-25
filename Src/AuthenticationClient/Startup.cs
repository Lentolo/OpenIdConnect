using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Client;
using TCPOS.Authentication.OpenId.Consumer;
using TCPOS.Authentication.OpenId.Consumer.Extensions;

namespace AuthenticationClient;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<DbContext>(options =>
        {
            // Configure the context to use sqlite.
            options.UseSqlite($"Filename={Path.GetDirectoryName(typeof(Startup).Assembly.Location)}\\db.sqlite");

            // Register the entity sets needed by OpenIddict.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
            options.UseOpenIddict();
        });
        services.AddControllersWithViews();

        services.AddAuthentication(options =>
                 {
                     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                 })
                .AddCookie(options =>
                 {
                     options.LoginPath = "/login";
                     options.LogoutPath = "/logout";
                     options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
                     options.SlidingExpiration = false;
                 });

        //services.AddDbContext<DbContext>(options =>
        //{
        //    // Configure the context to use an in-memory store.
        //    options.UseInMemoryDatabase(nameof(DbContext));

        //    // Register the entity sets needed by OpenIddict.
        //    options.UseOpenIddict();
        //});
        services.AddOpenIdConsumer();
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

        app.UseOpenIdConsumer();
    }
}
