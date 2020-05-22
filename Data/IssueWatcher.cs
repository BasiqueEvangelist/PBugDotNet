using System;
using System.Collections.Generic;

namespace PBug.Data
{
    public partial class IssueWatcher
    {
        public uint WatcherId { get; set; }
        public uint IssueId { get; set; }

        public virtual Issue Issue { get; set; }
        public virtual User Watcher { get; set; }
    }
}
