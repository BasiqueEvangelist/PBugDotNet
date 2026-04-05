using Microsoft.AspNetCore.Authorization;

namespace PBug.Authentication;

public class PBugPermissionAttribute : AuthorizeAttribute
{
    public const string POLICY_PREFIX = "permission://";

    public PBugPermissionAttribute(string permission) => Policy = POLICY_PREFIX + permission;
}