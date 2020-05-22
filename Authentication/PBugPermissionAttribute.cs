using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PBug.Authentication
{
    public class PBugPermissionAttribute : AuthorizeAttribute
    {
        public const string POLICY_PREFIX = "PBugPermission";

        public PBugPermissionAttribute(string permission) { Permission = permission; }

        public string Permission
        {
            get
            {
                return Policy.Substring(POLICY_PREFIX.Length);
            }
            set
            {
                Policy = POLICY_PREFIX + value;
            }
        }
    }
}