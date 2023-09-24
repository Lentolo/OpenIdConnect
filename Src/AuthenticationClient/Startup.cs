using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Client;

namespace AuthenticationClient;

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

        //services.AddDbContext<DbContext>(options =>
        //{
        //    // Configure the context to use an in-memory store.
        //    options.UseInMemoryDatabase(nameof(DbContext));

        //    // Register the entity sets needed by OpenIddict.
        //    options.UseOpenIddict();
        //});
        services.AddOpenIddict()
                 // Register the OpenIddict core components.
                .AddCore(options =>
                 {
                     // Configure OpenIddict to use the EF Core stores/models.
                     //options.UseEntityFrameworkCore()
                     //       .UseDbContext<DbContext>();
                 })
                .AddClient(options =>
                 {
                     // Note: this sample uses the code flow, but you can enable the other flows if necessary.
                     options.AllowAuthorizationCodeFlow();

                     // Register the signing and encryption credentials used to protect
                     // sensitive data like the state tokens produced by OpenIddict.
                     //options.AddDevelopmentEncryptionCertificate()
                     //       .AddDevelopmentSigningCertificate();

                     // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                     options.UseAspNetCore()
                            .EnableStatusCodePagesIntegration()
                            .EnableRedirectionEndpointPassthrough()
                         //.EnablePostLogoutRedirectionEndpointPassthrough()
                         ;

                     // Register the System.Net.Http integration and use the identity of the current
                     // assembly as a more specific user agent, which can be useful when dealing with
                     // providers that use the user agent as a way to throttle requests (e.g Reddit).
                     //options.UseSystemNetHttp()
                     //       .SetProductInformation(typeof(Startup).Assembly);

                     // Add a client registration matching the client application definition in the server project.
                     options.AddRegistration(new OpenIddictClientRegistration
                     {
                         Issuer = new Uri("https://localhost:44394/", UriKind.Absolute),

                         ClientId = "postman",
                         ClientSecret = "postman-secret",
                         Scopes =
                         {
                             "api"
                         },

                         // Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
                         // URI per provider, unless all the registered providers support returning a special "iss"
                         // parameter containing their URL as part of authorization responses. For more information,
                         // see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
                         RedirectUri = new Uri("callback/login/local", UriKind.Relative)
                         //PostLogoutRedirectUri = new Uri("callback/logout/local", UriKind.Relative)
                     });
                 });
    }

    public static void Configure(WebApplication app, IWebHostEnvironment env)
    {
        //app.MapGet("/", () => "Hello World!");
        if (env.IsDevelopment())
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
