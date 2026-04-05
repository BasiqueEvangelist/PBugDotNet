using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace PBug.Authentication;

public class PBugPermissionHandler : AuthorizationHandler<PBugPermissionRequirement>
{
    private readonly IHttpContextAccessor _httpctx;

    public PBugPermissionHandler(IHttpContextAccessor httpctx)
    {
        _httpctx = httpctx;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PBugPermissionRequirement requirement)
    {
        PermissionData data = _httpctx.HttpContext.Features.Get<PermissionData>();
        if (Permissions.CheckPermissions(data.PermissionText, requirement.RequiredPermission))
            context.Succeed(requirement);
        else
            context.Fail(new AuthorizationFailureReason(this, $"You do not have the permissions to do `{requirement.RequiredPermission}`."));
        return Task.CompletedTask;
    }
}