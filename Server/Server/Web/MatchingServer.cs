using Server.Contents.Room;
using System;


namespace Server.Web
{
    public class MatchingServer
    {
        public static void Start(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapPost("/create-room", async (RoomCreateRequest request) =>
            {
                if (string.IsNullOrEmpty(request.Mode) || request.MaxPlayers <= 0)
                {
                    Console.WriteLine("Invalid Request");
                    return Results.BadRequest(new { error = "Invalid parameters", errorCode = 4001 });
                }

                var roomId = Guid.NewGuid().ToString("N").Substring(0, 8); // 예: "4fd1c3b2"
                Console.WriteLine($"Room Create! Room Id : {roomId}");

                var room = new Room
                {
                    RoomId = roomId,
                    Mode = request.Mode,
                    MaxPlayers = request.MaxPlayers,
                    MapId = request.MapId ?? "default"
                };

                RoomManager.Rooms[roomId] = room;

                return Results.Ok(new
                {
                    roomId = room.RoomId,
                    ip = "127.0.0.1", // 여기에 실제 게임 서버 IP 넣기
                    port = 7777        // 고정 포트
                });
            });

            app.Run("http://0.0.0.0:5005"); // 5000 포트로 HTTP 서버 오픈
        }
    }
}
