using System.Collections.Generic;

namespace PBug.Data
{
    public partial class Role
    {
        public Role()
        {
            Invites = new HashSet<Invite>();
            Users = new HashSet<User>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        public string Permissions { get; set; }

        public virtual ICollection<Invite> Invites { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
