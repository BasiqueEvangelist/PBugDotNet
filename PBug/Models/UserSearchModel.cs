using System.Collections.Generic;
using PBug.Data;

namespace PBug.Models
{
    public class UserSearchModel
    {
        public string SearchString { get; set; }
        public IEnumerable<User> FoundUsers { get; set; }
    }
}