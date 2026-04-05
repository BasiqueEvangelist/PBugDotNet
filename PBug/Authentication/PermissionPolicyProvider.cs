using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PBug.Authentication;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider Fallback { get; }

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> opt)
     => Fallback = new DefaultAuthorizationPolicyProvider(opt);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => Fallback.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PermissionAttribute.POLICY_PREFIX))
        {
            var req = new PermissionRequirement(policyName[PermissionAttribute.POLICY_PREFIX.Length..]);
            var policy = new AuthorizationPolicy([req], []);
            return Task.FromResult(policy);
        }
        return Fallback.GetPolicyAsync(policyName);
    }

    public bool AllowsCachingPolicies => true;
}