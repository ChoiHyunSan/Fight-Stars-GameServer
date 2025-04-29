using Server.Web;
using static Google.Protobuf.Protocol.S_EnterRoom.Types;

namespace Server.Contents.Room
{
    public class DeathMatch : Room
    {
        public override SpawnPos GetSpwanPos(int userId)
        {
            return new SpawnPos
            {
                X = 0,
                Y = 0,
            };
        }
    }
}
