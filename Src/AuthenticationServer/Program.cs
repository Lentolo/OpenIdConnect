using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using System.Threading;

namespace AuthenticationServer;

//https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Startup.ConfigureServices(builder.Services);

        var app = builder.Build();
        Startup.Configure(app);

        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<DbContext>();
        await context.Database.EnsureCreatedAsync();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync("postman") is null)
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
            });
        }

        app.Run();
    }
}
