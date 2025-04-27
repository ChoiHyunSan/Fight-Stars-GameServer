using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Contents.Room
{
    public class Room
    {
        public string RoomId { get; set; }
        public string Mode { get; set; }
        
        public string Password { get; set; }
        public List<long> UserIds { get; set; } = new List<long>();
    }
}
