namespace TCPOS.Authentication.OpenId.Consumer.Configuration;

public class Configuration
{
    internal Configuration()
    {}

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

    public Uri?  CallBackUri
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
    } = new HashSet<string>();
}
