using System.Collections.Generic;

namespace PBug.Data
{
    public partial class Project
    {
        public Project()
        {
            Issues = new HashSet<Issue>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        public string ShortProjectId { get; set; }
        public uint? AuthorId { get; set; }

        public virtual User Author { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
    }
}
