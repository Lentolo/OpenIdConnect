namespace TCPOS.Authentication.OpenId.Producer.Configuration;

public sealed class Application : IEquatable<Application>
{
    public Application()
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

    public HashSet<Uri> RedirectUris
    {
        get;
    } = new();

    public string Scope
    {
        get;
        set;
    }

    public bool Equals(Application? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Compare(ClientId, other.ClientId, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Application other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ClientId);
    }

    public static bool operator ==(Application? left, Application? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Application? left, Application? right)
    {
        return !Equals(left, right);
    }
}
