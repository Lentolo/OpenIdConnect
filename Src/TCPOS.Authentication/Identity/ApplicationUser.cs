namespace TCPOS.Authentication.Identity;

public class ApplicationUser
{
    public string UserId
    {
        get;
        set;
    }

    public string UserName
    {
        get;
        set;
    }
    public string PasswordHash
    {
        get;
        set;
    }
}
