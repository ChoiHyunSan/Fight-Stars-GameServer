using Google.Protobuf.Protocol;
using Server.Contents.Room;
using System.Text;
using System.Text.Json;
using static Server.Contents.Room.Room;

public static class GameResultAPIHelper
{
    public static async Task<CreateGameResultResponse> CreateGameResult(CreateGameResultRequest request)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string url = "http://127.0.0.1:5001/api/data/result"; ; 
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpResponse = await httpClient.PostAsync(url, content);
            httpResponse.EnsureSuccessStatusCode();

            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var resultResponse = JsonSerializer.Deserialize<CreateGameResultResponse>(jsonResponse);
            return resultResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DeathMatch] 게임 결과 전송 실패: {ex.Message}");
            return null;
        }
    }
}

public class CreateGameResultRequest
{
    public List<PlayerGameResult> PlayerGameResults { get; set; }
}

public class PlayerGameResult
{
    public int PlayerId { get; set; }
    public bool IsWin { get; set; }
    public int Exp { get; set; }
    public int Gold { get; set; }
}

public class CreateGameResultResponse
{
    public List<UserGameResultData> userGameResults { get; set; } = new();
}

public class UserGameResultData
{
    public long userId { get; set; }
    public int winCount { get; set; } = 0;
    public int loseCount { get; set; } = 0;
    public int totalPlayCount { get; set; } = 0;
    public int gold { get; set; } = 0;
    public int energy { get; set; } = 0;
    public int exp { get; set; } = 0;
    public int level { get; set; } = 1;
}