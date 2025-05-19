using Google.Protobuf.Protocol;
using Server;
using Server.Contents.Room;
using System.Numerics;

public enum PlayerState
{
    None,   // 초기 생성 단계
    Data,   // 데이터를 전달받은 상태
    Ready,  // 준비가 완료된 상태
    InGame, // 게임에 참여중인 상태
    Exit,   // 게임에서 나간 상태
}

public enum Team
{
    Red,
    Blue
}

public class User : GameObject
{
    private const float speed = 5.0f; // 이동 속도

    public int userId { get; set; }
    public int characterId { get; set; }
    public int skinId { get; set; }
    public string nickname { get; set; } = string.Empty;
    public PlayerState state { get; set; } = PlayerState.None;
    public ClientSession session { get; set; } = null;
    public Room room { get; set; }
    public Team team { get; set; }

    // 인게임 데이터
    public int hp;
    internal int maxHp;
    public bool isDead { get; internal set; }

    public User(int userId, int characterId, int skinId, Room room)
    {
        this.userId = userId;
        this.characterId = characterId;
        this.skinId = skinId;
        this.room = room;
        this.ColliderRadius = 1f;

        // TODO : 인게임 데이터를 characterId에 따라 세팅하도록 변경
        this.hp = 100;
        this.maxHp = 100; 
        this.isDead = false;
    }

    public override void Update(double deltaTime)
    {
        Move(deltaTime);
    }

    private void Move(double deltaTime)
    {
        if (Velocity == Vector2.Zero)
            return;

        Vector2 normalizedVector = Vector2.Normalize(Velocity);
        Vector2 move = normalizedVector * speed * (float)deltaTime;

        Vector2 tentativePos = Position;

        // X축 이동 먼저
        Vector2 newPosX = tentativePos + new Vector2(move.X, 0);
        if (!room.Map.IsBlocked(newPosX))
            tentativePos = newPosX;

        // Y축 이동
        Vector2 newPosY = tentativePos + new Vector2(0, move.Y);
        if (!room.Map.IsBlocked(newPosY))
            tentativePos = newPosY;

        Position = tentativePos;

        Console.WriteLine($"Player {nickname}`s current Position is {Position}");
    }

    public void OnDamaged(int projectileId, int damage, User killUser)
    {
        Room room = this.room;
        if (room == null)
        {
            return;
        }

        // 데미지를 가하여 HP 갱신
        hp -= damage;
        if(hp <= 0)
        {
            hp = 0;

            // 유저 사망처리 메서드를 호출
            room.OnUserDeath(this, killUser);
        }
        else
        {
            // 죽지 않았으므로 Attack 패킷을 전달 
            S_Attack attackPakcet = new S_Attack
            {
                ProjectileId = projectileId,
                Hp = hp,
                UserId = this.userId
            };
            room.Broadcast(attackPakcet);
        }
    }
}