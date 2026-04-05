using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace PBug.Authentication;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpctx;

    public PermissionHandler(IHttpContextAccessor httpctx)
    {
        _httpctx = httpctx;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        PermissionData data = _httpctx.HttpContext.Features.Get<PermissionData>();
        if (Permissions.CheckPermissions(data.PermissionText, requirement.RequiredPermission))
            context.Succeed(requirement);
        else
            context.Fail(new AuthorizationFailureReason(this, $"You do not have the permissions to do `{requirement.RequiredPermission}`."));
        return Task.CompletedTask;
    }
}