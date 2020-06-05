using System.Globalization;
using System.Net.Cache;
using System.Net.Http.Headers;
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
                else
                {
                    searchQuery = searchQuery.Where(x => x.Name.Contains(sub));
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
            return View((object)(path.Replace("%2F", "/", true, CultureInfo.InvariantCulture)));
        }

        [Route("/kb/create/{*path}")]
        [HttpPost]
        [PBugPermission("kb.create")]
        public async Task<IActionResult> Create([FromRoute] string path, CreatePageRequest req)
        {
            if (!ModelState.IsValid)
                return View((object)(path.Replace("%2F", "/", true, CultureInfo.InvariantCulture)));
            if (!HttpContext.UserCan("kb.secrecy." + ((byte)req.Secrecy).ToString()))
                return Forbid();
            var page = (await Db.Infopages.AddAsync(new Infopage()
            {
                AuthorId = HttpContext.User.IsAnonymous() ? null : new uint?((uint)HttpContext.User.GetUserId()),
                EditorId = HttpContext.User.IsAnonymous() ? null : new uint?((uint)HttpContext.User.GetUserId()),
                DateOfCreation = DateTime.UtcNow,
                DateOfEdit = DateTime.UtcNow,
                Path = path.Replace("%2F", "/", true, CultureInfo.InvariantCulture),
                Name = req.Name,
                Tags = req.Tags ?? "",
                ContainedText = req.Text,
                Secrecy = (byte)req.Secrecy
            })).Entity;

            await Db.KBActivities.AddKBActivity(HttpContext, 0, new CreatePageActivity()
            {
                Infopage = page,
                Name = req.Name,
                ContainedText = req.Text,
                Tags = req.Tags,
                Secrecy = (int?)req.Secrecy
            });

            await Db.SaveChangesAsync();

            return RedirectToAction("ViewPage", new
            {
                path = path.Replace("%2F", "/", true, CultureInfo.InvariantCulture)
            });
        }

        [Route("/kb/view/{*path}")]
        [PBugPermission("kb.view")]
        public async Task<IActionResult> ViewPage([FromRoute] string path)
        {
            Infopage page = await Db.Infopages
                .Include(x => x.Author)
                .SingleOrDefaultAsync(x => x.Path == path.Replace("%2F", "/", true, CultureInfo.InvariantCulture));

            if (page == null)
                return View("NoPage", path.Replace("%2F", "/", true, CultureInfo.InvariantCulture));
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();

            return View(page);
        }

        [Route("/kb/edit/{*path}")]
        [HttpGet]
        public async Task<IActionResult> EditPage([FromRoute] string path)
        {
            var page = await Db.Infopages.SingleAsync(x => x.Path == path.Replace("%2F", "/", true, CultureInfo.InvariantCulture));
            if (!HttpContext.UserCan("kb.editpage.all")
            && !(HttpContext.UserCan("kb.editpage.own") && ((int?)page.AuthorId ?? -1) == HttpContext.User.GetUserId()))
            {
                if (HttpContext.User.IsAnonymous())
                    return Challenge();
                else
                    return Forbid();
            }
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            return View(page);
        }

        [Route("/kb/edit/{*path}")]
        [HttpPost]
        public async Task<IActionResult> EditPage([FromRoute] string path, CreatePageRequest req)
        {
            Infopage page = await Db.Infopages.SingleAsync(x => x.Path == path.Replace("%2F", "/", true, CultureInfo.InvariantCulture));
            if (!HttpContext.UserCan("kb.editpage.all")
            && !(HttpContext.UserCan("kb.editpage.own") && ((int?)page.AuthorId ?? -1) == HttpContext.User.GetUserId()))
            {
                if (HttpContext.User.IsAnonymous())
                    return Challenge();
                else
                    return Forbid();
            }

            if (!ModelState.IsValid)
                return View(page);

            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            if (!HttpContext.UserCan("kb.secrecy." + ((byte)req.Secrecy).ToString()))
                return Forbid();

            await Db.KBActivities.AddKBActivity(HttpContext, page.Id, new EditPageActivity()
            {
                OldName = page.Name,
                NewName = req.Name,
                OldTags = page.Tags,
                NewTags = req.Tags ?? "",
                OldContainedText = page.ContainedText,
                NewContainedText = req.Text,
                OldSecrecy = page.Secrecy,
                NewSecrecy = (byte)req.Secrecy
            });

            page.Name = req.Name;
            page.Tags = req.Tags ?? "";
            page.ContainedText = req.Text;
            page.Secrecy = (byte)req.Secrecy;

            await Db.SaveChangesAsync();

            return RedirectToAction("ViewPage", new { path = path.Replace("%2F", "/", true, CultureInfo.InvariantCulture) });
        }

        [Route("/kb/talk/{*path}")]
        [PBugPermission("kb.talk")]
        public async Task<IActionResult> ViewTalk([FromRoute] string path)
        {
            var page = await Db.Infopages
                .Include(x => x.Comments)
                    .ThenInclude(x => x.Author)
                .SingleAsync(x => x.Path == path.Replace("%2F", "/", true, CultureInfo.InvariantCulture));
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            return View(page);
        }

        [Route("/kb/activity/{*path}")]
        [PBugPermission("kb.activity")]
        public async Task<IActionResult> ViewActivity([FromRoute] string path)
        {
            var page = await Db.Infopages
                .Include(x => x.Activities)
                    .ThenInclude(x => x.Author)
                .SingleAsync(x => x.Path == path.Replace("%2F", "/", true, CultureInfo.InvariantCulture));
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            return View(page);
        }

        [Route("/kb/comment/{*path}")]
        [HttpPost]
        [PBugPermission("kb.comment")]
        public async Task<IActionResult> Comment([FromRoute] string path, CommentPostRequest req)
        {
            if (!ModelState.IsValid)
                return Forbid();
            var page = await Db.Infopages.SingleAsync(x => x.Path == path.Replace("%2F", "/", true, CultureInfo.InvariantCulture));
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            InfopageComment post = (await Db.InfopageComments.AddAsync(new InfopageComment()
            {
                AuthorId = HttpContext.User.IsAnonymous() ? null : new uint?((uint)HttpContext.User.GetUserId()),
                Infopage = page,
                ContainedText = req.Text,
                DateOfCreation = DateTime.UtcNow
            })).Entity;

            await Db.KBActivities.AddKBActivity(HttpContext, page.Id, new CommentActivity()
            {
                Comment = post,
                ContainedText = req.Text
            });

            await Db.SaveChangesAsync();

            return RedirectToAction("ViewTalk", "KnowledgeBase", new { path = path.Replace("%2F", "/", true, CultureInfo.InvariantCulture) }, post.Id.ToString());
        }

        [Route("/kb/editcomment/{id}")]
        [HttpGet]
        public async Task<IActionResult> EditComment([FromRoute] uint id)
        {
            var comment = await Db.InfopageComments
                .Include(x => x.Author)
                .Include(x => x.Infopage)
                .SingleAsync(x => x.Id == id);

            if (!HttpContext.UserCan("kb.editcomment.all")
            && !(HttpContext.UserCan("kb.editcomment.own") && ((int?)comment.AuthorId ?? -1) == HttpContext.User.GetUserId()))
            {
                if (HttpContext.User.IsAnonymous())
                    return Challenge();
                else
                    return Forbid();
            }
            if (!HttpContext.UserCan("kb.secrecy." + comment.Infopage.Secrecy.ToString()))
                return Forbid();

            return View(comment);
        }

        [Route("/kb/editcomment/{id}")]
        [HttpPost]
        public async Task<IActionResult> EditComment([FromRoute] uint id, CommentPostRequest req)
        {
            InfopageComment comment = await Db.InfopageComments
                .Include(x => x.Infopage)
                .SingleAsync(x => x.Id == id);
            if (!HttpContext.UserCan("kb.editcomment.all")
            && !(HttpContext.UserCan("kb.editcomment.own") && ((int?)comment.AuthorId ?? -1) == HttpContext.User.GetUserId()))
            {
                if (HttpContext.User.IsAnonymous())
                    return Challenge();
                else
                    return Forbid();
            }
            if (!ModelState.IsValid)
                return View(comment);

            if (!HttpContext.UserCan("kb.secrecy." + comment.Infopage.Secrecy.ToString()))
                return Forbid();

            await Db.KBActivities.AddKBActivity(HttpContext, comment.Infopage.Id, new EditCommentActivity()
            {
                CommentId = id,
                OldContainedText = comment.ContainedText,
                NewContainedText = req.Text
            });

            comment.DateOfEdit = DateTime.UtcNow;
            comment.ContainedText = req.Text;

            await Db.SaveChangesAsync();
            return RedirectToAction("ViewTalk", "KnowledgeBase", new { path = comment.Infopage.Path }, comment.Id.ToString());
        }

        [Route("/kb/delete/{*path}")]
        [PBugPermission("kb.deletepage")]
        public async Task<IActionResult> DeletePage([FromRoute] string path)
        {
            Infopage page = await Db.Infopages.SingleAsync(x => x.Path == path.Replace("%2F", "/", true, CultureInfo.InvariantCulture));
            if (!HttpContext.UserCan("kb.secrecy." + page.Secrecy.ToString()))
                return Forbid();
            await Db.Database.ExecuteSqlInterpolatedAsync($"delete from infopagecomments where infopageid = {page.Id}");
            await Db.Database.ExecuteSqlInterpolatedAsync($"delete from kbactivities where infopageid = {page.Id}");

            Db.Infopages.Remove(page);

            await Db.SaveChangesAsync();

            return RedirectToAction("Main");
        }
    }
}