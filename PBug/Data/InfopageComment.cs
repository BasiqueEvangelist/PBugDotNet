using System;
using System.Collections.Generic;

namespace PBug.Data
{
    public partial class InfopageComment
    {
        public uint Id { get; set; }
        public DateTime DateOfCreation { get; set; }
        public uint? AuthorId { get; set; }
        public uint? InfopageId { get; set; }
        public DateTime? DateOfEdit { get; set; }
        public string ContainedText { get; set; }

        public virtual User Author { get; set; }
        public virtual Infopage Infopage { get; set; }
    }
}
