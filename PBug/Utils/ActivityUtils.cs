using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PBug.Data;

namespace PBug.Utils
{
    public static class ActivityUtils
    {
        public static ValueTask<EntityEntry<IssueActivity>> AddIssueActivity(this DbSet<IssueActivity> activities, HttpContext ctx, uint id, IssueActivity i)
        {
            i.AuthorId = ctx.User.IsAnonymous() ? null : new uint?((uint)ctx.User.GetUserId());
            i.IssueId = id;
            i.DateOfOccurance = DateTime.UtcNow;
            return activities.AddAsync(i);
        }

        public static ValueTask<EntityEntry<KBActivity>> AddKBActivity(this DbSet<KBActivity> activities, HttpContext ctx, uint id, KBActivity i)
        {
            i.AuthorId = ctx.User.IsAnonymous() ? null : new uint?((uint)ctx.User.GetUserId());
            i.InfopageId = id;
            i.DateOfOccurance = DateTime.UtcNow;
            return activities.AddAsync(i);
        }
    }
}