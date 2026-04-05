using Microsoft.AspNetCore.Authorization;

namespace PBug.Authentication;

public class PermissionAttribute : AuthorizeAttribute
{
    public const string POLICY_PREFIX = "permission://";

    public PermissionAttribute(string permission) => Policy = POLICY_PREFIX + permission;
}