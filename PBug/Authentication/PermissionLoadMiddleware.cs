using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PBug.Data;
using System.Security.Claims;

namespace PBug.Authentication;

public class PermissionLoadMiddleware
{
    private readonly RequestDelegate next;
    IDbContextFactory<PBugContext> dbFactory;
    public PermissionLoadMiddleware(RequestDelegate next, IDbContextFactory<PBugContext> dbFactory)
    {
        this.next = next;
        this.dbFactory = dbFactory;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        Role role;
        using (var db = await dbFactory.CreateDbContextAsync())
        {
            if (ctx.User.IsAnonymous())
                role = await db.Roles.FindAsync(1u);
            else
                role = await db.Users.Where(x => x.Id == ctx.User.GetUserId()).Select(x => x.Role).FirstAsync();
        }
        ctx.Features.Set(new PermissionData()
        {
            PermissionText = role.Permissions,
            RoleName = role.Name
        });
        await next.Invoke(ctx);
    }
}

public struct PermissionData
{
    public string PermissionText { get; set; }
    public string RoleName { get; set; }
}
