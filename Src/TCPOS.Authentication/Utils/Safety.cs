using System.Diagnostics.CodeAnalysis;

namespace TCPOS.Authentication.Utils;

internal static class Safety
{
    public static void Check([DoesNotReturnIf(false)] bool condition, Func<Exception> exceptionFactory)
    {
        if (!condition)
        {
            throw exceptionFactory();
        }
    }
}