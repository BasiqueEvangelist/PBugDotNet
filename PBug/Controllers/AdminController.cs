using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBug.Authentication;
using PBug.Data;
using PBug.Models;
using PBug.Utils;

namespace PBug.Controllers
{
    public class AdminController : Controller
    {
        private readonly PBugContext Db;

        public AdminController(PBugContext db)
        {
            Db = db;
        }

        [Route("/admin/")]
        public IActionResult Main()
        {
            return View();
        }

        [PBugPermission("admin.createproject")]
        [Route("/admin/createproject")]
        [HttpGet]
        public IActionResult CreateProject()
        {
            return View();
        }

        [PBugPermission("admin.createproject")]
        [Route("/admin/createproject")]
        [HttpPost]
        public async Task<IActionResult> CreateProject(string name, string shortprojectid)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("", "Invalid Name");
                return View();
            }
            if (shortprojectid.Length > 3 || string.IsNullOrWhiteSpace(shortprojectid))
            {
                ModelState.AddModelError("", "Invalid ShortProjectId");
                return View();
            }
            await Db.Projects.AddAsync(new Project()
            {
                AuthorId = HttpContext.User.GetUserId() == -1 ? null : new uint?((uint)HttpContext.User.GetUserId()),
                Name = name,
                ShortProjectId = shortprojectid
            });
            await Db.SaveChangesAsync();
            return RedirectToAction("Main", "Admin");
        }

        [PBugPermission("admin.users")]
        [Route("/admin/viewusers")]
        public async Task<IActionResult> ViewUsers()
        {
            return View(new ViewUsersModel()
            {
                Users = await Db.Users
                    .Include(x => x.Role)
                    .ToArrayAsync(),
                AllRoles = await Db.Roles
                    .ToArrayAsync()
            });
        }

        [PBugPermission("admin.setrole")]
        [HttpPost]
        [Route("/admin/setrole/{id?}")]
        public async Task<IActionResult> SetRole([FromRoute] uint id, uint roleid)
        {
            User u = await Db.Users.FindAsync(id);
            u.RoleId = roleid;
            await Db.SaveChangesAsync();
            return RedirectToAction("ViewUsers");
        }
    }
}