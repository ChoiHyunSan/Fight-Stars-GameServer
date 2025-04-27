using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Web
{
    public class RoomCreateRequest
    {
        public string Mode { get; set; }
        public List<UserGameInfo> UserInfos { get; set; } = new List<UserGameInfo>();
    }

    public class UserGameInfo
    {
        public long UserId { get; set; }
        public int CharacterId { get; set; }
        public int SkinId { get; set; }
    }

    public class RoomCreateResponse
    {
        public string RoomId { get; set; }
        public string Password { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
