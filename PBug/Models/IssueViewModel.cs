using PBug.Data;

namespace PBug.Models
{
    public class IssueViewModel
    {
        public Issue Issue { get; set; }
        public bool IsWatching { get; set; }
        public MarkdownHelper MarkdownHelper { get; set; }
    }
}