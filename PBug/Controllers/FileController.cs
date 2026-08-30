using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBug.Authentication;
using PBug.Data;

namespace PBug.Controllers
{
    public class FileController : Controller
    {
        private readonly PBugContext db;
        private readonly AppConfig cfg;

        public FileController(PBugContext db, IOptions<AppConfig> cfg)
        {
            this.db = db;
            this.cfg = cfg.Value;
        }

        [Route("/file/{uid?}")]
        [Permission("issue.view")]
        public async Task<IActionResult> Download([FromRoute] string uid)
        {
            if (Path.GetFileName(uid) != uid)
                // Something's fishy...
                return Forbid();
            IssueFile ifi = await db.IssueFiles.SingleAsync(x => x.FileId == uid);
            return File(System.IO.File.OpenRead(Path.Combine(cfg.FilesDirectory, uid)), "application/octet-stream", ifi.FileName);
        }
    }
}