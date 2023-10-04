using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TCPOS.Authentication.OpenId.Producer.Configuration;
using TCPOS.Authentication.OpenId.Producer.Extensions;

namespace AuthenticationServer;

public static class Startup
{
    public static async Task<WebApplication> ConfigureApp(this WebApplication app)
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
        return app;
    }

    public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
    {
        // Add identity types
        builder.Services
               .AddIdentity<ApplicationUser, ApplicationRole>()
               .AddDefaultTokenProviders();
        builder.Services.AddTransient<IUserStore<ApplicationUser>, TestApplicationUserStore>();
        builder.Services.AddTransient<IUserPasswordStore<ApplicationUser>, TestApplicationUserStore>();
        builder.Services.AddTransient<IPasswordValidator<ApplicationUser>, TestPasswordValidator>();
        builder.Services.AddTransient<IRoleStore<ApplicationRole>, TestApplicationRoleStore>();

        builder.Services.AddControllersWithViews();
        builder.Services
               .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/account/login";
                });

        builder.Services.AddDbContext<DbContext>(options =>
        {
            // Configure the context to use sqlite.
            options.UseSqlite($"Filename={Path.GetDirectoryName(typeof(Program).Assembly.Location)}\\db.sqlite");

            // Register the entity sets needed by OpenIddict.
            options.UseOpenIddict();
        });
        builder.Services.AddOpenIdProducer(c =>
        {
            c.OpenIdDbContext = typeof(DbContext);
            c.AllowAuthorizationCodeFlow = true;
            c.AuthorizationEndpointUri = new Uri("/auth/connect/authorize", UriKind.Relative);
            c.RequirePKCE = true;

            c.AllowClientCredentialsFlow = true;
            c.TokenEndpointUri = new Uri("/auth/connect/token", UriKind.Relative);

            var application = new Application
            {
                Scopes = { "api" },
                ClientId = "test",
                ClientSecret = "test-test",
                DisplayName = "Test"
            };

            //c.EncryptionCertificate.Subject = "CN=odata-enc";
            c.SigningCertificate.Subject = "CN=odata-sign";

            application.RedirectUris.Add(new Uri("https://localhost:7290/auth/login/callback"));
            c.Applications.Add(application);
        });
        return builder;
    }
}
