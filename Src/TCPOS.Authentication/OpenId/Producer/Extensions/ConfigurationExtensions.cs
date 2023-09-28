using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using TCPOS.Authentication.OpenId.Producer.Configuration;
using TCPOS.Authentication.Utils;
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
        CheckConfiguration(configuration);
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

                     var encryptionCertificate = GetCertificate(configuration?.EncryptionCertificate);
                     var signingCertificate = GetCertificate(configuration?.SigningCertificate);

                     options
                        .ChainIf(encryptionCertificate != null, o => o.AddEncryptionCertificate(encryptionCertificate))
                        .ChainIf(signingCertificate != null, o => o.AddSigningCertificate(signingCertificate));

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
            return new X509Certificate2(certificate?.PfxPath, certificate?.Password);
        }

        using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        return store.Certificates.FirstOrDefault(c => !string.IsNullOrEmpty(certificate?.Thumbprint) && string.Compare(c.Thumbprint, certificate?.Thumbprint, StringComparison.OrdinalIgnoreCase) == 0 || !string.IsNullOrEmpty(certificate?.Subject) && string.Compare(c.Subject, certificate?.Subject, StringComparison.OrdinalIgnoreCase) == 0 || !string.IsNullOrEmpty(certificate?.FriendlyName) && string.Compare(c.FriendlyName, certificate?.FriendlyName, StringComparison.OrdinalIgnoreCase) == 0);
    }

    private static void CheckConfiguration(Configuration.Configuration configuration)
    {
        Safety.Check(configuration.AuthorizationEndpointUri != null, () => new ArgumentNullException(nameof(configuration.AuthorizationEndpointUri)));
        Safety.Check(configuration.AuthorizationEndpointUri.IsRelativeWithAbsolutePath(), () => new ArgumentException($"{nameof(configuration.AuthorizationEndpointUri)} must have an absolute path"));
        Safety.Check(configuration.TokenEndpointUri != null, () => new ArgumentNullException(nameof(configuration.TokenEndpointUri)));
        Safety.Check(configuration.TokenEndpointUri.IsRelativeWithAbsolutePath(), () => new ArgumentException($"{nameof(configuration.TokenEndpointUri)} must have an absolute path"));
        Safety.Check(configuration.Applications.Any(), () => new ArgumentException($"{nameof(configuration.Applications)} must be non empty"));

        foreach (var application in configuration.Applications)
        {
            Safety.Check(!string.IsNullOrEmpty(application.ClientSecret), () => new ArgumentNullException(nameof(application.ClientSecret)));
            Safety.Check(!string.IsNullOrEmpty(application.ClientId), () => new ArgumentNullException(nameof(application.ClientId)));
            Safety.Check(!string.IsNullOrEmpty(application.DisplayName), () => new ArgumentNullException(nameof(application.ClientId)));
            Safety.Check(application.RedirectUris.Any() && application.RedirectUris.All(u => u.IsAbsoluteUri), () => new ArgumentException($"{nameof(application.RedirectUris)} contains one or more relative uris"));
            Safety.Check(application.Scopes.Any() && application.Scopes.All(u => !string.IsNullOrEmpty(u)), () => new ArgumentException($"{nameof(application.Scopes)} contains one or more invalid scopes"));
        }
    }

    public static async Task<WebApplication> UseOpenIdProducer(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<Configuration.Configuration>();

        app.ChainIf(configuration.AllowAuthorizationCodeFlow, a =>
        {
            a.MapPost(configuration.AuthorizationEndpointUri!.MakeAbsolute(new Uri("http://fake.host")).PathAndQuery, Delegates.Delegates.Authorize);
            a.MapGet(configuration.AuthorizationEndpointUri!.MakeAbsolute(new Uri("http://fake.host")).PathAndQuery, Delegates.Delegates.Authorize);
        });

        app.ChainIf(configuration.AllowClientCredentialsFlow || configuration.AllowAuthorizationCodeFlow, a =>
        {
            app.MapPost(configuration.TokenEndpointUri!.MakeAbsolute(new Uri("http://fake.host")).PathAndQuery, Delegates.Delegates.Exchange);
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
            findByClientIdAsync = await manager.FindByClientIdAsync(application.ClientId);
        }

        return app;
    }
}
