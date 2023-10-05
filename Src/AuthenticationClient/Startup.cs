using System;
using System.IO;
using System.Threading.Tasks;
using AuthenticationClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TCPOS.Authentication.Identity;
using TCPOS.Authentication.OpenId.Consumer.Extensions;

public static class Startup
{
    public static async Task ConfigureApp(WebApplication app)
    {
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

        app.MapControllerRoute("default",
                               "{controller=Home}/{action=Index}/{id?}");

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

    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.Services
               .AddIdentity<ApplicationUser, ApplicationRole>()
               .AddDefaultTokenProviders();

        builder.Services.AddTransient<IUserStore<ApplicationUser>, TestApplicationUserStore>();
        builder.Services.AddTransient<IUserPasswordStore<ApplicationUser>, TestApplicationUserStore>();
        builder.Services.AddTransient<IPasswordHasher<ApplicationUser>, TestPasswordHasher>();
        builder.Services.AddTransient<IRoleStore<ApplicationRole>, TestApplicationRoleStore>();
        // Add services to the container.
        builder.Services.AddControllersWithViews();

        //builder.Services.AddDbContext<DbContext>(options =>
        //{
        //    // Configure the context to use sqlite.
        //    options.UseSqlite($"Filename={Path.GetDirectoryName(typeof(Program).Assembly.Location)}\\db.sqlite");

        //    // Register the entity sets needed by OpenIddict.
        //    // Note: use the generic overload if you need
        //    // to replace the default OpenIddict entities.
        //    options.UseOpenIddict();
        //});

        builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
               .AddCookie(options =>
                {
                    options.LoginPath = "/auth/login";
                    options.LogoutPath = "/auth/logout";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
                    options.SlidingExpiration = false;
                });

        builder.Services.AddOpenIdConsumer(c =>
        {
            c.Issuer = new Uri("https://localhost:7177");
            c.ClientId = "test";
            c.ClientSecret = "test-test";
            c.LoginUri = new Uri("/auth/login", UriKind.Relative);
            c.CallBackUri = new Uri("/auth/login/callback", UriKind.Relative);
            c.Scopes.Add("api");
        });
    }
}
