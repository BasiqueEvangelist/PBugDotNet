using System.Collections.Generic;
using PBug.Data;

namespace PBug.Models
{
    public class KBSearchModel
    {
        public string SearchString { get; set; }
        public IEnumerable<Infopage> FoundInfopages { get; set; }
    }
}