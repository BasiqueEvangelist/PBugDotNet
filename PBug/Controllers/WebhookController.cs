using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using PBug.Data;

namespace PBug.Controllers;

public class WebhookController : Controller
{
    private readonly PBugContext _db;
    private readonly IOptionsSnapshot<AppConfig> _options;

    public WebhookController(PBugContext db, IOptionsSnapshot<AppConfig> options)
    {
        _db = db;
        _options = options;
    }

    private static readonly Regex ResolvesRegex = new Regex("^(?:Resolves|Fixes|Closes|Solves): [A-Z]{1,3}-(\\d+)$", RegexOptions.Compiled | RegexOptions.Multiline);

    [HttpPost, Route("/webhook/push"), Consumes("application/json")]
    public async Task<IActionResult> PushWebhook()
    {
        var ms = new MemoryStream();
        await HttpContext.Request.Body.CopyToAsync(ms);
        var bytes = ms.ToArray();
        
        var signatures = HttpContext.Request.Headers["X-Forgejo-Signature"];

        if (signatures.Count < 1)
            return Forbid();

        var signature = signatures[0];
        var expected = Convert.ToHexStringLower(HMACSHA256.HashData(Encoding.UTF8.GetBytes(_options.Value.WebhookSecretKey!), bytes));

        if (signature != expected)
            return Forbid();

        var pushEvent = JsonSerializer.Deserialize<PushEvent>(bytes);

        var visitedIssues = new HashSet<uint>();
        
        foreach (var commit in pushEvent.Commits)
        {
            foreach (var match in ResolvesRegex.Matches(commit.Message).OfType<Match>())
            {
                string issueIdStr = match.Groups[1].Value;
                
                if (!uint.TryParse(issueIdStr, out var issueId))
                    continue;
                
                if (!visitedIssues.Add(issueId)) continue;

                await ResolveIssueWith(issueId, pushEvent, commit);
;            }
        }

        await _db.SaveChangesAsync();

        return Ok();
    }

    private async Task ResolveIssueWith(uint issueId, PushEvent pushEvent, PushEventCommit commit)
    {
        string message = $"Commit [{commit.Id}]({commit.Url}) (by {commit.Author.Name}) was pushed by [{pushEvent.Pusher.FullName}]({pushEvent.Pusher.HtmlUrl})\n" +
                         $"\n" +
                         $"```\n" +
                         $"{commit.Message}" +
                         $"```";
        
        IssuePost post = (await _db.IssuePosts.AddAsync(new IssuePost()
        {
            AuthorId = 1,
            IssueId = issueId,
            ContainedText = message,
            DateOfCreation = DateTime.UtcNow
        })).Entity;

        await _db.IssueActivities.AddAsync(new PostActivity()
        {
            ContainedText = message,
            Post = post,
            DateOfOccurance = DateTime.UtcNow,
            AuthorId = 1,
            IssueId = issueId
        });
        
        Issue i = await _db.Issues.FindAsync(issueId);

        await _db.IssueActivities.AddAsync(new EditIssueActivity()
        {
            DateOfOccurance = DateTime.UtcNow,
            IssueId = issueId,
            AuthorId = 1,
            
            OldName = i.Name,
            NewName = i.Name,
            OldTags = i.Tags,
            NewTags = i.Tags,
            OldDescription = i.Description,
            NewDescription = i.Description,
            OldProjectId = i.ProjectId,
            NewProjectId = i.ProjectId,
            OldAssigneeId = i.AssigneeId,
            NewAssigneeId = i.AssigneeId,
            OldStatus = i.Status,
            NewStatus = IssueStatus.Closed
        });
        
        i.Status = IssueStatus.Closed;
    }
}