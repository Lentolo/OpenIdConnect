using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TCPOS.Authentication.OpenId.Consumer.Extensions;

namespace AuthenticationClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<DbContext>(options =>
            {
                // Configure the context to use sqlite.
                options.UseSqlite($"Filename={Path.GetDirectoryName(typeof(Startup).Assembly.Location)}\\db.sqlite");

                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();
            });
            builder.Services.AddControllersWithViews();

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

            builder.Services.AddOpenIdConsumer(c=>{
                c.Issuer = new Uri("https://localhost:7177");
                c.ClientId = "test";
                c.ClientId = "test-test";
                c.LoginUri = new Uri("/auth/login", UriKind.Relative);
                c.CallBackUri= new Uri("/auth/login/callback", UriKind.Relative);
            });
            var app = builder.Build();

            using var scope=app.Services.CreateScope();
            await scope.ServiceProvider.GetService<DbContext>()!.Database.EnsureCreatedAsync();

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

            app.Run();
        }
    }
}