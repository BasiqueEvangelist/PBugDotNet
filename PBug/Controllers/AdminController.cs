using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public async Task<IActionResult> CreateProject(CreateProjectRequest req)
        {
            if (!ModelState.IsValid)
                return View();
            await Db.Projects.AddAsync(new Project()
            {
                AuthorId = HttpContext.User.GetUserId() == -1 ? null : new uint?((uint)HttpContext.User.GetUserId()),
                Name = req.Name,
                ShortProjectId = req.ShortID
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
        public async Task<IActionResult> SetRole(SetRoleRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            User u = await Db.Users.FindAsync(req.UserID);
            u.RoleId = req.RoleID;
            await Db.SaveChangesAsync();
            return RedirectToAction("ViewUsers");
        }
    }
}