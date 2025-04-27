using System.Collections.Generic;

namespace Server.Contents.Room
{
    public static class RoomManager
    {
        // RoomId를 키로 하는 방 목록
        public static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();

        public static object _lock = new object();

        public static Room createRoom(string mode, List<long> userIds)
        {
            lock (_lock)
            {
                var roomId = Guid.NewGuid().ToString("N").Substring(0, 8); // 예: "4fd1c3b2"
                Console.WriteLine($"Room Create! Room Id : {roomId}");
                var room = new Room
                {
                    RoomId = roomId,
                    Mode = mode,
                    UserIds = userIds
                };
                Rooms[roomId] = room;
                return room;
            }
        }
    }
}
