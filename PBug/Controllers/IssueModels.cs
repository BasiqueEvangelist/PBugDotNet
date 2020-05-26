using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PBug.Data;

namespace PBug.Controllers
{
    public class CreateIssueRequest
    {
        //string name, string tags, string firsttext, uint projectid, int assigneeid
        [BindProperty(Name = "name"), BindRequired]
        [StringLength(100)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [BindProperty(Name = "tags"), BindRequired]
        public string Tags { get; set; }

        [BindProperty(Name = "firsttext"), BindRequired]
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }

        [BindProperty(Name = "projectid"), BindRequired]
        public uint ProjectID { get; set; }

        [BindProperty(Name = "assigneeid"), BindRequired]
        public int AssigneeID { get; set; }
    }
    public class CommentPostRequest
    {
        [BindProperty(Name = "text"), BindRequired]
        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; }
    }
    public class EditIssueRequest
    {
        [BindProperty(Name = "filesremoved"), BindRequired]
        public string RemovedFiles { get; set; }

        [BindProperty(Name = "newtitle"), BindRequired]
        [StringLength(100)]
        [Required(AllowEmptyStrings = false)]
        public string NewName { get; set; }

        [BindProperty(Name = "newtags"), BindRequired]
        public string NewTags { get; set; }

        [BindProperty(Name = "newtext"), BindRequired]
        [Required(AllowEmptyStrings = false)]
        public string NewDescription { get; set; }

        [BindProperty(Name = "newprojectid"), BindRequired]
        public uint NewProjectID { get; set; }

        [BindProperty(Name = "newassigneeid"), BindRequired]
        public int NewAssigneeID { get; set; }

        [BindProperty(Name = "newstatusid"), BindRequired]
        public IssueStatus NewStatus { get; set; }
    }
}