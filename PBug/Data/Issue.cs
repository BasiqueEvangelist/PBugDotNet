using System;
using System.Collections.Generic;

namespace PBug.Data
{
    public partial class Issue
    {
        public Issue()
        {
            Activities = new HashSet<IssueActivity>();
            Files = new HashSet<IssueFile>();
            Posts = new HashSet<IssuePost>();
            Watchers = new HashSet<IssueWatcher>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public uint? AuthorId { get; set; }
        public uint ProjectId { get; set; }
        public IssueStatus Status { get; set; }
        public uint? AssigneeId { get; set; }
        public string Description { get; set; }
        public DateTime DateOfCreation { get; set; }

        public virtual User Assignee { get; set; }
        public virtual User Author { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<IssueActivity> Activities { get; set; }
        public virtual ICollection<IssueFile> Files { get; set; }
        public virtual ICollection<IssuePost> Posts { get; set; }
        public virtual ICollection<IssueWatcher> Watchers { get; set; }
    }

    public enum IssueStatus : short
    {
        Open, Closed, Resolved, WontFix
    }
}
