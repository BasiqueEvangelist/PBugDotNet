using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace PBug.Authentication
{
    public class PBugPermissionHandler : AuthorizationHandler<PBugPermissionRequirement>
    {
        IHttpContextAccessor _httpctx;
        public PBugPermissionHandler(IHttpContextAccessor httpctx)
        {
            _httpctx = httpctx;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PBugPermissionRequirement requirement)
        {
            PermissionData data = _httpctx.HttpContext.Features.Get<PermissionData>();
            if (PermissionParser.ProvePermission(data.PermissionText, requirement.RequiredPermission))
                context.Succeed(requirement);
            else
                context.Fail();
            return Task.CompletedTask;
        }
    }
}