using Server.Web;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Contents.Room
{
    public class Room : IJobQueue
    {
        public string RoomId { get; set; }
        public string Password { get; set; }

        public List<UserGameInfo> UserInfos { get; set; } = new List<UserGameInfo>();

        public void Push(Action job)
        {
            throw new NotImplementedException();
        }
    }
}
