using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Web
{
    public class RoomCreateRequest
    {
        public string Mode { get; set; }
        public List<long> UserIds { get; set; } = new List<long>();
    }

    public class RoomCreateResponse
    {
        public string RoomId { get; set; }
        public string Password { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
