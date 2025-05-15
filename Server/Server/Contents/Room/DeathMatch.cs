using Google.Protobuf.Protocol;

namespace Server.Contents.Room
{
    public class DeathMatch : Room
    {
        public DeathMatch(MapData mapData) : base(mapData){}

        public override SpawnPos GetSpwanPos(int userId)
        {
            return new SpawnPos
            {
                X = -4,
                Y = 0,
            };
        }

        public override void Update(double deltaTime)
        {
            lock (_lock)
            {
                // 방에 참여한 유저들에 대한 업데이트
                foreach (User user in Users)
                {
                    user.Update(deltaTime);
                }

                foreach (GameObject gameobject in Objects)
                {
                    gameobject.Update(deltaTime);
                }

                foreach(GameObject gameobject in removeObjects)
                {
                    Objects.Remove(gameobject);
                }
            }
        }
    }
}
