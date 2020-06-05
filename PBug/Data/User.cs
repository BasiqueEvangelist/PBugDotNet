using System.Collections.Generic;

namespace PBug.Data
{
    public partial class User
    {
        public User()
        {
            InfopageComments = new HashSet<InfopageComment>();
            InfopagesAuthored = new HashSet<Infopage>();
            InfopagesEdited = new HashSet<Infopage>();
            IssueActivities = new HashSet<IssueActivity>();
            IssuePosts = new HashSet<IssuePost>();
            IssuesAssignedTo = new HashSet<Issue>();
            IssuesAuthored = new HashSet<Issue>();
            IssuesWatched = new HashSet<IssueWatcher>();
            Projects = new HashSet<Project>();
        }

        public uint Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public uint? RoleId { get; set; }
        public PasswordFunction PasswordFunc { get; set; }
        public string Bio { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<InfopageComment> InfopageComments { get; set; }
        public virtual ICollection<Infopage> InfopagesAuthored { get; set; }
        public virtual ICollection<Infopage> InfopagesEdited { get; set; }
        public virtual ICollection<IssueActivity> IssueActivities { get; set; }
        public virtual ICollection<KBActivity> KBActivities { get; set; }
        public virtual ICollection<IssuePost> IssuePosts { get; set; }
        public virtual ICollection<Issue> IssuesAssignedTo { get; set; }
        public virtual ICollection<Issue> IssuesAuthored { get; set; }
        public virtual ICollection<IssueWatcher> IssuesWatched { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }

    public enum PasswordFunction : byte
    {
        PBKDF2
    }
}
