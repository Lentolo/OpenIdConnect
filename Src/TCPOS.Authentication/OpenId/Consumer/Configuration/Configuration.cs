namespace TCPOS.Authentication.OpenId.Consumer.Configuration;

public class Configuration
{
    internal Configuration()
    {}

    public string Issuer
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

    public string CallBackUri
    {
        get;
        set;
    }

    public string LoginUri
    {
        get;
        set;
    }
}
