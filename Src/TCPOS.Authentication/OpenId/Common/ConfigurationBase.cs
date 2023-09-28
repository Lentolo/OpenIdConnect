using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using TCPOS.Authentication.Utils;

[assembly: InternalsVisibleTo("TCPOS.Authentication.Tests")]

namespace TCPOS.Authentication.OpenId.Common;

public abstract class ConfigurationBase
{
    public Type? OpenIdDbContext
    {
        get;
        set;
    }
    internal virtual void EnsureValid()
    {
        Safety.Check(OpenIdDbContext == null || typeof(DbContext).IsAssignableFrom(OpenIdDbContext), () => new ArgumentException($"{nameof(OpenIdDbContext)} must inherit from {typeof(DbContext)}"));
    }
}
