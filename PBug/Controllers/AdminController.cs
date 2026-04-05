using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBug.Authentication;
using PBug.Data;
using PBug.Models;

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

        [Permission("admin.createproject")]
        [Route("/admin/createproject")]
        [HttpGet]
        public IActionResult CreateProject()
        {
            return View();
        }

        [Permission("admin.createproject")]
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

        [Permission("admin.users")]
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

        [Permission("admin.setrole")]
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
        [Permission("admin.viewinvites")]
        [Route("/admin/invites")]
        public async Task<IActionResult> ViewInvites()
        {
            return View(new ViewInvitesModel()
            {
                Invites = await Db.Invites
                    .Include(x => x.Role)
                    .ToArrayAsync(),
                Roles = await Db.Roles.ToArrayAsync()
            });
        }
        [Permission("admin.createinvite")]
        [Route("/admin/invites/create")]
        [HttpPost]
        public async Task<IActionResult> CreateInvite(CreateInviteRequest req)
        {
            await Db.Invites.AddAsync(new Invite()
            {
                RoleId = req.RoleID,
                Uid = Convert.ToBase64String(AuthUtils.GetRandomData(48))
                    .Replace('/', '_')
                    .Replace('+', '-')
            });
            await Db.SaveChangesAsync();
            return RedirectToAction("ViewInvites");
        }
        [Permission("admin.viewroles")]
        [Route("/admin/roles")]
        public async Task<IActionResult> ViewRoles()
        {
            return View(await Db.Roles.ToArrayAsync());
        }

        [Permission("admin.createrole")]
        [Route("/admin/roles/create")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await Db.Roles.AddAsync(new Role()
            {
                Name = req.Name,
                Permissions = req.Permissions
            });
            await Db.SaveChangesAsync();
            return RedirectToAction("ViewRoles");
        }

        [Permission("admin.editrole")]
        [Route("/admin/roles/edit/{id?}")]
        [HttpPost]
        public async Task<IActionResult> EditRole([FromRoute] uint id, EditRoleRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            Role r = await Db.Roles.FindAsync(id);
            r.Permissions = req.Permissions;
            await Db.SaveChangesAsync();
            return RedirectToAction("ViewRoles");
        }

        [Permission("admin.deleterole")]
        [Route("/admin/roles/delete/{id?}")]
        [HttpPost]
        public async Task<IActionResult> DeleteRole(uint id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            Role r = await Db.Roles.FindAsync(id);
            Db.Remove(r);
            await Db.SaveChangesAsync();
            return RedirectToAction("ViewRoles");
        }

        [Permission("admin.deleteinvite")]
        [Route("/admin/invites/delete/{id?}")]
        [HttpPost]
        public async Task<IActionResult> DeleteInvite(uint id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            Invite inv = await Db.Invites.FindAsync(id);
            Db.Remove(inv);
            await Db.SaveChangesAsync();
            return RedirectToAction("ViewInvites");
        }
    }
}