using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TCPOS.Authentication.OpenId.Producer.Configuration;
using TCPOS.Authentication.OpenId.Producer.Extensions;

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
        services.AddOpenIdProducer(c =>
        {
            c.AllowAuthorizationCodeFlow = true;
            c.AuthorizationEndpointUri = new Uri("/auth/connect/authorize",UriKind.Relative);
            c.RequirePKCE = true;

            c.AllowClientCredentialsFlow = true;
            c.TokenEndpointUri = new Uri("/auth/connect/token",UriKind.Relative);

            var application = new Application
            {
                Scope = "api",
                ClientId = "test",
                ClientSecret = "test-test",
                DisplayName = "Test"
            };
            application.RedirectUris.Add(new Uri("https://localhost:7290/auth/login/callback"));
            c.Applications.Add(application);
        });
    }

    public static async Task Configure(WebApplication app)
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

        await app.UseOpenIdProducer();
    }
}
