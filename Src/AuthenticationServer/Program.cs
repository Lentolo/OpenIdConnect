using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using System.Threading;

namespace AuthenticationServer;

//https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid
//https://documentation.openiddict.com/
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

        app.Run();
    }
}
