using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using PBug.Authentication;
using PBug.Data;

namespace PBug;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("Database"); 
        services.AddDbContextFactory<PBugContext>(opts =>
        {
            opts.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        services.AddSingleton<ITicketStore, PBugTicketStore>();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie((options) =>
            { 
                options.Cookie.Name = "pbug.sid";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.ReturnUrlParameter = "redirect";
                options.AccessDeniedPath = "/403";
                options.ClaimsIssuer = "PBug";
            });

        // Crutch to make CookieAuthenticationOptions use an ITicketStore that wants DI
        services.PostConfigureWithDi<CookieAuthenticationOptions, ITicketStore>((opts, store) =>
        {
            opts.SessionStore = store;
        });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddSingleton<IAuthorizationHandler, PBugPermissionHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PBugPermissionPolicyProvider>();

        services.AddControllersWithViews();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.Use(RequestTimeFeature.Middleware);
        app.UsePathBase(Configuration.GetValue<string>("PathBase"));
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }
        app.UseStaticFiles();

        app.UseStatusCodePagesWithReExecute("/{0}");

        app.UseAuthentication();
        app.Use(async (ctx, next) =>
        {
            if (!ctx.User.HasClaim(x => x.Type == ClaimTypes.PrimarySid))
            {
                ClaimsIdentity ci = new ClaimsIdentity();
                ci.AddClaims([
                    new Claim(ClaimTypes.PrimarySid, "-1"),
                    new Claim(ClaimTypes.NameIdentifier, "anonymous"),
                    new Claim(ClaimTypes.Name, "Anonymous"),
                    new Claim(ClaimTypes.Role, "1"),
                    new Claim(ClaimTypes.Anonymous, "true")
                ]);
                ctx.User.AddIdentity(ci);
            }
            await next.Invoke();
        });
        app.UseMiddleware<PermissionLoadMiddleware>();

        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
