using System;
using System.Collections.Generic;

namespace PBug.Data
{
    public partial class IssueFile
    {
        public uint Id { get; set; }
        public uint? IssueId { get; set; }
        public string FileName { get; set; }
        public string FileId { get; set; }

        public virtual Issue Issue { get; set; }
    }
}
