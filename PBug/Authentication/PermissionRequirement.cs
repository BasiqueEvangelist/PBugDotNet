using Microsoft.AspNetCore.Authorization;

namespace PBug.Authentication;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string RequiredPermission { get; }

    public PermissionRequirement(string permtext)
    {
        RequiredPermission = permtext;
    }
}