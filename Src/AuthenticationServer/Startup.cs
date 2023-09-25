using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

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
        services.AddOpenIddict()

                 // Register the OpenIddict core components.
                .AddCore(options =>
                 {
                     // Configure OpenIddict to use the EF Core stores/models.
                     options.UseEntityFrameworkCore()
                            .UseDbContext<DbContext>();
                 })

                 // Register the OpenIddict server components.
                .AddServer(options =>
                 {
                     options.AllowAuthorizationCodeFlow()
                            .RequireProofKeyForCodeExchange();

                     options.AllowClientCredentialsFlow();

                     options.SetAuthorizationEndpointUris("/connect/authorize")
                            .SetTokenEndpointUris("/connect/token");

                     // Encryption and signing of tokens
                     options.AddEphemeralEncryptionKey()
                            .AddEphemeralSigningKey();

                     // Disable encryption
                     options.DisableAccessTokenEncryption();

                     // Register scopes (permissions)
                     options.RegisterScopes("api");

                     // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                     options.UseAspNetCore()
                            .EnableTokenEndpointPassthrough()
                            .EnableAuthorizationEndpointPassthrough();
                 });
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
    }
}
