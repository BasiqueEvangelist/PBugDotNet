using System.Collections.Generic;
using PBug.Data;

namespace PBug.Models
{
    public class ViewInvitesModel
    {
        public IEnumerable<Invite> Invites { get; set; }
        public IEnumerable<Role> Roles { get; set; }
    }
}