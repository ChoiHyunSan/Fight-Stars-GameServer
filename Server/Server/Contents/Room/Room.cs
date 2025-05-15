using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Web;
using ServerCore;
using System.Numerics;
using static Google.Protobuf.Protocol.S_EnterRoom.Types;
using static Google.Protobuf.Protocol.S_PositionUpdate.Types;

namespace Server.Contents.Room
{
    public enum RoomState
    {
        Waiting,
        InGame,
        End,
    }

    public abstract class Room : IJobQueue
    {
        private const int TickRate = 30;                        // 초당 30 틱
        private const double TickInterval = 1000.0 / TickRate;  // 밀리초 단위 (33.33ms)
        private const int SyncIntervalTick = 5;                 // 틱 간격 (2틱마다 동기화)

        public string RoomId { get; set; }
        public string Password { get; set; }

        public MapData Map;

        public RoomState State { get; set; } = RoomState.Waiting;

        public object _lock = new object(); // Lock 객체

        private Task Task; // Task 객체

        // 방에 참여한 유저 정보 
        public List<User> Users { get; set; } = new List<User>();

        public List<GameObject> Objects { get; set; } = new List<GameObject>();
        public List<GameObject> removeObjects { get; set; } = new List<GameObject>();
        private int generateId = 1;

        public abstract SpawnPos GetSpwanPos(int userId);

        public Room(MapData mapData)
        {
            Map = mapData;

            // Room Task 시작
            Task = new Task(async () =>
            {
                Console.WriteLine("New Room Task Start");
                DateTime _nextTickTime = DateTime.UtcNow;
                int tickCount = 0; 

                while (true)
                {
                    if (State == RoomState.InGame)
                    {
                        var now = DateTime.UtcNow;
                        if (now >= _nextTickTime)
                        {
                            double deltaTime = TickInterval / 1000.0;

                            // 1. 모든 오브젝트 업데이트 (이동, 충돌 등)
                            Update(deltaTime);

                            tickCount++;

                            // 2. 일정 틱 간격마다 위치 동기화 패킷 전송
                            if (tickCount % SyncIntervalTick == 0)
                            {
                                BroadcastPositionUpdates();
                            }

                            _nextTickTime = _nextTickTime.AddMilliseconds(TickInterval);
                        }
                    }

                    if (State == RoomState.End)
                        break;

                    await Task.Delay(1);
                }
            });
            Task.Start();
        }

        private void BroadcastPositionUpdates()
        {
            S_PositionUpdate positionUpdatePacket = new S_PositionUpdate();
            positionUpdatePacket.PlayerPosUpdates.AddRange(Users.Select(user => new PlayerPosUpdate()
            {
                UserId = user.userId,
                X = user.Position.X,
                Y = user.Position.Y,
                Vx = user.Velocity.X,
                Vy = user.Velocity.Y,
            }));
            Broadcast(positionUpdatePacket);
        }

        public abstract void Update(double deltaTime);

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
                var user = new User((int)info.UserId, info.CharacterId, info.SkinId, this);
                SpawnPos spawnPos = GetSpwanPos(user.userId);
                user.Position = new Vector2(spawnPos.X, spawnPos.Y);

