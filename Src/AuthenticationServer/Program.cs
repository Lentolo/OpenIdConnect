using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using System.Threading;
using Microsoft.AspNetCore.Authentication.Cookies;
using TCPOS.Authentication.OpenId.Producer.Configuration;
using TCPOS.Authentication.OpenId.Producer.Extensions;

namespace AuthenticationServer;

//https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid
//https://documentation.openiddict.com/
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Startup.ConfigureBuilder(builder);

        var app = builder.Build();
        await Startup.ConfigureApp(app);

        await app.RunAsync();
    }
}
