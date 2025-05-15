using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Contents.Room;
using ServerCore;

class PacketHandler
{
    public static void C_EnterRoomHandler(PacketSession session, IMessage message)
    {
        C_EnterRoom? enterRoomPacket = message as C_EnterRoom;
        ClientSession? clientSession = session as ClientSession;
        if (enterRoomPacket == null || clientSession == null)
        {
            Console.WriteLine("Invalid packet or session. in C_EnterRoomHandler");
            return;
        }
        Console.WriteLine($"Handle EnterRoom Packet! {enterRoomPacket.RoomId}, {enterRoomPacket.UserId}");

        Room? room = RoomManager.GetRoom(enterRoomPacket.RoomId);
        if (room == null)
        {
            Console.WriteLine($"Room not found! RoomId: {enterRoomPacket.RoomId}");
            return;
        }

        room.EnterUser(enterRoomPacket.UserId, enterRoomPacket.Password, enterRoomPacket.Nickname, clientSession);
    }

    // TODO : 방을 준비하는 단계이므로, 하나라도 실패하는 경우, 대기중인 모든 세션들에 대해서 접속을 해제하도록 유도해야 함.
    public static void C_ReadyCompleteGameHandler(PacketSession session, IMessage message)
    {
        C_ReadyCompleteGame? readyCompletePacket = message as C_ReadyCompleteGame;
        ClientSession? clientSession = session as ClientSession;
        if(readyCompletePacket == null || clientSession == null)
        {
            Console.WriteLine("Invalid packet or session. in C_ReadyCompleteGameHandler");
            return;
        }

        Console.WriteLine($"Client {readyCompletePacket.UserId} is Ready Now!");
        if(clientSession.User == null)
        {
            Console.WriteLine("User not found in session.");
            return;
        }

        Room? room = clientSession.User.room;
        if (room == null)
        {
            Console.WriteLine($"Room not found! RoomId: {clientSession.User.room}");
            return;
        }
        room.AddReadyPlayer(clientSession.User);

    }

    public static void C_MoveHandler(PacketSession session, IMessage message)
    {
        C_Move? movePacket = message as C_Move;
        ClientSession? clientSession = session as ClientSession;
        if (movePacket == null || clientSession == null)
        {
            Console.WriteLine("Invalid packet or session. in C_MoveHandler");
            return;
        }

        // TODO: 플레이어의 이동 방향을 변경
        Room? room = clientSession.User.room;
        if (room == null)
        {
            Console.WriteLine($"Room not found! RoomId: {clientSession.User.room}");
            return;
        }

        room.UpdatePlayerDir(clientSession.User, movePacket.Dx, movePacket.Dy);
    }

    public static void C_FireHandler(PacketSession session, IMessage message)
    {
        C_Fire? firePacket = message as C_Fire;
        ClientSession? clientSession = session as ClientSession;
        if(firePacket == null || clientSession == null)
        {
            Console.WriteLine("Invalid packet or session. in C_Fire");
            return;
        }

        Room? room = clientSession.User.room;
        if (room == null)
        {
            Console.WriteLine($"Room not found! RoomId: {clientSession.User.room}");
            return;
        }

        room.CreateProjectile(clientSession.User, firePacket.Vx, firePacket.Vy);
    }
}
