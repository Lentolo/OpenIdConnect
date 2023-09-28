using TCPOS.Authentication.OpenId.Common;
using TCPOS.Authentication.OpenId.Producer.Configuration;
using TCPOS.Authentication.Utils;
using TCPOS.Authentication.Utils.Extensions;

namespace TCPOS.Authentication.OpenId.Consumer.Configuration;

public class Configuration : ConfigurationBase
{
    internal Configuration()
    { }

    public Uri? Issuer
    {
        get;
        set;
    }

    public string ClientId
    {
        get;
        set;
    }

    public string ClientSecret
    {
        get;
        set;
    }

    public Uri? CallBackUri
    {
        get;
        set;
    }

    public Uri? LoginUri
    {
        get;
        set;
    }

    public HashSet<string> Scopes
    {
        get;
    } = new();

    internal override void EnsureValid()
    {
        base.EnsureValid();
        Safety.Check(Issuer?.IsAbsoluteUri ?? false, () => new ArgumentException($"{nameof(CallBackUri)} must be absolute"));

        Safety.Check(CallBackUri?.IsRelativeWithAbsolutePath() ?? false, () => new ArgumentException($"{nameof(CallBackUri)} must be relative with absolute path"));
        Safety.Check(LoginUri?.IsRelativeWithAbsolutePath() ?? false, () => new ArgumentException($"{nameof(LoginUri)} must be relative with absolute path"));

        Safety.Check(!string.IsNullOrEmpty(ClientId), () => new ArgumentException($"{nameof(ClientId)} must not be empty"));
        Safety.Check(!string.IsNullOrEmpty(ClientSecret), () => new ArgumentException($"{nameof(ClientSecret)} must not be empty"));
        Safety.Check(Scopes.Any() && Scopes.All(s => !string.IsNullOrEmpty(s)), () => new ArgumentException($"{nameof(Scopes)} must not be empty"));
    }
}
