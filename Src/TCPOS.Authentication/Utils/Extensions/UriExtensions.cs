namespace TCPOS.Authentication.Utils.Extensions;

public static class UriExtensions
{
    public static Uri MakeAbsolute(this Uri absoluteUri, Uri relativeUri)
    {
        Safety.Check(absoluteUri != null, () => new ArgumentNullException(nameof(absoluteUri)));
        Safety.Check(absoluteUri.IsAbsoluteUri, () => new ArgumentOutOfRangeException($"{nameof(absoluteUri)} must be absolute"));

        Safety.Check(relativeUri != null, () => new ArgumentNullException(nameof(relativeUri)));
        Safety.Check(!relativeUri.IsAbsoluteUri, () => new ArgumentOutOfRangeException($"{nameof(relativeUri)} must be relative"));

        return new Uri(absoluteUri, relativeUri);
    }

    public static bool IsRelativeWithAbsolutePath(this Uri uri)
    {
        Safety.Check(uri!= null, () => new ArgumentNullException(nameof(uri)));
        return !uri.IsAbsoluteUri && uri.OriginalString.StartsWith("/");
    }
}
