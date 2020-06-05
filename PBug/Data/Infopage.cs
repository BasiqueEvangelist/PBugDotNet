using System;
using System.Collections.Generic;

namespace PBug.Data
{
    public partial class Infopage
    {
        public Infopage()
        {
            Comments = new HashSet<InfopageComment>();
        }

        public uint Id { get; set; }
        public string Path { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int? Secrecy { get; set; }
        public string Tags { get; set; }
        public uint? AuthorId { get; set; }
        public DateTime DateOfEdit { get; set; }
        public uint? EditorId { get; set; }
        public string Name { get; set; }
        public string ContainedText { get; set; }

        public virtual User Author { get; set; }
        public virtual User Editor { get; set; }
        public virtual ICollection<InfopageComment> Comments { get; set; }
        public virtual ICollection<KBActivity> Activities { get; set; }
    }
    public enum KBSecrecy
    {
        Public, Internal, Protected, Private
    }
}
