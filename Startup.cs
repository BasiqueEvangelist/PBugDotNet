using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PBug.Authentication;
using PBug.Data;
using Microsoft.Extensions.Logging;
using PBug.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace PBug
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            DbContextOptionsBuilder<PBugContext> dbOpts = new DbContextOptionsBuilder<PBugContext>().UseMySql(Configuration.GetConnectionString("Database"));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "pbug.sid";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.HttpOnly = true;
                    options.SessionStore = new PBugTicketStore(dbOpts);
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.ReturnUrlParameter = "redirect";
                    options.AccessDeniedPath = "/403";
                    options.ClaimsIssuer = "PBug";
                });

            services.AddAuthorization();
            services.AddHttpContextAccessor();
            services.AddSingleton<IAuthorizationHandler, PBugPermissionHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PBugPermissionPolicyProvider>();



            services.AddControllersWithViews()
#if DEBUG
                .AddRazorRuntimeCompilation();
#else
                ;
#endif
            services.AddDbContext<PBugContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Database"))
                       .EnableSensitiveDataLogging());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            DbContextOptionsBuilder<PBugContext> dbOpts = new DbContextOptionsBuilder<PBugContext>().UseMySql(Configuration.GetConnectionString("Database"));

            using (var ctx = new PBugContext(dbOpts.Options))
            {
                ILogger<Startup> logger = (ILogger<Startup>)app.ApplicationServices.GetRequiredService(typeof(ILogger<Startup>));
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
                    logger.LogInformation("Created system user with username `pbug` and password `{}`", password);
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.Use(async (ctx, next) =>
            {
                if (!ctx.User.HasClaim(x => x.Type == ClaimTypes.PrimarySid))
                {
                    ClaimsIdentity ci = new ClaimsIdentity();
                    ci.AddClaims(new Claim[] {
                        new Claim(ClaimTypes.PrimarySid, "-1"),
                        new Claim(ClaimTypes.NameIdentifier, "anonymous"),
                        new Claim(ClaimTypes.Name, "Anonymous"),
                        new Claim(ClaimTypes.Role, "1"),
                        new Claim(ClaimTypes.Anonymous, "true")
                    });
                    ctx.User.AddIdentity(ci);
                }
                await next.Invoke();
            });

            app.UseRouting();

            app.Use(new Func<RequestDelegate, RequestDelegate>(new PermissionLoadMiddleware(dbOpts.Options).Use));
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
