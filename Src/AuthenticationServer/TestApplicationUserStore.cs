using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationServer;

public class TestPasswordValidator : IPasswordValidator<ApplicationUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string? password)
    {
        if (string.Equals(user.PasswordHash, password, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(IdentityResult.Success);
        }

        return Task.FromResult(IdentityResult.Failed(new IdentityError
        {
            Code = "InvalidPassword",
            Description = "InvalidPassword"
        }));
    }
}

public class TestApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
{
    private readonly ConcurrentBag<ApplicationUser> _data = new()
    {
        new ApplicationUser
        {
            UserId = "U1",
            UserName = "UN1",
            PasswordHash = "PH1"
        },
        new ApplicationUser
        {
            UserId = "U2",
            UserName = "UN2",
            PasswordHash = "PH2"
        },
        new ApplicationUser
        {
            UserId = "U3",
            UserName = "UN3",
            PasswordHash = "PH3"
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
    {
        throw new NotImplementedException();
    }

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
