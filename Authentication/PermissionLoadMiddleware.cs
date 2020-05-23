using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using PBug.Data;
using PBug.Utils;
using Microsoft.EntityFrameworkCore;

namespace PBug.Authentication
{
    public class PermissionLoadMiddleware
    {
        DbContextOptions<PBugContext> dbopts;
        public PermissionLoadMiddleware(DbContextOptions<PBugContext> dbopts)
        {
            this.dbopts = dbopts;
        }
        public RequestDelegate Use(RequestDelegate next)
        {
            return async (HttpContext ctx) =>
            {
                Role role;
                using (PBugContext db = new PBugContext(dbopts))
                    if (ctx.User.IsAnonymous())
                        role = await db.Roles.FindAsync(1u);
                    else
                        role = await db.Users.Where(x => x.Id == ctx.User.GetUserId()).Select(x => x.Role).FirstAsync();
                ctx.Features.Set(new PermissionData()
                {
                    PermissionText = role.Permissions,
                    RoleName = role.Name
                });
                await next(ctx);
            };
        }
    }
    public struct PermissionData
    {
        public string PermissionText { get; set; }
        public string RoleName { get; set; }
    }
}