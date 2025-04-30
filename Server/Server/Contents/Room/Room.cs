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
            // 초기 결과값 설정
            EnterResult result = EnterResult.Success;
            User? user = null;

            lock (_lock)
            {
                // 1. 사용자 조회
                user = UserInfos.Find(u => u.UserId == userId);
                if (user == null)
                {
                    Console.WriteLine($"User not found! UserId: {userId}");
                    result = EnterResult.InvalidId;

                    // 사용자가 없는 경우에도 실패 응답을 보내기 위해 임시 응답 생성
                    S_EnterRoom failResponse = new S_EnterRoom()
                    {
                        EnterResult = result,
                        PlayerCount = UserInfos.Count
                    };
                    session.Send(failResponse);
                    return;
                }

                // 2. 비밀번호 검증
                if (!PasswordUtil.Verify(this.Password, password))
                {
                    Console.WriteLine($"Password not match! RoomId: {RoomId}, UserId: {userId}");
                    result = EnterResult.AccessDenied;
                }
                // 3. 사용자 상태 검증
                else if (user.State != PlayerState.None)
                {
                    Console.WriteLine($"User already in game! UserId: {userId}");
                    result = EnterResult.AlreadyInRoom;
                }
                // 4. 모든 검증 통과시 사용자 정보 업데이트
                else
                {
                    // 사용자 정보 업데이트
                    user.nickname = nickname;
                    user.Session = session;
                    session.User = user;
                    user.State = PlayerState.Data;
                }

                // 응답 생성 및 전송 
                S_EnterRoom enterRoom = new S_EnterRoom()
                {
                    EnterResult = result,
                    PlayerCount = UserInfos.Count
                };

                // 플레이어 정보 추가
                enterRoom.PlayerInfos.AddRange(UserInfos.Select(u => new PlayerInfo()
                {
                    UserId = u.UserId,
                    SkinId = u.SkinId,
                    SpawnPos = GetSpwanPos(u.UserId),
                }));

                Console.WriteLine(enterRoom);

                // 사용자 세션을 통해 응답 전송
                if (result == EnterResult.Success)
                {
                    user.Session.Send(enterRoom); 
                }
                else
                {
                    session.Send(enterRoom); 
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
