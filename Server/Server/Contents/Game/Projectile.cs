using Google.Protobuf.Protocol;
using Server.Contents.Room;
using System.Numerics;

public class Projectile : GameObject
{
    public int id;
    public int damage;
    public float size;
    public float speed;

    public Vector2 spawnPos;
    public float range;

    public Projectile(User user, Room room, int id, Vector2 position, Vector2 velocity)
    {
        this.user = user;
        this.id = id;
        Position = position;
        this.Velocity = Vector2.Normalize(velocity);
        spawnPos = position;

        // TODO : 하드코딩된 수치적 요소는 추후에 변경
        damage = 10;
        size = 1;
        speed = 10;
        range = 10;

        this.user = user;
        this.room = room;
    }

    public User user { get; set; }
    public Room room { get; set; }



    public override void Update(double deltaTime)
    {
        // 발사 방향으로 이동
        Move(deltaTime);
        Console.WriteLine($"Projectile {id} cur pos : {Position}");   

        // 플레이어에 피격된 경우 피격
        CheckPlayerHit();

        // 목적지까지 도착한 경우 소멸
        CheckArrival();
    }

    private void CheckArrival()
    {
        // 스폰위치로부터 range만큼 이동했다면 소멸하도록 처리
        float distance = Vector2.Distance(Position, spawnPos);
        if (distance >= range)
        {
            // 클라이언트에 소멸 패킷 전송
            S_DestroyProjectile destroyPacket = new S_DestroyProjectile
            {
                ProjectileId = id
            };

            room.removeObjects.Add(this);
            room.Broadcast(destroyPacket);
            this.user = null;
            this.room = null;
            Console.WriteLine($"Projectile Destroy, SpawnPos : {spawnPos}, Cur: {Position}");
        }
    }

    private void CheckPlayerHit()
    {
        List<User> users = room.Users;
        foreach(var user in users)
        {
            if(user == this.user || user.team == this.user.team)
            {
                continue;
            }

            // 피격시킨 경우 Attack 패킷 전달
            
        }
    }

    private void Move(double deltaTime)
    {
        if (Velocity == Vector2.Zero)
            return;

        Vector2 move = Velocity * speed * (float)deltaTime;
        Position += move;
    }
}
