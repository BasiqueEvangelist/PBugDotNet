using System.Collections.Generic;
using PBug.Data;

namespace PBug.Models
{
    public class NewsSearchModel
    {
        public string SearchString { get; set; }
        public IEnumerable<IssueActivity> FoundNews { get; set; }
    }
}