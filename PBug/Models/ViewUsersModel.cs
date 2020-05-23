using System.Collections.Generic;
using PBug.Data;

namespace PBug.Models
{
    public class ViewUsersModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Role> AllRoles { get; set; }
    }
}