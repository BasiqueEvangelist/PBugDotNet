using System.Collections.Generic;
using PBug.Data;

namespace PBug.Models
{
    public class IssueSearchModel
    {
        public string SearchString { get; set; }
        public IEnumerable<Issue> FoundIssues { get; set; }
    }
}