using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Contents.Room
{
    public class Room
    {
        public string RoomId { get; set; }
        public string Mode { get; set; }
        public int MaxPlayers { get; set; }
        public string MapId { get; set; }
    }
}
