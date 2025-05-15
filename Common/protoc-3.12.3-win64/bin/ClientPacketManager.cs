using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnterRoom, MakePacket<S_EnterRoom>);
		_handler.Add((ushort)MsgId.SEnterRoom, PacketHandler.S_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.SReadyCompleteGame, MakePacket<S_ReadyCompleteGame>);
		_handler.Add((ushort)MsgId.SReadyCompleteGame, PacketHandler.S_ReadyCompleteGameHandler);		
		_onRecv.Add((ushort)MsgId.SPositionUpdate, MakePacket<S_PositionUpdate>);
		_handler.Add((ushort)MsgId.SPositionUpdate, PacketHandler.S_PositionUpdateHandler);		
		_onRecv.Add((ushort)MsgId.SFire, MakePacket<S_Fire>);
		_handler.Add((ushort)MsgId.SFire, PacketHandler.S_FireHandler);		
		_onRecv.Add((ushort)MsgId.SAttack, MakePacket<S_Attack>);
		_handler.Add((ushort)MsgId.SAttack, PacketHandler.S_AttackHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SRespawn, MakePacket<S_Respawn>);
		_handler.Add((ushort)MsgId.SRespawn, PacketHandler.S_RespawnHandler);		
		_onRecv.Add((ushort)MsgId.SDestroyProjectile, MakePacket<S_DestroyProjectile>);
		_handler.Add((ushort)MsgId.SDestroyProjectile, PacketHandler.S_DestroyProjectileHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}