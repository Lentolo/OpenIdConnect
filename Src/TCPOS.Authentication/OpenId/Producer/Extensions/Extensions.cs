using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TCPOS.Authentication.Extensions;

namespace TCPOS.Authentication.OpenId.Producer.Extensions;

public static class Extensions
{
    public static void AddProducer(this IServiceCollection services, Action<Configuration.Configuration> action)
    {
        var configuration = new Configuration.Configuration();
        action(configuration);
        //check configuration
        services.AddSingleton(configuration);

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
                     options
                        .ChainIf(configuration.AllowAuthorizationCodeFlow, c =>
                         {
                             c.AllowAuthorizationCodeFlow()
                              .SetAuthorizationEndpointUris(configuration.AuthorizationEndpointUri)
                              .ChainIf(configuration.RequirePKCE, cc => cc.RequireProofKeyForCodeExchange());
                         })
                        .ChainIf(configuration.AllowClientCredentialsFlow, c =>
                         {
                             c.AllowClientCredentialsFlow()
                              .SetTokenEndpointUris(configuration.TokenEndpointUri);
                         });

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

    public static void UseProducer(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<Configuration.Configuration>();

        if (configuration.AllowAuthorizationCodeFlow)
        {
            app.MapPost(configuration.AuthorizationEndpointUri, Delegates.Delegates.Authorize);
            app.MapGet(configuration.AuthorizationEndpointUri, Delegates.Delegates.Authorize);
        }

        if (configuration.AllowClientCredentialsFlow || configuration.AllowAuthorizationCodeFlow)
        {
            app.MapPost(configuration.TokenEndpointUri, Delegates.Delegates.Exchange);
        }
    }
}
