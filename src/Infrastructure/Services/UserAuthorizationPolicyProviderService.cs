// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace NetCa.Infrastructure.Services;

/// <summary>
/// Permission
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Permission"/> class.
/// </remarks>
/// <param name="permissionName"></param>
public class Permission(string permissionName) : IAuthorizationRequirement
{
    /// <summary>
    /// Gets or sets permissionName
    /// </summary>
    /// <value></value>
    public string Name { get; set; } = permissionName;
}

/// <summary>
/// UserAuthorizationPolicyProviderService
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserAuthorizationPolicyProviderService"/> class.
/// </remarks>
/// <param name="options"></param>
public class UserAuthorizationPolicyProviderService(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions _options = options.Value;
    private static readonly SemaphoreSlim SemaphoreSlim = new(1);

    /// <inheritdoc/>
    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        SemaphoreSlim.Wait();

        var policy = await base.GetPolicyAsync(policyName).ConfigureAwait(false);

        if (policy == null)
        {
            policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new Permission(policyName))
                .Build();

            _options.AddPolicy(policyName, policy);
        }

        SemaphoreSlim.Release();

        return policy;
    }
}
