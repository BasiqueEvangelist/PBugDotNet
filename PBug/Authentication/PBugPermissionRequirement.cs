using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PBug.Authentication
{
    public class PBugPermissionRequirement : IAuthorizationRequirement
    {
        public string RequiredPermission { get; }

        public PBugPermissionRequirement(string permtext)
        {
            RequiredPermission = permtext;
        }
    }
}