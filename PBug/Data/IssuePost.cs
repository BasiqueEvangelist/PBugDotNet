using System;

namespace PBug.Data
{
    public partial class IssuePost
    {
        public uint Id { get; set; }
        public uint? AuthorId { get; set; }
        public uint IssueId { get; set; }
        public string ContainedText { get; set; }
        public DateTime DateOfCreation { get; set; }
        public DateTime? DateOfEdit { get; set; }

        public virtual User Author { get; set; }
        public virtual Issue Issue { get; set; }
    }
}
