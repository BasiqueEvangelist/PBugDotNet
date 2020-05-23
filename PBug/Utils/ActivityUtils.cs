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
        public static ValueTask<EntityEntry<IssueActivity>> AddActivity(this DbSet<IssueActivity> activities, HttpContext ctx, uint id, IssueActivity i)
        {
            i.AuthorId = ctx.User.IsAnonymous() ? null : new uint?((uint)ctx.User.GetUserId());
            i.IssueId = id;
            i.DateOfOccurance = DateTime.UtcNow;
            return activities.AddAsync(i);
        }
    }
}