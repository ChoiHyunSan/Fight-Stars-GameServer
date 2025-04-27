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
                if (string.IsNullOrEmpty(request.Mode) || request.UserIds.Count <= 0)
                {
                    Console.WriteLine("Invalid Request");
                    return Results.BadRequest(new { error = "Invalid parameters", errorCode = 4001 });
                }

                var room = RoomManager.createRoom(request.Mode, request.UserIds);
                var response = new RoomCreateResponse
                {
                    RoomId = room.RoomId,
                    Password = room.Password,

                    // TODO : 유동적으로 변경 가능하도록 수정 필요
                    Ip = "127.0.0.1",   // 고정 IP
                    Port = 7777         // 고정 포트
                };
                return Results.Ok(response);
            });

            app.Run("http://0.0.0.0:5005"); // 5000 포트로 HTTP 서버 오픈
        }
    }
}
