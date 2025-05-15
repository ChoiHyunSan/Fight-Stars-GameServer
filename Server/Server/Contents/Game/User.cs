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

    public User(int userId, int characterId, int skinId, Room room)
    {
        this.userId = userId;
        this.characterId = characterId;
        this.skinId = skinId;
        this.room = room;
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
}