namespace PBug.Data
{
    public partial class Invite
    {
        public uint Id { get; set; }
        public string Uid { get; set; }
        public uint? RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
