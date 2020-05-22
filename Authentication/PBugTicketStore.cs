using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PBug.Data;
using PBug.Utils;

namespace PBug.Authentication
{
    public class PBugTicketStore : ITicketStore
    {
        private readonly DbContextOptions<PBugContext> dbOptions;
        public PBugTicketStore(DbContextOptionsBuilder<PBugContext> options)
        {
            dbOptions = options.Options;
        }

        public async Task RemoveAsync(string key)
        {
            Console.WriteLine("remove " + key);
            using (var ctx = new PBugContext(dbOptions))
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
            Console.WriteLine("renew " + key);
            using (var ctx = new PBugContext(dbOptions))
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
            Console.WriteLine("retrieve " + key);
            using (var ctx = new PBugContext(dbOptions))
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
            using (var ctx = new PBugContext(dbOptions))
            {
                Session s = (await ctx.AddAsync(new Session()
                {
                    Id = AuthUtils.GetRandomData(64),
                    SessionData = TicketSerializer.Default.Serialize(ticket),
                    Expires = ticket.Properties.ExpiresUtc
                })).Entity;
                await ctx.SaveChangesAsync();
                Console.WriteLine("store " + Convert.ToBase64String(s.Id));
                return Convert.ToBase64String(s.Id);
            }
        }
    }
}