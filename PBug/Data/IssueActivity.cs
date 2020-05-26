using System;

namespace PBug.Data
{
    public abstract partial class IssueActivity
    {
        public uint Id { get; set; }
        public DateTime? DateOfOccurance { get; set; }
        public uint IssueId { get; set; }
        public uint? AuthorId { get; set; }
        public string Type { get; set; }

        public virtual User Author { get; set; }
        public virtual Issue Issue { get; set; }
    }

    public partial class CreateIssueActivity : IssueActivity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public uint ProjectId { get; set; }
        public uint? AssigneeId { get; set; }

        public virtual Project Project { get; set; }
        public virtual User Assignee { get; set; }
    }

    public partial class EditIssueActivity : IssueActivity
    {
        public string OldName { get; set; }
        public string OldDescription { get; set; }
        public string OldTags { get; set; }
        public uint OldProjectId { get; set; }
        public uint? OldAssigneeId { get; set; }
        public IssueStatus OldStatus { get; set; }

        public string NewName { get; set; }
        public string NewDescription { get; set; }
        public string NewTags { get; set; }
        public uint NewProjectId { get; set; }
        public uint? NewAssigneeId { get; set; }
        public IssueStatus NewStatus { get; set; }

        public virtual Project OldProject { get; set; }
        public virtual Project NewProject { get; set; }
        public virtual User OldAssignee { get; set; }
        public virtual User NewAssignee { get; set; }
    }

    public partial class PostActivity : IssueActivity
    {
        public string ContainedText { get; set; }
        public uint PostId { get; set; }

        public virtual IssuePost Post { get; set; }
    }

    public partial class EditPostActivity : IssueActivity
    {
        public string OldContainedText { get; set; }
        public string NewContainedText { get; set; }
        public uint PostId { get; set; }

        public virtual IssuePost Post { get; set; }
    }
}
