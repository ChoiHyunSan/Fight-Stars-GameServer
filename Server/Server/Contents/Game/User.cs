using Server;

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
        Console.WriteLine($"Player Update! Nickname : {nickname}");

        Move();
        
    }

    private void Move()
    {
        // 이동 

        // 충돌 체크 (ex. 벽에 충돌한 경우에는 기존 위치로 되돌리기)
    }
}