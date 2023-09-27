using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TCPOS.Authentication.OpenId.Consumer.Extensions;

namespace AuthenticationClient
{
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
}