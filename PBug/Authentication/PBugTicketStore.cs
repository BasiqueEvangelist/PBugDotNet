using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PBug.Data;

namespace PBug.Authentication
{
    public class PBugTicketStore : ITicketStore
    {
        IDbContextFactory<PBugContext> dbFactory;
        public PBugTicketStore(IDbContextFactory<PBugContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public async Task RemoveAsync(string key)
        {
            using (var ctx = await dbFactory.CreateDbContextAsync())
            {
                Session s = await ctx.Sessions.FindAsync(Convert.FromBase64String(key));
                if (s != null)
                {
                    ctx.Remove(s);
                    await ctx.SaveChangesAsync();
                }
            }
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            using (var ctx = await dbFactory.CreateDbContextAsync())
            {
                Session s = await ctx.Sessions.FindAsync(Convert.FromBase64String(key));
                if (s != null)
                {
                    s.SessionData = TicketSerializer.Default.Serialize(ticket);
                    s.Expires = ticket.Properties.ExpiresUtc.Value;
                    await ctx.SaveChangesAsync();
                }
            }
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            using (var ctx = await dbFactory.CreateDbContextAsync())
            {
                Session s = await ctx.Sessions.FindAsync(Convert.FromBase64String(key));
                if (s != null)
                {
                    return TicketSerializer.Default.Deserialize(s.SessionData);
                }
            }
            return null;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            using (var ctx = await dbFactory.CreateDbContextAsync())
            {
                Session s = (await ctx.AddAsync(new Session()
                {
                    Id = AuthUtils.GetRandomData(64),
                    SessionData = TicketSerializer.Default.Serialize(ticket),
                    Expires = ticket.Properties.ExpiresUtc
                })).Entity;
                await ctx.SaveChangesAsync();
                return Convert.ToBase64String(s.Id);
            }
        }
    }
}
