using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
    
namespace AuthenticationServer;

public class TestData : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public TestData(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync("postman", cancellationToken) is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "postman",   
                ClientSecret = "postman-secret",
                DisplayName = "Postman",
                RedirectUris={
                    new Uri("https://localhost:7290/callback/login/local")
                },
                Permissions =
                {
                    OpenIddictConstants.Permissions.ResponseTypes.Code,

                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                }
            }, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}