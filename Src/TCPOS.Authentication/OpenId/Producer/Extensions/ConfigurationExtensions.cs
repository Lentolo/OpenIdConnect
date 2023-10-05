using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using TCPOS.Authentication.OpenId.Common;
using TCPOS.Authentication.OpenId.Producer.Configuration;
using TCPOS.Authentication.Utils.Extensions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TCPOS.Authentication.OpenId.Producer.Extensions;

//https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid
//https://documentation.openiddict.com/

public static class ConfigurationExtensions
{
    public static void AddOpenIdProducer(this IServiceCollection services, Action<Configuration.Configuration> action)
    {
        var configuration = new Configuration.Configuration();
        action(configuration);
        configuration.EnsureValid();
        services.AddSingleton(configuration);

        services.ConfigureServicesOpenIdDbContext(configuration.OpenIdDbContext);

        services.AddOpenIddict()
                 // Register the OpenIddict core components.
                .AddCore(options =>
                 {
                     options.ConfigureOpenIdCoreOpenIdDbContext(configuration.OpenIdDbContext);
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

                     var encryptionCertificate = GetCertificate(configuration?.EncryptionCertificate);

                     if (encryptionCertificate != null)
                     {
                         options.AddEncryptionCertificate(encryptionCertificate);
                     }
                     else
                     {
                         options.AddEphemeralEncryptionKey();
                     }

                     options.ChainIf(configuration?.DisableAccessTokenEncryption ?? false, o => o.DisableAccessTokenEncryption());

                     var signingCertificate = GetCertificate(configuration?.SigningCertificate);

                     if (signingCertificate != null)
                     {
                         options.AddSigningCertificate(signingCertificate!);
                     }
                     else
                     {
                         options.AddEphemeralSigningKey();
                     }

                     // Register scopes (permissions)
                     options.RegisterScopes(configuration.Applications.SelectMany(a => a.Scopes).Distinct().ToArray());

                     // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                     options.UseAspNetCore()
                            .EnableTokenEndpointPassthrough()
                            .EnableAuthorizationEndpointPassthrough();
                 });
    }

    private static X509Certificate2? GetCertificate(Certificate? certificate)
    {
        if (File.Exists(certificate?.PfxPath))
        {
            return new X509Certificate2(certificate.PfxPath!, certificate?.Password);
        }

        using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        return store.Certificates.FirstOrDefault(c =>
        {
            var aaa = c.HasPrivateKey;
            return !string.IsNullOrEmpty(certificate?.Thumbprint) && string.Compare(c.Thumbprint, certificate.Thumbprint, StringComparison.OrdinalIgnoreCase) == 0 || !string.IsNullOrEmpty(certificate?.Subject) && string.Compare(c.Subject, certificate.Subject, StringComparison.OrdinalIgnoreCase) == 0 || !string.IsNullOrEmpty(certificate?.FriendlyName) && string.Compare(c.FriendlyName, certificate.FriendlyName, StringComparison.OrdinalIgnoreCase) == 0;
        });
    }

    public static async Task<WebApplication> UseOpenIdProducer(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<Configuration.Configuration>();

        app.ConfigureAppOpenIdDbContext(configuration.OpenIdDbContext);

        app.ChainIf(configuration.AllowAuthorizationCodeFlow, a =>
        {
            a.MapPost(configuration.AuthorizationEndpointUri!.MakeAbsolute(new Uri("http://fake.host")).PathAndQuery, Delegates.Delegates.Authorize);
            a.MapGet(configuration.AuthorizationEndpointUri!.MakeAbsolute(new Uri("http://fake.host")).PathAndQuery, Delegates.Delegates.Authorize);
        });

        app.ChainIf(configuration.AllowClientCredentialsFlow || configuration.AllowAuthorizationCodeFlow, a =>
        {
            app.MapPost(configuration.TokenEndpointUri!.MakeAbsolute(new Uri("http://fake.host")).PathAndQuery, Delegates.Delegates.Token);
        });

        using var scope = app.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        foreach (var application in configuration.Applications)
        {
            var findByClientIdAsync = await manager.FindByClientIdAsync(application.ClientId);

            if (findByClientIdAsync is not null)
            {
                await manager.DeleteAsync(findByClientIdAsync);
            }

            var applicationDescriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = application.ClientId,
                ClientSecret = application.ClientSecret,
                DisplayName = application.DisplayName
            };
            applicationDescriptor.RedirectUris.UnionWith(application.RedirectUris);

            if (configuration.AllowAuthorizationCodeFlow)
            {
                applicationDescriptor.Permissions.Add(Permissions.ResponseTypes.Code);
                applicationDescriptor.Permissions.Add(Permissions.Endpoints.Authorization);
                applicationDescriptor.Permissions.Add(Permissions.Endpoints.Token);
                applicationDescriptor.Permissions.Add(Permissions.GrantTypes.AuthorizationCode);
            }

            if (configuration.AllowClientCredentialsFlow)
            {
                applicationDescriptor.Permissions.Add(Permissions.ResponseTypes.Token);
                applicationDescriptor.Permissions.Add(Permissions.Endpoints.Token);
                applicationDescriptor.Permissions.Add(Permissions.GrantTypes.ClientCredentials);
            }

            foreach (var s in application.Scopes)
            {
                applicationDescriptor.Permissions.Add(Permissions.Prefixes.Scope + s);
            }

            if (configuration.RequirePKCE)
            {
                applicationDescriptor.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
            }

            await manager.CreateAsync(applicationDescriptor);
        }

        return app;
    }
}
