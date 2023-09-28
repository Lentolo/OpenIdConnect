namespace TCPOS.Authentication.Utils.Extensions;

internal static class UriExtensions
{
    public static Uri MakeAbsolute(this Uri uri1, Uri uri2)
    {
        Safety.Check(uri1 != null, () => new ArgumentNullException(nameof(uri1)));
        Safety.Check(uri2 != null, () => new ArgumentNullException(nameof(uri2)));
        Safety.Check(uri1.IsAbsoluteUri && !uri2.IsAbsoluteUri
                     || !uri1.IsAbsoluteUri && uri2.IsAbsoluteUri, () => new ArgumentOutOfRangeException($"{nameof(uri1)}, {nameof(uri2)} one must be absolute, the other relative"));

        return uri1.IsAbsoluteUri ? new Uri(uri1, uri2) : new Uri(uri2, uri1);
    }

    public static bool IsRelativeWithAbsolutePath(this Uri uri)
    {
        Safety.Check(uri != null, () => new ArgumentNullException(nameof(uri)));
        return !uri.IsAbsoluteUri && uri.OriginalString.StartsWith("/");
    }
}
