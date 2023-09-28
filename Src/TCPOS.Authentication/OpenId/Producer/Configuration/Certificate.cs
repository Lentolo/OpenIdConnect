namespace TCPOS.Authentication.OpenId.Producer.Configuration;

public class Certificate
{
    internal Certificate()
    { }

    public string? Thumbprint
    {
        get;
        set;
    }

    public string? Subject
    {
        get;
        set;
    }

    public string? FriendlyName
    {
        get;
        set;
    }
    public string? PfxPath
    {
        get;
        set;
    }
    public string? Password
    {
        get;
        set;
    }
}
