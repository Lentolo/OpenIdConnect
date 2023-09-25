namespace TCPOS.Authentication.OpenId.Producer.Configuration;

public sealed class Configuration
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

    public Uri AuthorizationEndpointUri
    {
        get;
        set;
    } 

    public Uri TokenEndpointUri
    {
        get;
        set;
    } 

    public HashSet<Application> Applications
    {
        get;
    } = new();
}
