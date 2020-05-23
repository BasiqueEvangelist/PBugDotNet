using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PBug.Authentication
{
    public class PBugPermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider Fallback { get; }

        public PBugPermissionPolicyProvider(IOptions<AuthorizationOptions> opt)
         => Fallback = new DefaultAuthorizationPolicyProvider(opt);

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Fallback.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => Fallback.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PBugPermissionAttribute.POLICY_PREFIX))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PBugPermissionRequirement(policyName.Substring(PBugPermissionAttribute.POLICY_PREFIX.Length)));
                return Task.FromResult(policy.Build());
            }
            return Fallback.GetPolicyAsync(policyName);
        }
    }
}