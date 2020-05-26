using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBug.Authentication;
using PBug.Data;

namespace PBug.Controllers
{
    public class FileController : Controller
    {
        private readonly PBugContext db;

        public FileController(PBugContext db)
        {
            this.db = db;
        }

        [Route("/file/{uid?}")]
        [PBugPermission("issue.view")]
        public async Task<IActionResult> Download([FromRoute] string uid)
        {
            if (Path.GetFileName(uid) != uid)
                // Something's fishy...
                return Forbid();
            IssueFile ifi = await db.IssueFiles.SingleAsync(x => x.FileId == uid);
            return File(System.IO.File.OpenRead(Path.Combine("files", uid)), "application/octet-stream", ifi.FileName);
        }
    }
}