namespace AuthenticationServer;

//https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid
//https://documentation.openiddict.com/
public class Program
{
    public static async Task Main(string[] args)
    {
        await (await WebApplication
                    .CreateBuilder(args)
                    .ConfigureBuilder()
                    .Build()
                    .ConfigureApp())
                    .RunAsync();
    }
}
