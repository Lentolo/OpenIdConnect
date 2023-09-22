using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AuthorizationServer.ViewModels;

public class LoginViewModel
{
    [Required]
    public string Username
    {
        get;
        set;
    }

    [Required]
    public string Password
    {
        get;
        set;
    }

    public string? ReturnUrl
    {
        get;
        set;
    }
}