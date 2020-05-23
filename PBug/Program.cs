using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PBug.Data;
using PBug.Utils;

namespace PBug
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--make-db"))
                BuildDb();
            else
                CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void BuildDb()
        {
            var cfg = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();

            var dbOpts = new DbContextOptionsBuilder<PBugContext>().UseMySql(cfg.GetConnectionString("Database"));
            using (var ctx = new PBugContext(dbOpts.Options))
            {
                var logger = LoggerFactory.Create(opts => opts.AddConsole()).CreateLogger("PBug");
                logger.LogInformation("Ensuring creation of DB");
                ctx.Database.EnsureCreated();
                if (!ctx.Users.Any())
                {
                    Role anonymous = new Role()
                    {
                        Name = "Anonymous",
                        Permissions = ""
                    };

                    Role admin = new Role()
                    {
                        Name = "Administrator",
                        Permissions = "**"
                    };

                    ctx.AddRange(anonymous, admin);


                    User system = new User()
                    {
                        Role = admin,
                        Username = "pbug",
                        FullName = "PBug System"
                    };
                    string password = Convert.ToBase64String(AuthUtils.GetRandomData(64));
                    system.PasswordSalt = AuthUtils.GetRandomData(64);
                    system.PasswordHash = AuthUtils.GetHashFor(password, system.PasswordSalt);

                    ctx.Add(system);
                    ctx.SaveChanges();
                    File.WriteAllText("systempass.txt", password);
                    logger.LogInformation("Created database. System password is in systempass.txt", password);
                }
            }
        }
    }
}
