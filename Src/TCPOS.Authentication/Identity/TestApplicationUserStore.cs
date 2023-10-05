using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;

namespace TCPOS.Authentication.Identity;

public class TestApplicationUserStore : IUserPasswordStore<ApplicationUser>
{
    private readonly ConcurrentBag<ApplicationUser> _data = new()
    {
        new ApplicationUser
        {
            UserId = "U1",
            UserName = "UN1",
            PasswordHash = "P1"
        },
        new ApplicationUser
        {
            UserId = "U2",
            UserName = "UN2",
            PasswordHash = "P2"
        },
        new ApplicationUser
        {
            UserId = "U3",
            UserName = "UN3",
            PasswordHash = "P3"
        }
    };

    public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.PasswordHash);
    }

    public async Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(!string.IsNullOrEmpty(user.UserName));
    }

    public void Dispose()
    { }

    public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.UserId);
    }

    public async Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.UserName);
    }

    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return await Task.FromResult(user.UserName);
    }

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_data.FirstOrDefault(i => string.Compare(i.UserId, userId, StringComparison.OrdinalIgnoreCase) == 0));
    }

    public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_data.FirstOrDefault(i => string.Compare(i.UserName, normalizedUserName, StringComparison.OrdinalIgnoreCase) == 0));
    }
}
