using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Web;
using ServerCore;
using static Google.Protobuf.Protocol.S_EnterRoom.Types;

namespace Server.Contents.Room
{
    public abstract class Room : IJobQueue
    {
        public string RoomId { get; set; }
        public string Password { get; set; }

        public object _lock = new object(); // Lock 객체

        // 방에 참여한 유저 정보 
        public List<User> UserInfos { get; set; } = new List<User>();
        public abstract SpawnPos GetSpwanPos(int userId);

        public void Push(Action job)
        {
            throw new NotImplementedException();
        }

        public void SetData(string roomId, string password, List<UserGameInfo> userInfos)
        {
            RoomId = roomId;
            Password = password;
            foreach(UserGameInfo info in userInfos)
            {
                // TODO : (int) 캐스팅 하드코딩 수정
                var user = new User((int)info.UserId, info.CharacterId, info.SkinId, roomId);
                UserInfos.Add(user);
            }
        }

        public void EnterUser(int userId, string password, string nickname, ClientSession session)
        {
            // TODO : 비밀번호 체크

            // TODO : 유저 정보 체크
            lock(_lock)
            {
                User? user = UserInfos.Find(u => u.UserId == userId);
                if(user == null)
                {
                    Console.WriteLine($"User not found! UserId: {userId}");
                    return;
                }

                if (user.State != PlayerState.None)
                {
                    Console.WriteLine($"User already in game! UserId: {userId}");
                    return;
                }

                user.nickname = nickname;
                user.Session = session;
                session.User = user;

                if (user != null)
                {
                    S_EnterRoom enterRoom = new S_EnterRoom()
                    {
                        PlayerCount = UserInfos.Count
                    };
                    enterRoom.PlayerInfos.AddRange(UserInfos.Select(u => new PlayerInfo()
                    {
                        UserId = u.UserId,
                        SkinId = u.SkinId,
                        SpawnPos = GetSpwanPos(u.UserId),
                    }));
                    Console.WriteLine(enterRoom);
                    user.Session.Send(enterRoom);
                    user.State = PlayerState.Data;
                }
            }        
        }

        public void AddReadyPlayer(User user)
        {
            lock (_lock)
            {
                user.State = PlayerState.Ready;

                // 만약 모든 플레이어가 준비된 경우엔 게임 준비 완료상태임을 모든 플레이어에게 전달합니다.
                if (UserInfos.Find(u => u.State != PlayerState.Ready) == null)
                {
                    UserInfos.ForEach(u => u.State = PlayerState.InGame);

                    S_ReadyCompleteGame readyComplete = new S_ReadyCompleteGame()
                    {
                        StartTime = Timestamp.FromDateTime(DateTime.UtcNow)
                    };
                    Broadcast(readyComplete);
                }
            }
        }

        public void Broadcast(IMessage message)
        {
            lock (_lock)
            {
                foreach (User user in UserInfos)
                {
                    if (user.Session != null)
                    {
                        user.Session.Send(message);
                    }
                }
            }
        }
    }
}
