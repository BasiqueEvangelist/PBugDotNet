using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PBug.Data;
using PBug.Models;
using PBug.Authentication;
using Microsoft.EntityFrameworkCore;
using PBug.Utils;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace PBug.Controllers
{
    public class IssueController : Controller
    {
        private readonly PBugContext Db;

        public IssueController(PBugContext db)
        {
            Db = db;
        }

        [Route("/")]
        [Route("/news/")]
        [PBugPermission("issue.news")]
        public async Task<IActionResult> News([FromQuery] string q = "watched:yes")
        {
            if (q == null) q = "";
            NewsSearchModel model = new NewsSearchModel();
            model.SearchString = q;
            IQueryable<IssueActivity> searchQuery = Db.IssueActivities
                .Include(x => x.Issue)
                    .ThenInclude(x => x.Assignee)
                .Include(x => x.Issue)
                    .ThenInclude(x => x.Project)
                .Include(x => x.Author);
            bool orderDescending = true;
            foreach (string sub in q.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (sub.StartsWith("#"))
                {
                    string tagName = sub.Substring(1);
                    searchQuery = searchQuery.Where(x => x.Issue.Tags.Contains(tagName));
                }
                else if (sub.StartsWith("status:"))
                {
                    IssueStatus status;
                    if (!Enum.TryParse(sub.Substring("status:".Length), true, out status))
                        continue;
                    searchQuery = searchQuery.Where(x => x.Issue.Status == status);
                }
                else if (sub.StartsWith("project:"))
                {
                    string projectId = sub.Substring("project:".Length);
                    searchQuery = searchQuery.Where(x => x.Issue.Project.ShortProjectId == projectId);
                }
                else if (sub.StartsWith("assignee:"))
                {
                    string assigneeName = sub.Substring("assignee:".Length);
                    if (assigneeName == "me")
                    {
                        searchQuery = searchQuery.Where(x => x.Issue.Assignee != null && x.Issue.Assignee.Id == HttpContext.User.GetUserId());
                    }
                    else if (assigneeName == "none")
                    {
                        searchQuery = searchQuery.Where(x => x.Issue.Assignee == null);
                    }
                    else
                    {
                        searchQuery = searchQuery.Where(x => x.Issue.Assignee != null && x.Issue.Assignee.Username == assigneeName);
                    }
                }
                else if (sub.StartsWith("author:"))
                {
                    string authorName = sub.Substring("author:".Length);
                    if (authorName == "me")
                    {
                        searchQuery = searchQuery.Where(x => x.Issue.Author.Id == HttpContext.User.GetUserId());
                    }
                    else
                    {
                        searchQuery = searchQuery.Where(x => x.Issue.Author.Username == authorName);
                    }
                }
                else if (sub.StartsWith("actor:"))
                {
                    string actorName = sub.Substring("actor:".Length);
                    if (actorName == "me")
                    {
                        searchQuery = searchQuery.Where(x => x.Author.Id == HttpContext.User.GetUserId());
                    }
                    else
                    {
                        searchQuery = searchQuery.Where(x => x.Issue.Author.Username == actorName);
                    }
                }
                else if (sub.StartsWith("watched:"))
                {
                    int userId = HttpContext.User.GetUserId();
                    bool watched = sub.Substring("watched:".Length) == "yes";
                    searchQuery = searchQuery.Where(x =>
                        watched == (x.Issue.AssigneeId == userId || x.Issue.Watchers.Any(w => w.WatcherId == userId)));
                }
                else if (sub.StartsWith("order:"))
                {
                    string order = sub.Substring("order:".Length);
                    if (Regex.IsMatch(order, ".*asc.*"))
                        orderDescending = false;
                }
            }
            if (orderDescending)
                searchQuery = searchQuery.OrderByDescending(x => x.Id);
            else
                searchQuery = searchQuery.OrderBy(x => x.Id);

            model.FoundNews = await searchQuery
                .ToArrayAsync();

            return View(model);
        }

        [Route("/issues/")]
        [Route("/issues/search/")]
        [PBugPermission("issue.search")]
        public async Task<IActionResult> Search([FromQuery] string q = "watched:yes")
        {
            if (q == null) q = "";
            IssueSearchModel model = new IssueSearchModel();
            model.SearchString = q;
            IQueryable<Issue> searchQuery = Db.Issues
                .Include(x => x.Assignee)
                .Include(x => x.Project);
            bool orderDescending = true;
            foreach (string sub in q.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (sub.StartsWith("#"))
                {
                    string tagName = sub.Substring(1);
                    searchQuery = searchQuery.Where(x => x.Tags.Contains(tagName));
                }
                else if (sub.StartsWith("status:"))
                {
                    IssueStatus status;
                    if (!Enum.TryParse(sub.Substring("status:".Length), true, out status))
                        continue;
                    searchQuery = searchQuery.Where(x => x.Status == status);
                }
                else if (sub.StartsWith("project:"))
                {
                    string projectId = sub.Substring("project:".Length);
                    searchQuery = searchQuery.Where(x => x.Project.ShortProjectId == projectId);
                }
                else if (sub.StartsWith("assignee:"))
                {
                    string assigneeName = sub.Substring("assignee:".Length);
                    if (assigneeName == "me")
                    {
                        searchQuery = searchQuery.Where(x => x.Assignee != null && x.Assignee.Id == HttpContext.User.GetUserId());
                    }
                    else if (assigneeName == "none")
                    {
                        searchQuery = searchQuery.Where(x => x.Assignee == null);
                    }
                    else
                    {
                        searchQuery = searchQuery.Where(x => x.Assignee != null && x.Assignee.Username == assigneeName);
                    }
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
                else if (sub.StartsWith("watched:"))
                {
                    int userId = HttpContext.User.GetUserId();
                    bool watched = sub.Substring("watched:".Length) == "yes";
                    searchQuery = searchQuery.Where(x =>
                        watched == (x.AssigneeId == userId || x.Watchers.Any(w => w.WatcherId == userId)));
                }
                else if (sub.StartsWith("order:"))
                {
                    string order = sub.Substring("order:".Length);
                    if (Regex.IsMatch(order, ".*asc.*"))
                        orderDescending = false;
                }
            }
            if (orderDescending)
                searchQuery = searchQuery.OrderByDescending(x => x.Id);
            else
                searchQuery = searchQuery.OrderBy(x => x.Id);

            model.FoundIssues = await searchQuery
                .ToArrayAsync();

            return View(model);
        }

        [Route("/issues/create")]
        [PBugPermission("issue.create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new IssueCreateModel()
            {
                AllProjects = await Db.Projects.ToArrayAsync(),
                AllUsers = await Db.Users.ToArrayAsync()
            });
        }

        [Route("/issues/create")]
        [PBugPermission("issue.create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateIssueRequest req)
        {
            if (!ModelState.IsValid)
                return View();
            if (HttpContext.Request.Form.Files.Sum(x => x.Length) >= 128 * 1024 * 1024)
            {
                ModelState.AddModelError("", "File size too large.");
                return View();
            }
            Issue i = (await Db.Issues.AddAsync(new Issue()
            {
                Name = req.Name,
                Tags = req.Tags,
                Description = req.Description,
                ProjectId = req.ProjectID,
                AssigneeId = req.AssigneeID == -1 ? null : new uint?((uint)req.AssigneeID),
                AuthorId = HttpContext.User.IsAnonymous() ? null : new uint?((uint)HttpContext.User.GetUserId()),
                DateOfCreation = DateTime.UtcNow,
                Status = IssueStatus.Open
            })).Entity;

            await Db.IssueActivities.AddActivity(HttpContext, 0, new CreateIssueActivity()
            {
                Issue = i,
                Name = req.Name,
                Description = req.Description,
                Tags = req.Tags,
                ProjectId = req.ProjectID,
                AssigneeId = req.AssigneeID == -1 ? null : new uint?((uint)req.AssigneeID),
            });

            List<(IssueFile, IFormFile)> toProcess = new List<(IssueFile, IFormFile)>();

            foreach (IFormFile file in HttpContext.Request.Form.Files)
            {
                toProcess.Add((
                    (await Db.IssueFiles.AddAsync(new IssueFile()
                    {
                        Issue = i,
                        FileName = file.FileName,
                        FileId = Convert.ToBase64String(AuthUtils.GetRandomData(12))
                            .Replace('/', '_')
                            .Replace('+', '-')
                    })).Entity,
                    file));
            }

            await Db.SaveChangesAsync();

            foreach ((IssueFile, IFormFile) pack in toProcess)
            {
                using (FileStream fs = System.IO.File.OpenWrite(Path.Combine("files", pack.Item1.FileId)))
                    await pack.Item2.CopyToAsync(fs);
            }

            return RedirectToAction("ViewTalk", new { id = i.Id });
        }

        [Route("/issues/{id?}")]
        [PBugPermission("issue.view")]
        public async Task<IActionResult> ViewTalk([FromRoute] int id)
        {
            Issue i = await Db.Issues
                .Include(x => x.Posts)
                .ThenInclude(x => x.Author)
                .Include(x => x.Author)
                .Include(x => x.Project)
                .Include(x => x.Files)
                .Include(x => x.Assignee)
                .SingleAsync(x => x.Id == id);
            return View(new IssueViewModel()
            {
                Issue = i,
                IsWatching = await Db.IssueWatchers.AnyAsync(x => x.IssueId == i.Id && x.WatcherId == HttpContext.User.GetUserId())
            });
        }

        [Route("/issues/{id?}/activity")]
        [PBugPermission("issue.activity")]
        public async Task<IActionResult> ViewActivity([FromRoute] int id)
        {
            await Db.IssueActivities
                .Where(x => x.IssueId == id)
                .Include(x => x.Author)
                .Include(x => (x as CreateIssueActivity).Assignee)
                .Include(x => (x as CreateIssueActivity).Project)
                .Include(x => (x as EditIssueActivity).NewAssignee)
                .Include(x => (x as EditIssueActivity).OldAssignee)
                .Include(x => (x as EditIssueActivity).OldProject)
                .Include(x => (x as EditIssueActivity).NewProject)
                .LoadAsync();
            Issue i = await Db.Issues
                .Include(x => x.Activities)
                .Include(x => x.Author)
                .Include(x => x.Project)
                .Include(x => x.Files)
                .Include(x => x.Assignee)
                .SingleAsync(x => x.Id == id);
            return View(new IssueViewModel()
            {
                Issue = i,
                IsWatching = await Db.IssueWatchers.AnyAsync(x => x.IssueId == i.Id && x.WatcherId == HttpContext.User.GetUserId())
            });
        }

        [Route("/issues/{id?}/post")]
        [PBugPermission("issue.post")]
        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] uint id, CommentPostRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            IssuePost post = (await Db.IssuePosts.AddAsync(new IssuePost()
            {
                AuthorId = HttpContext.User.IsAnonymous() ? null : new uint?((uint)HttpContext.User.GetUserId()),
                IssueId = id,
                ContainedText = req.Text,
                DateOfCreation = DateTime.UtcNow
            })).Entity;

            await Db.IssueActivities.AddActivity(HttpContext, id, new PostActivity()
            {
                ContainedText = req.Text,
                Post = post
            });

            await Db.SaveChangesAsync();

            return RedirectToAction("ViewTalk", "Issue", new { id = id }, post.Id.ToString());
        }

        [Route("/issues/posts/{id?}/edit")]
        [PBugPermission("issue.editpost")]
        [HttpGet]
        public async Task<IActionResult> EditPost([FromRoute] uint id)
        {
            return View(await Db.IssuePosts
                    .Include(x => x.Author)
                    .SingleAsync(x => x.Id == id));
        }

        [Route("/issues/posts/{id?}/edit")]
        [PBugPermission("issue.editpost")]
        [HttpPost]
        public async Task<IActionResult> EditPost([FromRoute] uint id, CommentPostRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            IssuePost post = await Db.IssuePosts.FindAsync(id);

            if (post.ContainedText != req.Text)
            {
                await Db.IssueActivities.AddActivity(HttpContext, id, new EditPostActivity()
                {
                    OldContainedText = post.ContainedText,
                    NewContainedText = req.Text,
                    PostId = id
                });

                post.DateOfEdit = DateTime.UtcNow;
                post.ContainedText = req.Text;

                await Db.SaveChangesAsync();
            }

            return RedirectToAction("ViewTalk", "Issue", new { id = post.IssueId }, post.Id.ToString());
        }

        [Route("/issues/{id?}/edit")]
        [PBugPermission("issue.editissue")]
        [HttpGet]
        public async Task<IActionResult> EditIssue([FromRoute] uint id)
        {
            return View(new IssueEditIssueModel()
            {
                Issue = await Db.Issues
                    .Include(x => x.Files)
                    .SingleAsync(x => x.Id == id),
                AllProjects = await Db.Projects.ToArrayAsync(),
                AllUsers = await Db.Users.ToArrayAsync()
            });
        }

        [Route("/issues/{id?}/edit")]
        [PBugPermission("issue.editissue")]
        [HttpPost]
        public async Task<IActionResult> EditIssue([FromRoute] uint id, EditIssueRequest req)
        {
            if (!ModelState.IsValid)
                return View();

            Issue i = await Db.Issues.FindAsync(id);

            string[] filesremovedproc = JsonSerializer.Deserialize<string[]>(req.RemovedFiles);

            if (filesremovedproc.Length > 0
            || req.NewName != i.Name
            || req.NewTags != i.Tags
            || req.NewDescription != i.Description
            || req.NewProjectID != i.ProjectId
            || req.NewAssigneeID != i.AssigneeId
            || req.NewStatus != i.Status
            || HttpContext.Request.Form.Files.Count > 0)
            {
                await Db.IssueActivities.AddActivity(HttpContext, id, new EditIssueActivity()
                {
                    OldName = i.Name,
                    NewName = req.NewName,
                    OldTags = i.Tags,
                    NewTags = req.NewTags ?? "",
                    OldDescription = i.Description,
                    NewDescription = req.NewDescription,
                    OldProjectId = i.ProjectId,
                    NewProjectId = req.NewProjectID,
                    OldAssigneeId = i.AssigneeId,
                    NewAssigneeId = req.NewAssigneeID == -1 ? null : new uint?((uint)req.NewAssigneeID),
                    OldStatus = i.Status,
                    NewStatus = req.NewStatus
                });

                i.Name = req.NewName;
                i.Tags = req.NewTags ?? "";
                i.Description = req.NewDescription;
                i.ProjectId = req.NewProjectID;
                i.AssigneeId = req.NewAssigneeID == -1 ? null : new uint?((uint)req.NewAssigneeID);
                i.Status = req.NewStatus;

                foreach (string uid in filesremovedproc)
                {
                    if (Path.GetFileName(uid) != uid)
                        // Something's fishy...
                        return Forbid();
                    Db.Remove(await Db.IssueFiles.FirstAsync(x => x.FileId == uid));
                    System.IO.File.Delete(Path.Combine("files", uid));
                }

                List<(IssueFile, IFormFile)> toProcess = new List<(IssueFile, IFormFile)>();

                foreach (IFormFile file in HttpContext.Request.Form.Files)
                {
                    toProcess.Add((
                        (await Db.IssueFiles.AddAsync(new IssueFile()
                        {
                            Issue = i,
                            FileName = file.FileName,
                            FileId = Convert.ToBase64String(AuthUtils.GetRandomData(12))
                                .Replace('/', '_')
                                .Replace('+', '-')
                        })).Entity,
                        file));
                }

                await Db.SaveChangesAsync();

                foreach ((IssueFile, IFormFile) pack in toProcess)
                {
                    using (FileStream fs = System.IO.File.OpenWrite(Path.Combine("files", pack.Item1.FileId)))
                        await pack.Item2.CopyToAsync(fs);
                }
            }

            return RedirectToAction("ViewTalk", new { id = id });
        }

        [Route("/issues/{id?}/watch")]
        [PBugPermission("issue.watch")]
        [HttpPost]
        public async Task<IActionResult> ToggleWatch([FromRoute] uint id)
        {
            if (HttpContext.User.GetUserId() != -1)
            {
                IssueWatcher w = await Db.IssueWatchers.SingleOrDefaultAsync(x => x.IssueId == id && x.WatcherId == HttpContext.User.GetUserId());
                if (w != null)
                    Db.Remove(w);
                else
                    await Db.AddAsync(new IssueWatcher()
                    {
                        IssueId = id,
                        WatcherId = (uint)HttpContext.User.GetUserId()
                    });

                await Db.SaveChangesAsync();
            }

            return RedirectToAction("ViewTalk", new { id = id });
        }

        [Route("/issues/posts/{id?}/delete")]
        [PBugPermission("issue.deleteissue")]
        [HttpPost]
        public async Task<IActionResult> DeleteIssue([FromRoute] uint id)
        {
            await Db.Database.ExecuteSqlInterpolatedAsync($"delete from issueactivities where issueid = {id}");
            await Db.Database.ExecuteSqlInterpolatedAsync($"delete from issueposts where issueid = {id}");
            await Db.Database.ExecuteSqlInterpolatedAsync($"delete from issuefiles where issueid = {id}");
            await Db.Database.ExecuteSqlInterpolatedAsync($"delete from issuewatchers where issueid = {id}");
            await Db.Database.ExecuteSqlInterpolatedAsync($"delete from issues where id = {id}");
            await Db.SaveChangesAsync();

            return RedirectToAction("Search");
        }
    }
}