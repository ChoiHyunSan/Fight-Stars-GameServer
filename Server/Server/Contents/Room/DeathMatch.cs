using Server.Web;
using static Google.Protobuf.Protocol.S_EnterRoom.Types;

namespace Server.Contents.Room
{
    public class DeathMatch : Room
    {
        public DeathMatch(MapData mapData) : base(mapData){}

        public override SpawnPos GetSpwanPos(int userId)
        {
            return new SpawnPos
            {
                X = 0,
                Y = 0,
            };
        }

        public override void Update(double deltaTime)
        {
            // 방에 참여한 유저들에 대한 업데이트
            foreach (User user in Users)
            {
                user.Update(deltaTime);
            }
        }
    }
}
