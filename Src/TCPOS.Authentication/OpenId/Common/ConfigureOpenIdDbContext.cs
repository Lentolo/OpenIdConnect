using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TCPOS.Authentication.Utils.Extensions;

namespace TCPOS.Authentication.OpenId.Common;

internal static class ConfigureOpenIdDbContext
{
    public static void ConfigureOpenIdCoreOpenIdDbContext(this OpenIddictCoreBuilder options, Type? openIdDbContext)
    {
        if (openIdDbContext != null)
        {
            // Configure OpenIddict to use the EF Core stores/models.
            options
               .UseEntityFrameworkCore()
               .UseDbContext(openIdDbContext);
        }
        else
        {
            // Configure OpenIddict to use the EF Core stores/models.
            options
               .UseEntityFrameworkCore()
               .UseDbContext<DbContext>();
        }
    }

    public static void ConfigureServicesOpenIdDbContext(this IServiceCollection services, Type? openIdDbContext)
    {
        if (openIdDbContext == null)
        {
            services.AddDbContext<DbContext>(o =>
            {
                o.UseInMemoryDatabase("OpenID");
                o.UseOpenIddict();
            });
        }
    }

    public static void ConfigureAppOpenIdDbContext(this WebApplication app, Type? openIdDbContext)
    {
        app.ChainIf(openIdDbContext == null, a =>
        {
            using var scope = app.Services.CreateScope();
            var inMemoryDbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
            inMemoryDbContext.Database.EnsureDeleted();
            inMemoryDbContext.Database.EnsureCreated();
        });
    }
}