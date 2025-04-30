using Server.Web;

namespace Server.Contents.Room
{
    public static class RoomManager
    {
        // RoomId를 키로 하는 방 목록
        public static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();
        public static Dictionary<string, MapData> mapDatas = new Dictionary<string, MapData>();

        public static object _lock = new object();

        public static void init()
        {
            
        }

        public static Room? CreateRoom(string mode, List<UserGameInfo> userInfos)
        {
            lock (_lock)
            {
                var roomId = Guid.NewGuid().ToString("N").Substring(0, 8); // 예: "4fd1c3b2"
                Console.WriteLine($"Room Create! Room Id : {roomId}");
                Console.WriteLine($"User Count : {userInfos.Count}");
                foreach(UserGameInfo info in userInfos)
                {
                    Console.WriteLine($"UserId : {info.UserId}, CharacterId : {info.CharacterId}, Skin ID : {info.SkinId}");
                }

                var password = PasswordUtil.CreatePassword();

                // TODO : 방 생성 로직 추가
                var room = CreateRoomByMode(mode);
                if(room == null)
                {
                    Console.WriteLine("Room Creation Failed");
                    return null;
                }
                room.SetData(roomId, password, userInfos);

                Rooms[roomId] = room;
                return room;
            }
        }

        public static void RemoveRoom(string roomId)
        {
            lock (_lock)
            {
                if (Rooms.ContainsKey(roomId))
                {
                    Room room = Rooms[roomId];
                    room.Release();

                    Rooms.Remove(roomId);
                    Console.WriteLine($"Room Removed! Room Id : {roomId}");
                }
            }
        }

        public static Room? GetRoom(string roomId)
        {
            lock (_lock)
            {
                if (Rooms.ContainsKey(roomId))
                {
                    return Rooms[roomId];
                }
                return null;
            }
        }

        private static Room? CreateRoomByMode(string mode)
        {
            Room room = null;
            switch (mode)
            {
                case "deathmatch":
                    room = new DeathMatch(mapDatas.GetValueOrDefault("deathmatch", null));
                    break;
                default:
                    break;
            }
            return room;
        }
    }
}
