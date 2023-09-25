namespace TCPOS.Authentication.OpenId.Consumer.Configuration;

public class Configuration
{
    internal Configuration()
    {}
    public bool AllowClientCredentialsFlow
    {
        get;
        set;
    }
    public bool AllowAuthorizationCodeFlow
    {
        get;
        set;
    }
    public bool RequirePKCE
    {
        get;
        set;
    }
    public string AuthorizationEndpointUri
    {
        get;
        set;
    }
    public string TokenEndpointUri
    {
        get;
        set;
    }
}
