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
                if (string.IsNullOrEmpty(request.Mode) || request.UserInfos.Count <= 0)
                {
                    Console.WriteLine("Invalid Request");
                    return Results.BadRequest(new { error = "Invalid parameters", errorCode = 4001 });
                }

                var room = RoomManager.CreateRoom(request.Mode, request.UserInfos);
                if(room == null)
                {
                    Console.WriteLine("Room Creation Failed");
                    return Results.BadRequest(new { error = "Room creation failed", errorCode = 4002 });
                }

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

            Task.Run(() => app.Run("http://0.0.0.0:5005")); // 5000 포트로 HTTP 서버 오픈
        }
    }
}
