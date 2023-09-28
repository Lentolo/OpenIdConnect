using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Client;
using TCPOS.Authentication.OpenId.Common;
using TCPOS.Authentication.OpenId.Producer.Extensions;
using TCPOS.Authentication.Utils;
using TCPOS.Authentication.Utils.Extensions;

namespace TCPOS.Authentication.OpenId.Consumer.Extensions;

public static class ConfigurationExtensions
{
    public static void AddOpenIdConsumer(this IServiceCollection services, Action<Configuration.Configuration> action)
    {
        var configuration = new Configuration.Configuration();
        action(configuration);
        configuration.EnsureValid();
        services.AddSingleton(configuration);

        services.AddOpenIddict()
                 // Register the OpenIddict core components.
                .AddCore(options =>
                 {
                     options.ConfigureOpenIdCoreOpenIdDbContext(configuration.OpenIdDbContext);
                 })
                .AddClient(options =>
                 {
                     // Note: this sample uses the code flow, but you can enable the other flows if necessary.
                     options.AllowAuthorizationCodeFlow();

                     // Register the signing and encryption credentials used to protect
                     // sensitive data like the state tokens produced by OpenIddict.
                     options.AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();

                     // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                     options.UseAspNetCore()
                            .EnableStatusCodePagesIntegration()
                            .EnableRedirectionEndpointPassthrough()
                         //.EnablePostLogoutRedirectionEndpointPassthrough()
                         ;

                     // Register the System.Net.Http integration and use the identity of the current
                     // assembly as a more specific user agent, which can be useful when dealing with
                     // providers that use the user agent as a way to throttle requests (e.g Reddit).
                     options.UseSystemNetHttp()
                            .SetProductInformation(typeof(ConfigurationExtensions).Assembly);

                     // Add a client registration matching the client application definition in the server project.
                     var registration = new OpenIddictClientRegistration
                     {
                         Issuer = configuration.Issuer,

                         ClientId = configuration.ClientId,
                         ClientSecret = configuration.ClientSecret,

                         // Note: to mitigate mix-up attacks, it's recommended to use a unique redirection endpoint
                         // URI per provider, unless all the registered providers support returning a special "iss"
                         // parameter containing their URL as part of authorization responses. For more information,
                         // see https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics#section-4.4.
                         RedirectUri = configuration.CallBackUri
                         //PostLogoutRedirectUri = new Uri("callback/logout/local", UriKind.Relative)
                     };
                     registration.Scopes.UnionWith(configuration.Scopes);
                     options.AddRegistration(registration);
                 });
    }

    public static void UseOpenIdConsumer(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<Configuration.Configuration>();
        app.MapGet(configuration.LoginUri!.MakeAbsolute(new Uri("http://fake.host")).PathAndQuery, Delegates.Delegates.Login);
        app.MapGet(configuration.CallBackUri!.MakeAbsolute(new Uri("http://fake.host")).PathAndQuery, Delegates.Delegates.LoginCallback);
    }
}
