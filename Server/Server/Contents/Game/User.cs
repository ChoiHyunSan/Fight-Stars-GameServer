using Server;
using System.Numerics;

public enum PlayerState
{
    None,   // 초기 생성 단계
    Data,   // 데이터를 전달받은 상태
    Ready,  // 준비가 완료된 상태
    InGame, // 게임에 참여중인 상태
    Exit,   // 게임에서 나간 상태
}

public class User : GameObject
{
    private const float Speed = 5.0f; // 이동 속도

    public int UserId { get; set; }
    public int CharacterId { get; set; }
    public int SkinId { get; set; }

    public string nickname { get; set; } = string.Empty;
    public PlayerState State { get; set; } = PlayerState.None;
    public ClientSession Session { get; set; } = null;
    public string RoomId { get; set; } = string.Empty;

    public User(int userId, int characterId, int skinId, string roomId)
    {
        UserId = userId;
        CharacterId = characterId;
        SkinId = skinId;
        RoomId = roomId;
    }

    public override void Update(double deltaTime)
    {
        Move(deltaTime);
    }

    private void Move(double deltaTime)
    {
        // 이동 
        if (Velocity == Vector2.Zero)
        {
            Velocity = Vector2.Zero;
            return;
        }

        Vector2 normalizedVector = Vector2.Normalize(Velocity);
        Vector2 move = normalizedVector * Speed * (float)deltaTime;
        Vector2 newPos = Position + move;

        // 충돌 체크 (ex. 벽에 충돌한 경우에는 기존 위치로 되돌리기)
        Position = newPos;

        Console.WriteLine($"Player {nickname}`s current Position is {Position}");
    }
}