using System;
namespace PBug.Data
{
    public class Session
    {
        public byte[] Id { get; set; }
        public byte[] SessionData { get; set; }
        public DateTimeOffset? Expires { get; set; }
    }
}