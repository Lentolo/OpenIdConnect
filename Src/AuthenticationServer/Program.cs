namespace AuthenticationServer;

//https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Startup.ConfigureServices(builder.Services);

        var app = builder.Build();
        Startup.Configure(app, app.Environment);

        app.Run();
    }
}
