using Microsoft.AspNetCore.Identity;

namespace AuthenticationServer;

public class TestPasswordHasher : IPasswordHasher<ApplicationUser>
{
    public string HashPassword(ApplicationUser user, string password)
    {
        return user.PasswordHash;
    }

    public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
    {
        if (string.Equals(user.PasswordHash, hashedPassword, StringComparison.Ordinal))
        {
            return PasswordVerificationResult.Success;
        }

        return PasswordVerificationResult.Failed;
    }
}