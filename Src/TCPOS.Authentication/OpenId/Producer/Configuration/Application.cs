namespace TCPOS.Authentication.OpenId.Producer.Configuration;

public sealed class Application
{
    internal Application()
    { }

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

    public string DisplayName
    {
        get;
        set;
    }
    public string RedirectUri
    {
        get;
        set;
    }
    public string Scope
    {
        get;
        set;
    }
}
