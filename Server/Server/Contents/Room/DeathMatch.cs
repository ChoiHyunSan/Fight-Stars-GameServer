using Google.Protobuf.Protocol;

namespace Server.Contents.Room
{
    public class DeathMatch : Room
    {
        private float _remainingGameTime = 10f;

        public DeathMatch(MapData mapData) : base(mapData) { }

        public override SpawnPos GetSpwanPos(int userId)
        {
            return userId % 2 == 0 ? new SpawnPos { X = -4, Y = 0 } : new SpawnPos { X = -8, Y = 0 };
        }

        public override void UpdateGamemode(double deltaTime)
        {
            // 제한 시간 감소 처리
            _remainingGameTime -= (float)deltaTime;
            if (_remainingGameTime <= 0)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            Console.WriteLine("[DeathMatch] Game End");

            // API 서버에 게임 결과 전송
            CreateGameResultResponse response = GameResultAPIHelper
                .CreateGameResult(new CreateGameResultRequest
                {
                    PlayerGameResults = Users.Select(player => new PlayerGameResult
                    {
                        PlayerId = player.userId,
                        IsWin = player.team == Team.Blue
                            ? score.blue > score.red
                            : score.red > score.blue,
                        Exp = 100,
                        Gold = 100
                    }).ToList()
                }).GetAwaiter().GetResult();
            Console.WriteLine($"CreateGameResultResponse :  {response.ToString()}");

            if(response != null)
            {
                // TODO: S_GameOver 패킷 정의 필요
                S_Gameover gameOverPacket = new S_Gameover
                {
                    BlueScore = score.blue,
                    RedScore = score.red
                };
                Broadcast(gameOverPacket);
            }
            else
            {
                Console.WriteLine("[DeathMatch] Failed to send game result to API server.");
            }
            Release();
        }
    }
}
