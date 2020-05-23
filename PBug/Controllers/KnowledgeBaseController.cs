using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBug.Authentication;
using PBug.Data;
using PBug.Models;
using PBug.Utils;

namespace PBug.Controllers
{
    public class KnowledgeBaseController : Controller
    {
        private readonly PBugContext Db;
        public KnowledgeBaseController(PBugContext ctx)
        {
            Db = ctx;
        }

        [Route("/kb/")]
        public IActionResult Main()
        {
            return RedirectToAction("ViewPage", new { path = "main" });
        }

        [Route("/kb/search")]
        [PBugPermission("kb.search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (q == null) q = "";
            KBSearchModel model = new KBSearchModel();
            model.SearchString = q;
            IQueryable<Infopage> searchQuery = Db.Infopages
                .Include(x => x.Author);
            bool orderAscending = true;
            if (!HttpContext.UserCan("kb.secrecy.0"))
                searchQuery = searchQuery.Where(x => x.Secrecy != 0);
            if (!HttpContext.UserCan("kb.secrecy.1"))
                searchQuery = searchQuery.Where(x => x.Secrecy != 1);
            if (!HttpContext.UserCan("kb.secrecy.2"))
                searchQuery = searchQuery.Where(x => x.Secrecy != 2);
            if (!HttpContext.UserCan("kb.secrecy.3"))
                searchQuery = searchQuery.Where(x => x.Secrecy != 3);

            foreach (string sub in q.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (sub.StartsWith("#"))
                {
                    string tagName = sub.Substring(1);
                    searchQuery = searchQuery.Where(x => x.Tags.Contains(tagName));
                }
                else if (sub.StartsWith("author:"))
                {
                    string authorName = sub.Substring("author:".Length);
                    if (authorName == "me")
                    {
                        searchQuery = searchQuery.Where(x => x.Author.Id == HttpContext.User.GetUserId());
                    }
                    else
                    {
                        searchQuery = searchQuery.Where(x => x.Author.Username == authorName);
                    }
                }
                else if (sub.StartsWith("order:"))
                {
                    string order = sub.Substring("order:".Length);
                    if (!Regex.IsMatch(order, ".*asc.*"))
                        orderAscending = false;
                }
            }
            if (orderAscending)
                searchQuery = searchQuery.OrderBy(x => x.Id);
            else
                searchQuery = searchQuery.OrderByDescending(x => x.Id);

            model.FoundInfopages = await searchQuery.ToArrayAsync();

            return View(model);
        }

        [Route("/kb/create/{*path}")]
        [HttpGet]
        [PBugPermission("kb.create")]
        public IActionResult Create(string path)
        {
            return View((object)path);
        }

        [Route("/kb/create/{*path}")]
        [HttpPost]
        [PBugPermission("kb.create")]
        public async Task<IActionResult> Create([FromRoute] string path, string name, string tags, string text, byte secrecy)
        {
            if (!HttpContext.UserCan("kb.secrecy." + secrecy.ToString()))
                return Forbid();
            if (secrecy > 3)
                return Forbid();
            await Db.Infopages.AddAsync(new Infopage()
            {
                AuthorId = HttpContext.User.IsAnonymous() ? null : new uint?((uint)HttpContext.User.GetUserId()),
                EditorId = HttpContext.User.IsAnonymous() ? null : new uint?((uint)HttpContext.User.GetUserId()),
                DateOfCreation = DateTime.UtcNow,
                DateOfEdit = DateTime.UtcNow,
                Path = path,
                Name = name,
                Tags = tags,
                ContainedText = text,
                Secrecy = secrecy
            });

            await Db.SaveChangesAsync();

            return RedirectToAction("ViewPage", new
            {
                path = path
            });
        }

        [Route("/kb/view/{*path}")]
        [PBugPermission("kb.view")]
        public async Task<IActionResult> ViewPage([FromRoute] string path)
        {
            Infopage page = await Db.Infopages
                .Include(x => x.Author)
                .SingleOrDefaultAsync(x => x.Path == path);

            if (page == null)
                return RedirectToAction("Create", new { path = path });
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();

            return View(page);
        }

        [Route("/kb/edit/{*path}")]
        [HttpGet]
        [PBugPermission("kb.editpage")]
        public async Task<IActionResult> EditPage([FromRoute] string path)
        {
            var page = await Db.Infopages.SingleAsync();
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            return View(page);
        }

        [Route("/kb/edit/{*path}")]
        [HttpPost]
        [PBugPermission("kb.editpage")]
        public async Task<IActionResult> EditPage([FromRoute] string path, string newtitle, string newtags, string newtext, byte secrecy)
        {
            Infopage page = await Db.Infopages.SingleAsync(x => x.Path == path);

            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            if (secrecy > 3)
                return Forbid();

            page.Name = newtitle;
            page.Tags = newtags;
            page.ContainedText = newtext;
            page.Secrecy = secrecy;

            await Db.SaveChangesAsync();

            return RedirectToAction("ViewPage", new { path = path });
        }

        [Route("/kb/talk/{*path}")]
        [PBugPermission("kb.talk")]
        public async Task<IActionResult> ViewTalk([FromRoute] string path)
        {
            var page = await Db.Infopages
                .Include(x => x.Comments)
                .SingleAsync();
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            return View(page);
        }

        [Route("/kb/comment/{*path}")]
        [HttpPost]
        [PBugPermission("kb.comment")]
        public async Task<IActionResult> Comment([FromRoute] string path, string text)
        {
            var page = await Db.Infopages.SingleAsync(x => x.Path == path);
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            InfopageComment post = (await Db.InfopageComments.AddAsync(new InfopageComment()
            {
                AuthorId = HttpContext.User.IsAnonymous() ? null : new uint?((uint)HttpContext.User.GetUserId()),
                Infopage = await Db.Infopages.SingleAsync(x => x.Path == path),
                ContainedText = text,
                DateOfCreation = DateTime.UtcNow
            })).Entity;

            await Db.SaveChangesAsync();

            return RedirectToAction("ViewTalk", "KnowledgeBase", new { path = path }, post.Id.ToString());
        }

        [Route("/kb/editcomment/{id}")]
        [HttpGet]
        [PBugPermission("kb.editcomment")]
        public async Task<IActionResult> EditComment([FromRoute] uint id)
        {
            var comment = await Db.InfopageComments
                .Include(x => x.Author)
                .Include(x => x.Infopage)
                .SingleAsync(x => x.Id == id);

            if (!HttpContext.UserCan("kb.secrecy." + comment.Infopage.Secrecy.ToString()))
                return Forbid();

            return View();
        }

        [Route("/kb/editcomment/{id}")]
        [HttpPost]
        [PBugPermission("kb.editcomment")]
        public async Task<IActionResult> EditComment([FromRoute] uint id, string newtext)
        {
            InfopageComment comment = await Db.InfopageComments
                .Include(x => x.Infopage)
                .SingleAsync();

            if (!HttpContext.UserCan("kb.secrecy." + comment.Infopage.Secrecy.ToString()))
                return Forbid();

            comment.DateOfEdit = DateTime.UtcNow;
            comment.ContainedText = newtext;

            await Db.SaveChangesAsync();
            return RedirectToAction("ViewTalk", "KnowledgeBase", new { path = comment.Infopage.Path }, comment.Id.ToString());
        }

        [Route("/kb/delete/{*path}")]
        [PBugPermission("kb.deletepage")]
        public async Task<IActionResult> DeletePage([FromRoute] string path)
        {
            Infopage page = await Db.Infopages.SingleAsync(x => x.Path == path);
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            await Db.Database.ExecuteSqlInterpolatedAsync($"delete from infopagecomments where infopageid = {page.Id}");

            Db.Infopages.Remove(page);

            await Db.SaveChangesAsync();

            return RedirectToAction("Main");
        }
    }
}