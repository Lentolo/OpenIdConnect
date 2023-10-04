using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationServer;

public class TestApplicationRoleStore : IRoleStore<ApplicationRole>
{
    private readonly ConcurrentBag<ApplicationRole> _data = new()
    {
        new ApplicationRole
        {
            RoleId = "R1",
            RoleName = "RN1"
        },
        new ApplicationRole
        {
            RoleId = "R2",
            RoleName = "RN2"
        },
        new ApplicationRole
        {
            RoleId = "R3",
            RoleName = "RN3"
        }
    };

    public void Dispose()
    { }

    public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.RoleId);
    }

    public async Task<string?> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.RoleName);
    }

    public Task SetRoleNameAsync(ApplicationRole role, string? roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string?> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return await Task.FromResult(role.RoleName);
    }

    public Task SetNormalizedRoleNameAsync(ApplicationRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ApplicationRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_data.FirstOrDefault(i => string.Compare(i.RoleId, roleId, StringComparison.OrdinalIgnoreCase) == 0));
    }

    public async Task<ApplicationRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_data.FirstOrDefault(i => string.Compare(i.RoleName, normalizedRoleName, StringComparison.OrdinalIgnoreCase) == 0));
    }
}
