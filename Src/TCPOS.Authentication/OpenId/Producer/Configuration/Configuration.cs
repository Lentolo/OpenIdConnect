using Microsoft.Extensions.Configuration;
using TCPOS.Authentication.OpenId.Common;
using TCPOS.Authentication.OpenId.Consumer.Configuration;
using TCPOS.Authentication.Utils;
using TCPOS.Authentication.Utils.Extensions;

namespace TCPOS.Authentication.OpenId.Producer.Configuration;

public sealed class Configuration: ConfigurationBase
{
    internal Configuration()
    { }

    public bool AllowClientCredentialsFlow
    {
        get;
        set;
    } = false;

    public bool AllowAuthorizationCodeFlow
    {
        get;
        set;
    } = false;

    public bool RequirePKCE
    {
        get;
        set;
    } = false;

    public Uri? AuthorizationEndpointUri
    {
        get;
        set;
    }

    public Uri? TokenEndpointUri
    {
        get;
        set;
    }

    public Certificate SigningCertificate
    {
        get;
    } = new();

    public Certificate EncryptionCertificate
    {
        get;
    } = new();

    public bool DisableAccessTokenEncryption
    {
        get;
        set;
    } = false;

    public HashSet<Application> Applications
    {
        get;
    } = new();

    internal override void EnsureValid()
    {
        base.EnsureValid();
        Safety.Check(AuthorizationEndpointUri != null, () => new ArgumentNullException(nameof(AuthorizationEndpointUri)));
        Safety.Check(AuthorizationEndpointUri.IsRelativeWithAbsolutePath(), () => new ArgumentException($"{nameof(AuthorizationEndpointUri)} must have an absolute path"));
        Safety.Check(TokenEndpointUri != null, () => new ArgumentNullException(nameof(TokenEndpointUri)));
        Safety.Check(TokenEndpointUri.IsRelativeWithAbsolutePath(), () => new ArgumentException($"{nameof(TokenEndpointUri)} must have an absolute path"));
        Safety.Check(Applications.Any(), () => new ArgumentException($"{nameof(Applications)} must be non empty"));

        foreach (var application in Applications)
        {
            Safety.Check(!string.IsNullOrEmpty(application.ClientSecret), () => new ArgumentNullException(nameof(application.ClientSecret)));
            Safety.Check(!string.IsNullOrEmpty(application.ClientId), () => new ArgumentNullException(nameof(application.ClientId)));
            Safety.Check(!string.IsNullOrEmpty(application.DisplayName), () => new ArgumentNullException(nameof(application.ClientId)));
            Safety.Check(application.RedirectUris.Any() && application.RedirectUris.All(u => u.IsAbsoluteUri), () => new ArgumentException($"{nameof(application.RedirectUris)} contains one or more relative uris"));
            Safety.Check(application.Scopes.Any() && application.Scopes.All(u => !string.IsNullOrEmpty(u)), () => new ArgumentException($"{nameof(application.Scopes)} contains one or more invalid scopes"));
        }
    }
}