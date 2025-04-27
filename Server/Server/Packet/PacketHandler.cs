using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;


class PacketHandler
{
	public static void C_ChatHandler(PacketSession session, IMessage packet)
	{
		C_Chat chatPacket = packet as C_Chat;
		ClientSession clientSession = session as ClientSession;
		

	}
}
