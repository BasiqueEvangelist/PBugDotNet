using System;

namespace PBug.Data
{
    public abstract partial class KBActivity
    {
        public uint Id { get; set; }
        public DateTime? DateOfOccurance { get; set; }
        public uint InfopageId { get; set; }
        public uint? AuthorId { get; set; }
        public string Type { get; set; }

        public virtual User Author { get; set; }
        public virtual Infopage Infopage { get; set; }
    }

    public partial class CreatePageActivity : KBActivity
    {
        public string Name { get; set; }
        public string ContainedText { get; set; }
        public string Tags { get; set; }
        public int? Secrecy { get; set; }
    }

    public partial class EditPageActivity : KBActivity
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public string OldContainedText { get; set; }
        public string NewContainedText { get; set; }
        public string OldTags { get; set; }
        public string NewTags { get; set; }
        public int? OldSecrecy { get; set; }
        public int? NewSecrecy { get; set; }
    }

    public partial class CommentActivity : KBActivity
    {
        public string ContainedText { get; set; }
        public uint CommentId { get; set; }

        public virtual InfopageComment Comment { get; set; }
    }

    public partial class EditCommentActivity : KBActivity
    {
        public string OldContainedText { get; set; }
        public string NewContainedText { get; set; }
        public uint CommentId { get; set; }

        public virtual InfopageComment Comment { get; set; }
    }
}