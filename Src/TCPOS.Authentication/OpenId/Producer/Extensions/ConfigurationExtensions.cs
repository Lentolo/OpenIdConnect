using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using TCPOS.Authentication.Extensions;

namespace TCPOS.Authentication.OpenId.Producer.Extensions;

//https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid
//https://documentation.openiddict.com/

public static class ConfigurationExtensions
{
    public static void AddOpenIdProducer(this IServiceCollection services, Action<Configuration.Configuration> action)
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

    public static async Task<WebApplication> UseOpenIdProducer(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<Configuration.Configuration>();

        app.ChainIf(configuration.AllowAuthorizationCodeFlow, a =>
        {
            a.MapPost(configuration.AuthorizationEndpointUri.PathAndQuery, Delegates.Delegates.Authorize);
            a.MapGet(configuration.AuthorizationEndpointUri.PathAndQuery, Delegates.Delegates.Authorize);
        });

        app.ChainIf(configuration.AllowClientCredentialsFlow || configuration.AllowAuthorizationCodeFlow, a =>
        {
            app.MapPost(configuration.TokenEndpointUri.PathAndQuery, Delegates.Delegates.Exchange);
        });

        using var scope = app.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        foreach (var application in configuration.Applications)
        {
            if (await manager.FindByClientIdAsync(application.ClientId) is null)
            {
                var applicationDescriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = application.ClientId,
                    ClientSecret = application.ClientSecret,
                    DisplayName = application.DisplayName
                };
                applicationDescriptor.RedirectUris.Union(application.RedirectUris);

                if (configuration.AllowAuthorizationCodeFlow)
                {
                    applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                    applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                    applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                }

                if (configuration.AllowClientCredentialsFlow)
                {
                    applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
                    applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
                }

                await manager.CreateAsync(applicationDescriptor);
            }
        }

        return app;
    }
}
