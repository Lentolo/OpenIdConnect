using Microsoft.EntityFrameworkCore;
using TCPOS.Authentication.Utils;

namespace TCPOS.Authentication.OpenId.Producer.Configuration;

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
