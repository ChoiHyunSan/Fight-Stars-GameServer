using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

class PacketHandler
{
    internal static void C_EnterRoomHandler(PacketSession session, IMessage message)
    {
        C_EnterRoom enterRoomPacket = message as C_EnterRoom;
        ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"Handle EnterRoom Packet! {enterRoomPacket.RoomId}, {enterRoomPacket.UserId}");
    }

    internal static void C_ReadyCompleteGameHandler(PacketSession session, IMessage message)
    {
        throw new NotImplementedException();
    }
}
