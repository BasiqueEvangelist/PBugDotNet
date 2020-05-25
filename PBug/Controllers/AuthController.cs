using System.Security.Claims;
using System.Buffers;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBug.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using PBug.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using PBug.Utils;

namespace PBug.Controllers
{
    public class AuthController : Controller
    {
        private readonly PBugContext Db;
        private readonly ILogger<AuthController> logger;

        public AuthController(PBugContext db, ILogger<AuthController> log)
        {
            Db = db;
            logger = log;
        }

        [Route("/login/")]
        [HttpGet]
        public IActionResult Login([FromQuery] string redirect = "")
        {
            try
            {
                ViewData["RedirectTo"] = new PathString(redirect);
            }
            catch (ArgumentException)
            {
                ViewData["RedirectTo"] = new PathString("");
            }
            return View();
        }

        [Route("/login/")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest req, [FromQuery] string redirect = "")
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                ViewData["RedirectTo"] = new PathString(redirect);
            }
            catch (ArgumentException)
            {
                ViewData["RedirectTo"] = new PathString("");
            }
            var user = await Db.Users
                .SingleOrDefaultAsync(x => x.Username == req.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View();
            }
            byte[] newHash = AuthUtils.GetHashFor(req.Password, user.PasswordSalt);
            if (!Enumerable.SequenceEqual(newHash, user.PasswordHash))
            {
                ModelState.AddModelError("", "Incorrect password");
                return View();
            }
            var userIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            userIdentity.AddClaims(new Claim[] {
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.FullName)
            });
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            try
            {
                return Redirect(new PathString(redirect));
            }
            catch (ArgumentException)
            {
                return RedirectToAction("News", "Issue");
            }
        }

        [Route("/register/")]
        [HttpGet]
        public IActionResult Register([FromQuery] string redirect = "")
        {
            try
            {
                ViewData["RedirectTo"] = new PathString(redirect);
            }
            catch (ArgumentException)
            {
                ViewData["RedirectTo"] = "";
            }
            return View();
        }

        [Route("/register/")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest req, [FromQuery] string redirect = "")
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                ViewData["RedirectTo"] = new PathString(redirect);
            }
            catch (ArgumentException)
            {
                ViewData["RedirectTo"] = new PathString("");
            }
            var user = await Db.Users.SingleOrDefaultAsync(x => x.Username == req.Username && x.FullName == req.FullName);
            if (user != null)
            {
                ModelState.AddModelError("", "User already exists");
                return View();
            }
            Invite inv = await Db.Invites
                .SingleAsync(x => x.Uid == req.InviteID);
            User newUser = new User()
            {
                RoleId = inv.RoleId, // Anonymous role
                Username = req.Username,
                FullName = req.FullName,
                PasswordSalt = AuthUtils.GetRandomData(64)
            };
            newUser.PasswordHash = AuthUtils.GetHashFor(req.Password, newUser.PasswordSalt);
            await Db.AddAsync(newUser);
            Db.Remove(inv);
            await Db.SaveChangesAsync();

            var userIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            userIdentity.AddClaims(new Claim[] {
                new Claim(ClaimTypes.PrimarySid, newUser.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, newUser.Username),
                new Claim(ClaimTypes.Name, newUser.FullName),
                new Claim(ClaimTypes.Role, newUser.RoleId.ToString())
            });
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            try
            {
                return Redirect(new PathString(redirect));
            }
            catch (ArgumentException)
            {
                return RedirectToAction("News", "Issue");
            }
        }

        [Route("/logout/")]
        public async Task<IActionResult> LogOut([FromQuery] string redirect = "")
        {
            await HttpContext.SignOutAsync();
            try
            {
                return Redirect(new PathString(redirect));
            }
            catch (ArgumentException)
            {
                return RedirectToAction("Login", "Auth");
            }
        }
    }
}