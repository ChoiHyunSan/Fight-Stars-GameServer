using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Web
{
    public class RoomCreateRequest
    {
        public string Mode { get; set; }
        public int MaxPlayers { get; set; }
        public string MapId { get; set; }
    }

    public class RoomCreateResponse
    {
        public string RoomId { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
