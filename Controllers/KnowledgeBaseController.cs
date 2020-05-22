using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (q == null) q = "";
            KBSearchModel model = new KBSearchModel();
            model.SearchString = q;
            IQueryable<Infopage> searchQuery = Db.Infopages
                .Include(x => x.Author);
            bool orderAscending = true;
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
        public IActionResult Create(string path)
        {
            return View((object)path);
        }

        [Route("/kb/create/{*path}")]
        [HttpPost]
        public async Task<IActionResult> Create([FromRoute] string path, string name, string tags, string text, byte secrecy)
        {
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
        public async Task<IActionResult> ViewPage([FromRoute] string path)
        {
            Infopage page = await Db.Infopages
                .Include(x => x.Author)
                .SingleOrDefaultAsync(x => x.Path == path);

            if (page == null)
            {
                return RedirectToAction("Create", new { path = path });
            }

            return View(page);
        }

        [Route("/kb/edit/{*path}")]
        [HttpGet]
        public async Task<IActionResult> EditPage([FromRoute] string path)
        {
            return View(await Db.Infopages
                .SingleAsync());
        }

        [Route("/kb/edit/{*path}")]
        [HttpPost]
        public async Task<IActionResult> EditPage([FromRoute] string path, string newtitle, string newtags, string newtext, byte secrecy)
        {
            Infopage page = await Db.Infopages.SingleAsync(x => x.Path == path);

            page.Name = newtitle;
            page.Tags = newtags;
            page.ContainedText = newtext;
            page.Secrecy = secrecy;

            await Db.SaveChangesAsync();

            return RedirectToAction("ViewPage", new { path = path });
        }

        [Route("/kb/talk/{*path}")]
        public async Task<IActionResult> ViewTalk([FromRoute] string path)
        {
            return View(await Db.Infopages
                .Include(x => x.Comments)
                .SingleAsync());
        }

        [Route("/kb/comment/{*path}")]
        [HttpPost]
        public async Task<IActionResult> Comment([FromRoute] string path, string text)
        {
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
        public async Task<IActionResult> EditComment([FromRoute] uint id)
        {
            return View(await Db.InfopageComments
                .Include(x => x.Author)
                .SingleAsync(x => x.Id == id));
        }

        [Route("/kb/editcomment/{id}")]
        [HttpPost]
        public async Task<IActionResult> EditComment([FromRoute] uint id, string newtext)
        {
            InfopageComment comment = await Db.InfopageComments
                .Include(x => x.Infopage)
                .SingleAsync();

            comment.DateOfEdit = DateTime.UtcNow;
            comment.ContainedText = newtext;

            await Db.SaveChangesAsync();
            return RedirectToAction("ViewTalk", "KnowledgeBase", new { path = comment.Infopage.Path }, comment.Id.ToString());
        }

        [Route("/kb/delete/{*path}")]
        public async Task<IActionResult> DeletePage([FromRoute] string path)
        {
            Infopage page = await Db.Infopages.SingleAsync(x => x.Path == path);
            await Db.Database.ExecuteSqlInterpolatedAsync($"delete from infopagecomments where infopageid = {page.Id}");

            Db.Infopages.Remove(page);

            await Db.SaveChangesAsync();

            return RedirectToAction("Main");
        }
    }
}