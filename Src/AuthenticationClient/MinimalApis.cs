using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;

public static class MinimalApis
{
    public static async Task Login(HttpContext ctx, [FromQuery] string? returnUrl)
    {
        var properties = new AuthenticationProperties
        {
            // Only allow local return URLs to prevent open redirect attacks.
            RedirectUri = !string.IsNullOrEmpty(returnUrl) && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative) && (returnUrl.StartsWith("/") || returnUrl.StartsWith("~/")) ? returnUrl : "/"
        };

        // Ask the OpenIddict client middleware to redirect the user agent to the identity provider.
        await ctx.ChallengeAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme, properties);
    }
}