                Users.Add(user);
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
                user = Users.Find(u => u.userId == userId);
                if (user == null)
                {
                    Console.WriteLine($"User not found! UserId: {userId}");
                    result = EnterResult.InvalidId;

                    // 사용자가 없는 경우에도 실패 응답을 보내기 위해 임시 응답 생성
                    S_EnterRoom failResponse = new S_EnterRoom()
                    {
                        EnterResult = result,
                        PlayerCount = Users.Count
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
                else if (user.state != PlayerState.None)
                {
                    Console.WriteLine($"User already in game! UserId: {userId}");
                    result = EnterResult.AlreadyInRoom;
                }
                // 4. 모든 검증 통과시 사용자 정보 업데이트
                else
                {
                    // 사용자 정보 업데이트
                    user.nickname = nickname;
                    user.session = session;
                    session.User = user;
                    user.state = PlayerState.Data;
                }

                // 응답 생성 및 전송 
                S_EnterRoom enterRoom = new S_EnterRoom()
                {
                    EnterResult = result,
                    PlayerCount = Users.Count
                };

                // 플레이어 정보 추가
                enterRoom.PlayerInfos.AddRange(Users.Select(u => new PlayerInfo()
                {
                    UserId = u.userId,
                    SkinId = u.skinId,
                    CharacterId = u.characterId,
                    SpawnPos = GetSpwanPos(u.userId),
                }));

                Console.WriteLine(enterRoom);

                // 사용자 세션을 통해 응답 전송
                if (result == EnterResult.Success)
                {
                    user.session.Send(enterRoom); 
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
                user.state = PlayerState.Ready;

                // 만약 모든 플레이어가 준비된 경우엔 게임 준비 완료상태임을 모든 플레이어에게 전달합니다.
                if (Users.Find(u => u.state != PlayerState.Ready) == null)
                {
                    Users.ForEach(u => u.state = PlayerState.InGame);

                    S_ReadyCompleteGame readyComplete = new S_ReadyCompleteGame()
                    {
                        StartTime = Timestamp.FromDateTime(DateTime.UtcNow)
                    };
                    Broadcast(readyComplete);
                }

                State = RoomState.InGame;
            }
        }

        public void Broadcast(IMessage message)
        {
            lock (_lock)
            {
                foreach (User user in Users)
                {
                    if (user.session != null)
                    {
                        user.session.Send(message);
                    }
                }
            }
        }

        public int LeaveUser(int userId)
        {
            lock (_lock)
            {
                // 사용자 조회
                User? user = Users.Find(u => u.userId == userId);
                if (user == null)
                {
                    Console.WriteLine($"User not found! UserId: {userId}");
                    return Users.Count;
                }

                // 사용자 상태 업데이트
                user.state = PlayerState.Exit;
                user.session = null;

                // 사용자 제거
                Console.WriteLine($"User left the room! UserId: {userId}");
                Users.Remove(user);
                return Users.Count;
            }
        }
        public void Release()
        {
            lock (_lock)
            {
                // 방 종료시 모든 사용자 상태 업데이트
                foreach (User user in Users)
                {
                    user.state = PlayerState.Exit;
                    user.session = null;
                }

                // 방 종료
                State = RoomState.End;
                Console.WriteLine($"Room Release, Room ID : {RoomId}");
            }
        }

        public void UpdatePlayerDir(User user, float dx, float dy)
        {
            if (user == null)
            {
                return;
            }

            lock (_lock)
            {
                Vector2 newDir = new Vector2(dx, dy);
                if (Users.Find(u => u.userId == user.userId) != null)
                {
                    user.Velocity = newDir;
                }
            }
        }

        public void CreateProjectile(User user, float vx, float vy)
        {
            Console.WriteLine($"Create Projectile, vx : {vx}. vy : {vy}");

            if(user == null)
            {
                return;
            }

            lock (_lock)
            {
                // 쿨타임이 체크 (너무 빨리 쏘려고 하는 경우를 방지)

                // 투사체 생성
                Vector2 velocity = new Vector2(vx, vy);
                var projectile = new Projectile(
                        user,
                        this,
                        generateId++,
                        user.Position,
                        velocity
                );
                Objects.Add(projectile);

                S_Fire firePacket = new S_Fire
                {
                    X = projectile.Position.X,
                    Y = projectile.Position.Y,
                    Vx = projectile.Velocity.X,
                    Vy = projectile.Velocity.Y,
                    ProjectileInfo = new S_Fire.Types.ProjectileInfo
                    {
                        Damage = projectile.damage,
                        Team = user.team.ToString(),
                        Type = 1,
                        UserId = projectile.user.userId,
                        Speed = projectile.speed,
                        ProjectileId = projectile.id
                    }
                };
                Broadcast(firePacket);
            }
        }
    }
}
